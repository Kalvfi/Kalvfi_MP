using WorksheetGen.Api.Models;

namespace WorksheetGen.Api.Dtos.Worksheets
{
    public record QuestionResponse(
        int Order,
        QuestionType Type,
        string Text,
        List<string> Options,
        string? Answer,
        int Points
    );
}
