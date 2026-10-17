using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorksheetGen.Api.Data;
using WorksheetGen.Api.Dtos.Worksheets;
using WorksheetGen.Api.Mappings;
using WorksheetGen.Api.Models;

namespace WorksheetGen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorksheetsController(AppDbContext db, ICurrentUser user) : Controller
    {
        [HttpGet]
        public async Task<ActionResult<List<WorksheetSummaryResponse>>> GetMine(
            CancellationToken ct
        )
        {
            var items = await db
                .Worksheets.Where(w => w.OwnerId == user.Id)
                .OrderByDescending(w => w.CreatedAt)
                .Select(w => new WorksheetSummaryResponse(
                    w.Id,
                    w.Title,
                    w.Subject,
                    w.GradeLevel,
                    w.IsPublic,
                    w.CreatedAt
                ))
                .ToListAsync(ct);

            return items;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<WorksheetResponse>> GetById(Guid id, CancellationToken ct)
        {
            var worksheet = await db.Worksheets.AsNoTracking().FirstOrDefaultAsync(ct);

            if (worksheet is null)
                return NotFound();

            return WorksheetMappings.ToResponse(worksheet);
        }

        [HttpPost]
        public async Task<ActionResult<WorksheetResponse>> Create(
            CreateWorksheetRequest request,
            CancellationToken ct
        )
        {
            var now = DateTime.UtcNow;
            var worksheet = new Worksheet
            {
                OwnerId = user.Id,
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
                                Text = q.Text.Trim(),
                                Options = q.Options,
                                Answer = q.Answer,
                                Points = q.Points,
                            }
                    )
                    .ToList(),
            };

            db.Worksheets.Add(worksheet);
            await db.SaveChangesAsync(ct);

            return CreatedAtAction(
                nameof(GetById),
                new { id = worksheet.Id },
                WorksheetMappings.ToResponse(worksheet)
            );
        }
    }
}
