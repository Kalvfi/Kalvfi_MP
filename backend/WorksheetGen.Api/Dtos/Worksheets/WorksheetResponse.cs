namespace WorksheetGen.Api.Dtos.Worksheets
{
    public record WorksheetResponse(
        Guid Id,
        string Title,
        string? Subject,
        string? GradeLevel,
        bool IsPublic,
        DateTime CreatedAt,
        DateTime UpdatedAt,
        List<QuestionResponse> Questions
    );
}
