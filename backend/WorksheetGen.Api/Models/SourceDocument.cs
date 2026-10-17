namespace WorksheetGen.Api.Models
{
    internal class SourceDocument
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public string FileName { get; set; } = "";
        public string ContentType { get; set; } = "";
        public long SizeBytes { get; set; }
        public string ExtractedText { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid OwnerId { get; set; }
        public User Owner { get; set; } = null!;
    }
}
