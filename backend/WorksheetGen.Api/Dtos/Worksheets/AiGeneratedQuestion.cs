using System.ComponentModel;
using WorksheetGen.Api.Models;

namespace WorksheetGen.Api.Dtos.Worksheets
{
    public record AiGeneratedQuestion(
        [property: Description(
            "Question type. Must be one of: MultipleChoice, TrueFalse, ShortAnswer, FillInTheBlank, Matching, Open"
        )]
            QuestionType Type,
        [property: Description("The question prompt shown to the student")] string Prompt,
        [property: Description("Answer choices if MultipleChoice; empty list otherwise")]
            List<string> Options,
        [property: Description(
            "Correct answer or grading key if requested; null if IncludeAnswers is false"
        )]
            string? CorrectAnswer,
        [property: Description("Suggested point value for this question")] int Points
    );
}
