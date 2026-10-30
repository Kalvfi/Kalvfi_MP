using WorksheetGen.Api.Models;

namespace WorksheetGen.Api.Dtos.Worksheets
{
    public record GenerateWorksheetRequest(
        string Subject,
        string Topic,
        string GradeLevel,
        Guid? SourceDocumentId,
        string? RawSourceText,
        int QuestionCount,
        List<QuestionType> QuestionTypes,
        bool IncludeAnswers,
        bool IncludePoints,
        string? AdditionalNotes
    );
}
