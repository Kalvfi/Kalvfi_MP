using System.ComponentModel.DataAnnotations;
using WorksheetGen.Api.Models;

namespace WorksheetGen.Api.Dtos.Worksheets
{
    public class QuestionRequest
    {
        public QuestionType Type { get; set; }

        [Required, StringLength(2000, MinimumLength = 1)]
        public string Prompt { get; set; } = string.Empty;

        public List<string> Options { get; set; } = [];
        public string? CorrectAnswer { get; set; }

        [Range(0, 1000)]
        public int Points { get; set; } = 1;

        public QuestionRequest(
            string Prompt,
            QuestionType Type,
            List<string> Options,
            string? CorrectAnswer,
            int Points
        )
        {
            this.Prompt = Prompt;
            this.Type = Type;
            this.Options = Options;
            this.CorrectAnswer = CorrectAnswer;
            this.Points = Points;
        }
    }
}
