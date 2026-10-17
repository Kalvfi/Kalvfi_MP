namespace WorksheetGen.Api.Models
{
    internal class Worksheet
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}