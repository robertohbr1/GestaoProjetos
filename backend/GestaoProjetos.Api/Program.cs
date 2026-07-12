using System.Security.Claims;
using System.Text;
using GestaoProjetos.Api.Application.Services;
using GestaoProjetos.Api.Domain.Entities;
using GestaoProjetos.Api.Domain.Enums;
using GestaoProjetos.Api.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.AspNetCore.OpenApi;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure OpenAPI document generation
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

// Configure EF Core with SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularDevPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Configure JWT Authentication
var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "SuperSecretKeyForProjectManagementApp2026!!!";
var key = Encoding.UTF8.GetBytes(jwtSecret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "GestaoProjetosIssuer",
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "GestaoProjetosAudience",
        IssuerSigningKey = new SymmetricSecurityKey(key),
        NameClaimType = ClaimTypes.Name,
        RoleClaimType = ClaimTypes.Role
    };
});

builder.Services.AddAuthorization();

// Register Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IIssueService, IssueService>();
builder.Services.AddScoped<ITimeLogService, TimeLogService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IAttachmentService, AttachmentService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuditService, AuditService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

// Serve static files from wwwroot (for attachments)
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AngularDevPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Auto-migration & Database Seeding
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate();

        // Seed Users
        if (!context.Users.Any())
        {
            var adminUser = new User
            {
                Username = "admin",
                Email = "admin@gestaoprojetos.local",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Role = UserRole.Administrator
            };
            var devUser = new User
            {
                Username = "dev1",
                Email = "dev1@gestaoprojetos.local",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("dev123"),
                Role = UserRole.Developer
            };
            var userCollab = new User
            {
                Username = "collab1",
                Email = "collab1@gestaoprojetos.local",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("collab123"),
                Role = UserRole.Collaborator
            };

            context.Users.AddRange(adminUser, devUser, userCollab);
            context.SaveChanges();
        }

        // Seed Projects
        if (!context.Projects.Any())
        {
            var project1 = new Project
            {
                Name = "Portal da TI",
                Description = "Desenvolvimento do novo portal de serviços de TI interna.",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            var project2 = new Project
            {
                Name = "Integração ERP",
                Description = "Criação de fluxos SSIS e C# para integração de dados.",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            context.Projects.AddRange(project1, project2);
            context.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro durante a inicialização do banco de dados.");
    }
}

app.Run();

public sealed class BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider) 
    : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
        
        if (authenticationSchemes != null && authenticationSchemes.Any(authScheme => authScheme.Name == "Bearer"))
        {
            var securityScheme = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header
            };

            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
            document.Components.SecuritySchemes["Bearer"] = securityScheme;

            if (document.Paths != null)
            {
                foreach (var path in document.Paths.Values)
                {
                    if (path.Operations != null)
                    {
                        foreach (var operation in path.Operations.Values)
                        {
                            if (operation != null)
                            {
                                operation.Security ??= new List<OpenApiSecurityRequirement>();
                                operation.Security.Add(new OpenApiSecurityRequirement
                                {
                                    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                                });
                            }
                        }
                    }
                }
            }
        }
    }
}
