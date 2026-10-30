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
                        q.Prompt,
                        q.Options,
                        q.CorrectAnswer,
                        q.Points
                    ))
                    .ToList()
            );

        public static Worksheet ToEntity(this CreateWorksheetRequest request, Guid ownerId)
        {
            var now = DateTime.UtcNow;
            return new Worksheet
            {
                OwnerId = ownerId,
                Title = request.Title.Trim(),
                Subject = request.Subject?.Trim(),
                GradeLevel = request.GradeLevel?.Trim(),
                IsPublic = request.IsPublic,
                CreatedAt = now,
                UpdatedAt = now,
                Questions = request
                    .Questions.Select(
                        (q, i) =>
                            new Question
                            {
                                Order = i + 1,
                                Type = q.Type,
                                Prompt = q.Prompt.Trim(),
                                Options = q.Options,
                                CorrectAnswer = q.CorrectAnswer,
                                Points = q.Points,
                            }
                    )
                    .ToList(),
            };
        }
    }
}
