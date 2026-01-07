using Microsoft.EntityFrameworkCore;
using PFG_BackEnd;
using System.Text;
using PFG_BackEnd.Helper;
using PFG_BackEnd.Service.Extern;
using PFG_BackEnd.Service;

using Serilog;
using Serilog.Filters;


var builder = WebApplication.CreateBuilder(args);

var bdLogDir = @"C:\temp\loggersBD";
var appLogDir = @"C:\temp\loggersProduccio";

Directory.CreateDirectory(bdLogDir);
Directory.CreateDirectory(appLogDir);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()

    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(Matching.FromSource("PFG_BackEnd.Service.Extern"))
        .WriteTo.File(Path.Combine(bdLogDir, "bd-.log"),
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: null))

    .WriteTo.Logger(lc => lc
        .Filter.ByExcluding(Matching.FromSource("PFG_BackEnd.Service.Backup"))
        .WriteTo.File(Path.Combine(appLogDir, "app-.log"),
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: null))
    
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString")));

builder.Services.AddScoped<CashDrawerService>();
builder.Services.AddScoped<ComandaService>();
builder.Services.AddScoped<ComandaPagadaService>();
builder.Services.AddScoped<TaulaService>();
builder.Services.AddScoped<ProducteService>();
builder.Services.AddScoped<TipusProducteService>();
builder.Services.AddScoped<EstadistiquesService>();
builder.Services.AddScoped<CaixaDiariaService>();
builder.Services.AddScoped<MenuService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<DbBackupOptions>(opt =>
{
    opt.ConnectionString =  builder.Configuration.GetConnectionString("ConnectionString")!;
    builder.Configuration.GetSection("DbBackup").Bind(opt);
});

builder.Services.AddSingleton<DbBackupService>();
builder.Services.AddHostedService<BackupSchedulerWorker>();

var app = builder.Build();

app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngular");

app.UseHttpsRedirection();

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);


app.MapControllers();

app.MapGet("/api/health", () => Results.Ok("OK"));

app.Run();