using System.ComponentModel.DataAnnotations;
using WorksheetGen.Api.Models;

namespace WorksheetGen.Api.Dtos.Worksheets
{
    public class QuestionRequest
    {
        public QuestionType Type { get; set; }

        [Required, StringLength(2000, MinimumLength = 1)]
        public string Text { get; set; } = "";

        public List<string> Options { get; set; } = [];
        public string? Answer { get; set; }

        [Range(0, 1000)]
        public int Points { get; set; } = 1;
    }
}
