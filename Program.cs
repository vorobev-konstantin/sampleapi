using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using sampleapi;
using sampleapi.DbContexts;
using sampleapi.Services;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/sampleapi.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();


var builder = WebApplication.CreateBuilder(args);
//builder.Logging.ClearProviders();
//builder.Logging.AddConsole();
builder.Host.UseSerilog();
// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;
}).AddNewtonsoftJson();

builder.Services.AddProblemDetails( options =>
{
    options.CustomizeProblemDetails = ctx =>
    {
        ctx.ProblemDetails.Extensions.Add("server", Environment.MachineName);
    };
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<FileExtensionContentTypeProvider>();
builder.Services.AddDbContext<sampleapiContext>(dbContextOption 
    => dbContextOption.UseNpgsql(builder.Configuration["ConnectionStrings:sampleapiDBConnectionString"]));

builder.Services.AddScoped<ISampleApiRepository, SampleApiRepository>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

#if DEBUG
builder.Services.AddTransient<IMailService, LocalMailService>();
#else
builder.Services.AddTransient<IMailService, CloudMailService>();
#endif

var app = builder.Build();

// Configure the HTTP request pipeline.

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
}


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
