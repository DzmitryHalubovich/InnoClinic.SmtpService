using SmtpAPI.Configuration;
using SmtpAPI.DocumentHttpClient;
using SmtpAPI.EmailService;
using SmtpAPI.Extentions;
using SmtpAPI.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton<RabbitListener>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<DocumentsAPIHttpClient>();

var emailConfig = builder.Configuration
    .GetSection("EmailConfiguration")
    .Get<EmailConfiguration>();

builder.Services.AddTransient<IEmailService, EmailService>();

builder.Services.AddSingleton(emailConfig);

var app = builder.Build();

app.UseRabbitListener();

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
