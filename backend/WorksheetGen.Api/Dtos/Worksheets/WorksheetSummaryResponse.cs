namespace WorksheetGen.Api.Dtos.Worksheets
{
    public record WorksheetSummaryResponse(
        Guid Id,
        string Title,
        string? Subject,
        string? GradeLevel,
        bool IsPublic,
        DateTime CreatedAt
    );
}
