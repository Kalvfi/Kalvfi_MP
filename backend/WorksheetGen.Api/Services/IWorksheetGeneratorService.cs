using WorksheetGen.Api.Dtos.Worksheets;

namespace WorksheetGen.Api.Services
{
    public interface IWorksheetGeneratorService
    {
        Task<AiGeneratedWorksheet> GenerateWorksheetAsync(
            GenerateWorksheetRequest request,
            string? extractedSourceText,
            CancellationToken cancellationToken = default
        );
    }
}
