using System.Text;
using Users.Application.Users.Commands.RegisterUser;
using Users.Infrastructure;
using Users.Infrastructure.Persistence;
using Core.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddUsersModule(builder.Configuration);
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(RegisterUserCommand).Assembly));

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
  var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
  await db.Database.MigrateAsync();
}

app.UsePlatformErrorHandling();

if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
  app.MapScalarApiReference();
}

app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "users" }));

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
