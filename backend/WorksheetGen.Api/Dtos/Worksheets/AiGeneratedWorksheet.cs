using System.ComponentModel;

namespace WorksheetGen.Api.Dtos.Worksheets
{
    public record AiGeneratedWorksheet(
        [property: Description("Title of the activity sheet")] string Title,
        [property: Description("Subject of the activity sheet")] string Subject,
        [property: Description("Topic of the activity sheet")] string Topic,
        [property: Description("Grade level of the activity sheet")] string GradeLevel,
        [property: Description("List of generated questions based on the source material")]
            List<AiGeneratedQuestion> Questions
    );
}
