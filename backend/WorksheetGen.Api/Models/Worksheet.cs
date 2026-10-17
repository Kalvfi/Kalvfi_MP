namespace WorksheetGen.Api.Models
{
    internal class Worksheet
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public string Title { get; set; } = "";
        public string? Subject { get; set; }
        public string? GradeLevel { get; set; }
        public bool IsPublic { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public List<Question> Questions { get; set; } = [];

        public Guid OwnerId { get; set; }
        public User Owner { get; set; } = null!;
    }

    public enum QuestionType
    {
        MultipleChoice,
        TrueFalse,
        ShortAnswer,
        FillInTheBlank,
        Matching,
        Open,
    }

    public class Question
    {
        public int Order { get; set; }
        public QuestionType Type { get; set; }
        public string Text { get; set; } = "";
        public List<string> Options { get; set; } = [];
        public string? Answer { get; set; }
        public int Points { get; set; } = 1;
    }
}
