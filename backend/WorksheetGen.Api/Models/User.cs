namespace WorksheetGen.Api.Models
{
    internal class User
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public string Email { get; set; } = "";
        public string? DisplayName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
