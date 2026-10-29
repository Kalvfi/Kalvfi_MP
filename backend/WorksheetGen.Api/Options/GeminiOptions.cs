using System.ComponentModel.DataAnnotations;

namespace WorksheetGen.Api.Options;

public class GeminiOptions
{
    public const string SectionName = "Gemini";

    [Required]
    public string ApiKey { get; set; } = "";

    [Required]
    public string Model { get; set; } = "";

    [Range(5, 300)]
    public int TimeoutSeconds { get; set; } = 60;
}
