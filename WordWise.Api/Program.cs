using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WordWise.Api.Data;
using WordWise.Api.Mapping;
using WordWise.Api.Models.Domain;
using WordWise.Api.Repositories.Implement;
using WordWise.Api.Repositories.Interface;
using WordWise.Api.Services.Implement;
using WordWise.Api.Services.Interface;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "WordWise.Api",
        Version = "v1"
    });
    /*options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                },
                Scheme = "oauth2",
                Name = JwtBearerDefaults.AuthenticationScheme,
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });*/
});

builder.Services.AddMemoryCache();
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddDbContext<WordWiseDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("WordWiseConnectionString"));
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
        });
});

// Register repositories
builder.Services.AddScoped<IFlashCardRepository, FlashCardRepository>();
builder.Services.AddScoped<IFlashcardSetRepository, FlashcardSetRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<IFlashcardReviewRepository, FlashcardReviewRepository>();
builder.Services.AddScoped<IWritingExerciseRepository, WritingExerciseRepository>();
builder.Services.AddScoped<IMultipleChoiceTestRepository, MultipleChoiceTestRepository>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<IUserLearningStatsRepository, UserLearningStatsRepository>();
builder.Services.AddScoped<IContentReportRepository, ContentReportRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

// Register services
builder.Services.AddSingleton<ICacheService, CacheService>();
builder.Services.AddScoped<IFlashcardSetService, FlashcardSetService>();
builder.Services.AddScoped<IWritingExerciseService, WritingExerciseService>();
builder.Services.AddScoped<IMultipleChoiceTestService, MultipleChoiceTestService>();
builder.Services.AddScoped<IUserLearningStatsService, UserLearningStatsService>();
builder.Services.AddScoped<IEmailService, EmailService>();


builder.Services.AddIdentityCore<ExtendedIdentityUser>()
    .AddRoles<IdentityRole>()
    .AddTokenProvider<DataProtectorTokenProvider<ExtendedIdentityUser>>("PhucDai")
    .AddEntityFrameworkStores<WordWiseDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
});

builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromMinutes(10);
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
