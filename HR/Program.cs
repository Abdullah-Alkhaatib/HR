using HR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = Encoding.UTF8.GetBytes("SKDJFSJKJDSJK!!2@@@#EJSJD872DKFSKDKK"); // Define the secret key
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false, // The source where the token is issued
            ValidateAudience = false, // The users whome can this token
            ValidateIssuerSigningKey = true, //Make sure that the token is using my secret key
            IssuerSigningKey = new SymmetricSecurityKey(key) // Generate the token using our key
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", builder =>
    {
        //builder.WithOrigins("http://localhost:4200") // Allow requests from this origin

        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

// Add DbContext
builder.Services.AddDbContext<HrDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("HrContext"))// HrContext => اللي ب appsettings.json
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngular"); // Enable CORS for Angular application

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
