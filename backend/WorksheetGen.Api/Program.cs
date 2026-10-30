using System.Text.Json.Serialization;
using Google.GenAI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using WorksheetGen.Api.Data;
using WorksheetGen.Api.Options;
using WorksheetGen.Api.Services;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder
            .Services.AddControllers()
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())
            );

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection"))
        );

        builder
            .Services.AddOptions<GeminiOptions>()
            .BindConfiguration(GeminiOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddSingleton<Client>(sp =>
        {
            var geminiOptions = sp.GetRequiredService<IOptions<GeminiOptions>>().Value;

            return new Client(apiKey: geminiOptions.ApiKey);
        });

        builder.Services.AddChatClient(sp =>
        {
            var client = sp.GetRequiredService<Client>();
            var geminiOptions = sp.GetRequiredService<IOptions<GeminiOptions>>().Value;
            return client.AsIChatClient(geminiOptions.Model);
        });

        builder.Services.AddScoped<ICurrentUser, DevCurrentUser>();
        builder.Services.AddScoped<IWorksheetGeneratorService, GeminiWorksheetGeneratorService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
