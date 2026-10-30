using Microsoft.Extensions.AI;
using WorksheetGen.Api.Dtos.Worksheets;

namespace WorksheetGen.Api.Services
{
    public class GeminiWorksheetGeneratorService(IChatClient chatClient)
        : IWorksheetGeneratorService
    {
        public async Task<AiGeneratedWorksheet> GenerateWorksheetAsync(
            GenerateWorksheetRequest request,
            string? extractedSourceText,
            CancellationToken cancellationToken = default
        )
        {
            var systemPrompt = """
                You are an expert teacher's assistant that creates structured school activity sheets.
                Generate clear, pedagogically sound questions strictly grounded in the provided source material and parameters.
                Respond in the language of the source material or topic (default to Czech if unspecified).
                """;

            var userPrompt = $"""
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
                """;

            List<ChatMessage> messages =
            [
                new(ChatRole.System, systemPrompt),
                new(ChatRole.User, userPrompt),
            ];

            var options = new ChatOptions { Temperature = 0.4f };

            ChatResponse<AiGeneratedWorksheet> response =
                await chatClient.GetResponseAsync<AiGeneratedWorksheet>(
                    messages,
                    options,
                    useJsonSchemaResponseFormat: true,
                    cancellationToken: cancellationToken
                );

            return response.Result
                ?? throw new InvalidOperationException(
                    "Gemini returned an empty worksheet response."
                );
        }
    }
}
