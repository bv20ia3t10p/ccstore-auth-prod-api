using CcStore.Middleware;
using CcStore.Repository.Contracts;
using CcStore.Repository;
using CcStore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Configure services (original setup)
var mongoConnectionString = builder.Configuration.GetValue<string>("ConnectionStrings:MongoDB");
var mongoDatabaseName = builder.Configuration.GetValue<string>("MongoDbSettings:DatabaseName");

builder.Services.AddControllers();
builder.Services.AddSingleton<IMongoClient>(sp => new MongoClient(mongoConnectionString));
builder.Services.AddScoped<IMongoDatabase>(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(mongoDatabaseName));
builder.Services.AddSingleton<MongoDbContext>(sp => new MongoDbContext(mongoConnectionString, mongoDatabaseName));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<ProductRepository>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "C2S-UP-Api",
        Version = "v1",
        Description = "API Documentation with JWT Authentication",
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by a space and your JWT token.",
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });

    options.OperationFilter<FileUploadOperationFilter>();
});

var app = builder.Build();

// Enable Swagger for the `swagger tofile` command
if (args.Contains("swagger"))
{
    // Start application for Swagger CLI
    app.UseSwagger();
    app.MapControllers();
}
else
{
    // Original app pipeline
    app.UseCors("AllowAnyOrigin");
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "C2S-UP-Api v1"));
    }

    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
}

app.Run();
