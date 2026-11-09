using System.ComponentModel.DataAnnotations;

namespace WorksheetGen.Api.Options;

public class GeminiOptions
{
    public const string SectionName = "Gemini";

    [Required]
    public string ApiKey { get; set; } = string.Empty;

    [Required]
    public string Model { get; set; } = "gemini-3.8-flash";

    public List<string> FallbackModels { get; set; } =
    ["gemini-3.6-flash", "gemini-3.5-flash-lite"];
}
