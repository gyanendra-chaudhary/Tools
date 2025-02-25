using Microsoft.AspNetCore.Http.HttpResults;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/
builder.Services.AddOpenApi();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapGet("/", () =>
{
    if (app.Environment.IsDevelopment())
    {
        return Results.Redirect("/scalar");
    }

    return Results.Json(
        new
        {
            AppDetails = new
            {
                AppName = "esn-dictionary",
                AppVersion = "v1",
                PublishDate = "2-26-2025",
                Description = ""
            },
            DeveloperDetails = new
            {
                Name = "Gyanendra Chaudhary",
                Email = "dev.gyanendrapc@gmail.com"
            }
        });
});
app.MapControllers();

app.Run();