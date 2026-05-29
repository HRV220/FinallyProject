using System.Text;
using Categories.Application.Categories.Commands.CreateCategory;
using Categories.Infrastructure;
using Categories.Infrastructure.Persistence;
using Categories.Infrastructure.Persistence.Seed;
using Core.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddCategoriesModule(builder.Configuration);
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(CreateCategoryCommand).Assembly));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
  options => options.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = builder.Configuration["Jwt:Issuer"],
    ValidAudience = builder.Configuration["Jwt:Audience"],
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!))
  });
builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
  var db = scope.ServiceProvider.GetRequiredService<CategoriesDbContext>();
  await db.Database.MigrateAsync();
  await CategoriesSeeder.SeedAsync(db);
}

app.UsePlatformErrorHandling();

if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
  app.MapScalarApiReference();
}

app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "categories" }));

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
