using System.ComponentModel.DataAnnotations;

namespace WorksheetGen.Api.Options;

public class GeminiOptions
{
    public const string SectionName = "Gemini";

    [Required]
    public string ApiKey { get; set; } = string.Empty;

    [Required]
    public string Model { get; set; } = string.Empty;
}
