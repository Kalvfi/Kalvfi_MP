using System.ComponentModel.DataAnnotations;

namespace WorksheetGen.Api.Dtos.Worksheets
{
    public class CreateWorksheetRequest
    {
        [Required, StringLength(200, MinimumLength = 1)]
        public string Title { get; set; } = "";

        [StringLength(100)]
        public string? Subject { get; set; }

        [StringLength(50)]
        public string? GradeLevel { get; set; }
        public bool IsPublic { get; set; }

        [MaxLength(100)]
        public List<QuestionRequest> Questions { get; set; } = [];
    }
}
