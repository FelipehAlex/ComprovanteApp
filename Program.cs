using System.Text.Json.Serialization;
using ComprovantesApp.Data;
using ComprovantesApp.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        "logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Server=(localdb)\\mssqllocaldb;Database=comprovantes_db;Trusted_Connection=True;";

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(connectionString)
    );

    builder.Services.AddScoped<IFornecedorService, FornecedorService>();
    builder.Services.AddScoped<IComprovanteService, ComprovanteService>();

    // Razor Pages: as telas que o usuário usa no navegador
    builder.Services.AddRazorPages();

    // Controllers: os endpoints de API (/api/fornecedores, /api/comprovantes),
    // documentados pelo Swagger. Usam os mesmos Services das Razor Pages.
    builder.Services
        .AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new()
        {
            Title = "Comprovantes Financeiros API",
            Version = "v1",
            Description = "API para controle de comprovantes financeiros enviados por hotéis e fornecedores, desde o recebimento até a integração simulada com o ERP."
        });
    });

    var app = builder.Build();

    // Aplica migrations pendentes e popula dados de exemplo na primeira execução
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();
        await DbInitializer.SeedAsync(context);
    }

    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Comprovantes Financeiros API v1");
        });
    }
    else
    {
        app.UseExceptionHandler("/Error");
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();
    app.UseAuthorization();

    app.MapControllers();
    app.MapRazorPages();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "A aplicação encerrou de forma inesperada");
}
finally
{
    Log.CloseAndFlush();
}
