using WorksheetGen.Api.Models;

namespace WorksheetGen.Api.Dtos.Worksheets
{
    public record QuestionResponse(
        int Order,
        QuestionType Type,
        string Prompt,
        List<string> Options,
        string? CorrectAnswer,
        int Points
    );
}
