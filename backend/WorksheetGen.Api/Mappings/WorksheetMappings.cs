using WorksheetGen.Api.Dtos.Worksheets;
using WorksheetGen.Api.Models;

namespace WorksheetGen.Api.Mappings
{
    internal static class WorksheetMappings
    {
        public static WorksheetResponse ToResponse(this Worksheet w) =>
            new(
                w.Id,
                w.Title,
                w.Subject,
                w.GradeLevel,
                w.IsPublic,
                w.CreatedAt,
                w.UpdatedAt,
                w.Questions.OrderBy(q => q.Order)
                    .Select(q => new QuestionResponse(
                        q.Order,
                        q.Type,
                        q.Text,
                        q.Options,
                        q.Answer,
                        q.Points
                    ))
                    .ToList()
            );
    }
}
