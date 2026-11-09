using Google.GenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using WorksheetGen.Api.Dtos.Worksheets;
using WorksheetGen.Api.Options;

namespace WorksheetGen.Api.Services
{
    public class GeminiWorksheetGeneratorService(
        Client geminiClient,
        IOptions<GeminiOptions> geminiOptions,
        ILogger<GeminiWorksheetGeneratorService> logger
    ) : IWorksheetGeneratorService
    {
        public async Task<AiGeneratedWorksheet> GenerateWorksheetAsync(
            GenerateWorksheetRequest request,
            string? extractedSourceText,
            CancellationToken cancellationToken = default
        )
        {
            List<string> modelsToTry =
            [
                geminiOptions.Value.Model,
                .. geminiOptions.Value.FallbackModels.Where(m => m != geminiOptions.Value.Model),
            ];

            List<ChatMessage> messages =
            [
                new ChatMessage(
                    ChatRole.System,
                    """
                    You are an expert teacher's assistant that creates structured school activity sheets.
                    Generate clear, pedagogically sound questions strictly grounded in the provided source material and parameters.
                    Respond in the language of the source material or topic (default to Czech if unspecified).
                    """
                ),
                new ChatMessage(
                    ChatRole.User,
                    $"""
                    Subject: {request.Subject}
                    Topic: {request.Topic}
                    Grade Level: {request.GradeLevel}
                    Number of questions: {request.QuestionCount}
                    Allowed question types: {string.Join(", ", request.QuestionTypes)}
                    Include correct answers: {request.IncludeAnswers}
                    Assign points per question: {request.IncludePoints}
                    Teacher notes: {request.AdditionalNotes ?? "None"}

                    Source Material:
                    {extractedSourceText
                    ?? request.RawSourceText
                    ?? "No external text provided; generate based on the topic."}
                    """
                ),
            ];

            ChatOptions chatOptions = new ChatOptions { Temperature = 0.4f };
            Exception? lastException = null;

            foreach (string modelId in modelsToTry)
            {
                using CancellationTokenSource perModelCts = new CancellationTokenSource(
                    TimeSpan.FromSeconds(25)
                );
                using CancellationTokenSource linkedCts =
                    CancellationTokenSource.CreateLinkedTokenSource(
                        cancellationToken,
                        perModelCts.Token
                    );

                try
                {
                    logger.LogInformation(
                        "Attempting worksheet generation with model: {modelId}",
                        modelId
                    );

                    IChatClient chatClient = geminiClient.AsIChatClient(modelId);

                    ChatResponse<AiGeneratedWorksheet> response =
                        await chatClient.GetResponseAsync<AiGeneratedWorksheet>(
                            messages,
                            chatOptions,
                            useJsonSchemaResponseFormat: true,
                            linkedCts.Token
                        );

                    if (response.Result is not null)
                    {
                        return response.Result;
                    }
                }
                catch (OperationCanceledException ex)
                    when (!cancellationToken.IsCancellationRequested)
                {
                    lastException = ex;
                    logger.LogWarning(
                        ex,
                        "Model {modelId} timed out after 25s (likely stuck in SDK retry due to heavy traffic). Falling back to next model...",
                        modelId
                    );
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    logger.LogWarning(
                        "Client disconnected / canceled the request while waiting for model {modelId}.",
                        modelId
                    );
                    throw;
                }
                catch (Exception ex)
                    when (IsTransientCapacityError(ex) && !cancellationToken.IsCancellationRequested
                    )
                {
                    lastException = ex;
                    logger.LogWarning(
                        ex,
                        "Model {modelId} is experiencing heavy traffic or rate limits. Falling back to next model...",
                        modelId
                    );
                }
            }

            throw new InvalidOperationException(
                $"All configured Gemini models ({string.Join(", ", modelsToTry)}) failed due to heavy traffic.",
                lastException
            );
        }

        private static bool IsTransientCapacityError(Exception ex)
        {
            string exText = ex.ToString();

            return exText.Contains("503", StringComparison.OrdinalIgnoreCase)
                || exText.Contains("429", StringComparison.OrdinalIgnoreCase)
                || exText.Contains("UNAVAILABLE", StringComparison.OrdinalIgnoreCase)
                || exText.Contains("RESOURCE_EXHAUSTED", StringComparison.OrdinalIgnoreCase)
                || exText.Contains("resource exhausted", StringComparison.OrdinalIgnoreCase)
                || exText.Contains("high demand", StringComparison.OrdinalIgnoreCase)
                || exText.Contains("heavy traffic", StringComparison.OrdinalIgnoreCase)
                || exText.Contains("overloaded", StringComparison.OrdinalIgnoreCase)
                || exText.Contains("too many requests", StringComparison.OrdinalIgnoreCase)
                || exText.Contains("rate limit", StringComparison.OrdinalIgnoreCase)
                || exText.Contains("rate-limit", StringComparison.OrdinalIgnoreCase)
                || exText.Contains("rate_limit", StringComparison.OrdinalIgnoreCase)
                || exText.Contains("quota", StringComparison.OrdinalIgnoreCase)
                || exText.Contains("temporarily unavailable", StringComparison.OrdinalIgnoreCase)
                || exText.Contains("capacity", StringComparison.OrdinalIgnoreCase);
        }
    }
}
