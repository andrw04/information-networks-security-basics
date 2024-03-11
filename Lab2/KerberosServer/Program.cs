using KerberosServer.Services;
using System.Text;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<KeyManagementService>();
builder.Services.AddSingleton<SessionService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var app = builder.Build();

var keyManagementService = app.Services.GetService<KeyManagementService>()!;
keyManagementService.Keys.Add("tgs", Encoding.UTF8.GetBytes(KeyManagementService.GenerateKey(8)));
keyManagementService.Keys.Add("testserver", Encoding.UTF8.GetBytes(KeyManagementService.GenerateKey(8)));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
