using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorksheetGen.Api.Data;
using WorksheetGen.Api.Dtos.Worksheets;
using WorksheetGen.Api.Mappings;
using WorksheetGen.Api.Services;

namespace WorksheetGen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorksheetsController(AppDbContext db, ICurrentUser currentUser) : Controller
    {
        [HttpGet]
        public async Task<ActionResult<List<WorksheetSummaryResponse>>> GetMine(
            CancellationToken ct
        )
        {
            var items = await db
                .Worksheets.Where(w => w.OwnerId == currentUser.Id)
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

        [HttpPost("generate")]
        public async Task<ActionResult<AiGeneratedWorksheet>> Generate(
            [FromBody] GenerateWorksheetRequest request,
            [FromServices] IWorksheetGeneratorService generatorService,
            CancellationToken ct
        )
        {
            string? sourceText = null;

            if (request.SourceDocumentId.HasValue)
            {
                var sourceDoc = await db.SourceDocuments.FirstOrDefaultAsync(
                    d => d.Id == request.SourceDocumentId.Value && d.OwnerId == currentUser.Id,
                    ct
                );

                if (sourceDoc is null)
                    return NotFound("Source document not found.");

                sourceText = sourceDoc.ExtractedText;
            }

            AiGeneratedWorksheet aiResult = await generatorService.GenerateWorksheetAsync(
                request,
                sourceText,
                ct
            );

            var createRequest = new CreateWorksheetRequest(
                Title: aiResult.Title,
                Subject: aiResult.Subject,
                Topic: aiResult.Topic,
                GradeLevel: aiResult.GradeLevel,
                Questions: aiResult
                    .Questions.Select(
                        (q, index) =>
                            new QuestionRequest(
                                Prompt: q.Prompt,
                                Type: q.Type,
                                Options: q.Options,
                                CorrectAnswer: q.CorrectAnswer,
                                Points: q.Points
                            )
                    )
                    .ToList()
            );

            var worksheet = createRequest.ToEntity(currentUser.Id);
            db.Worksheets.Add(worksheet);
            await db.SaveChangesAsync(ct);

            return CreatedAtAction(
                nameof(GetById),
                new { id = worksheet.Id },
                worksheet.ToResponse()
            );
        }
    }
}
