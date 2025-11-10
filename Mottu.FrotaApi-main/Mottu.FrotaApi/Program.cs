using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Mottu.FrotaApi.Data;
using Mottu.FrotaApi.Models;
using Mottu.FrotaApi.ML;
using Microsoft.OpenApi.Any;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ======================================
// Health Checks
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection"));

// Razor Pages
builder.Services.AddRazorPages();

// Controllers e JSON
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling =
            Newtonsoft.Json.ReferenceLoopHandling.Ignore);

// Banco de Dados
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- ADICIONADO (ETAPA 7.2): Configuração do CORS ---
// Define uma "política" de permissão
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("*") // Permite QUALQUER origem
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});
// --- FIM DA ADIÇÃO ---

// Versionamento de API
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// Swagger / OpenAPI
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen();

// ... (O resto da sua configuração do Swagger/MapType/JWT continua aqui) ...
// (O código que você enviou está correto, não precisa mudar nada nele)
// ...
builder.Services.PostConfigure<SwaggerGenOptions>(options =>
{
    options.MapType<Moto>(() => new OpenApiSchema
    {
        Type = "object",
        Properties = new Dictionary<string, OpenApiSchema>
        {
            ["id"] = new OpenApiSchema { Type = "integer", Example = new OpenApiInteger(1) },
            ["placa"] = new OpenApiSchema { Type = "string", Example = new OpenApiString("ABC-1234") },
            ["modelo"] = new OpenApiSchema { Type = "string", Example = new OpenApiString("Honda CG 160") },
            ["status"] = new OpenApiSchema { Type = "string", Example = new OpenApiString("Disponível") },
            ["filialId"] = new OpenApiSchema { Type = "integer", Example = new OpenApiInteger(1) }
        },
        Required = new HashSet<string> { "placa", "modelo", "status", "filialId" }
    });
    options.MapType<Filial>(() => new OpenApiSchema
    {
        Type = "object",
        Properties = new Dictionary<string, OpenApiSchema>
        {
            ["id"] = new OpenApiSchema { Type = "integer", Example = new OpenApiInteger(1) },
            ["nome"] = new OpenApiSchema { Type = "string", Example = new OpenApiString("Filial Central") },
            ["endereco"] = new OpenApiSchema { Type = "string", Example = new OpenApiString("Rua das Flores, 123") }
        },
        Required = new HashSet<string> { "nome", "endereco" }
    });
    options.MapType<Manutencao>(() => new OpenApiSchema
    {
        Type = "object",
        Properties = new Dictionary<string, OpenApiSchema>
        {
            ["id"] = new OpenApiSchema { Type = "integer", Example = new OpenApiInteger(1) },
            ["descricao"] = new OpenApiSchema { Type = "string", Example = new OpenApiString("Troca de óleo") },
            ["data"] = new OpenApiSchema { Type = "string", Format = "date-time", Example = new OpenApiString(DateTime.UtcNow.ToString("o")) },
            ["motoId"] = new OpenApiSchema { Type = "integer", Example = new OpenApiInteger(1) },
            ["filialId"] = new OpenApiSchema { Type = "integer", Example = new OpenApiInteger(1) }
        },
        Required = new HashSet<string> { "descricao", "data", "motoId" }
    });
    options.MapType<DisponibilidadeInput>(() => new OpenApiSchema
    {
        Type = "object",
        Properties = new Dictionary<string, OpenApiSchema>
        {
            ["KmRodados"] = new OpenApiSchema { Type = "number", Format = "float", Example = new OpenApiFloat(12000f) },
            ["DiasDesdeUltimaEntrega"] = new OpenApiSchema { Type = "number", Format = "float", Example = new OpenApiFloat(30f) },
            ["EmManutencao"] = new OpenApiSchema { Type = "number", Format = "float", Example = new OpenApiFloat(0f) }
        },
        Required = new HashSet<string> { "KmRodados", "DiasDesdeUltimaEntrega", "EmManutencao" }
    });
    options.MapType<DisponibilidadePrediction>(() => new OpenApiSchema
    {
        Type = "object",
        Properties = new Dictionary<string, OpenApiSchema>
        {
            ["PredictedLabel"] = new OpenApiSchema { Type = "boolean", Example = new OpenApiBoolean(true) },
            ["Score"] = new OpenApiSchema { Type = "number", Format = "float", Example = new OpenApiFloat(0.92f) },
            ["Probability"] = new OpenApiSchema { Type = "number", Format = "float", Example = new OpenApiFloat(0.87f) }
        },
        Required = new HashSet<string> { "PredictedLabel", "Score", "Probability" }
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Insira bearer + token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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
                },
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
    options.OperationFilter<AuthorizeCheckOperationFilter>();
});
builder.Services.AddSingleton<Mottu.FrotaApi.Services.DisponibilidadeTrainer>();
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });
// ... (Fim da sua configuração de serviços) ...


var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var desc in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json",
                $"Mottu Frota API {desc.GroupName.ToUpperInvariant()}");
        }

        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

// --- ADICIONADO (ETAPA 7.2): Habilita o CORS ---
// Diz ao app para USAR a política de permissão que definimos
// (Deve vir antes de UseAuthorization)
app.UseCors(MyAllowSpecificOrigins);
// --- FIM DA ADIÇÃO ---

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/healthz");
app.MapRazorPages();
app.MapControllers();

app.Run();

// ======================================
// Classe parcial para testes funcionarem
public partial class Program { }

// ======================================
// Classes auxiliares do Swagger
public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;
    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => _provider = provider;
    public void Configure(SwaggerGenOptions options)
    {
        foreach (var desc in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(desc.GroupName, new OpenApiInfo
            {
                Title = "Mottu Frota API",
                Version = desc.ApiVersion.ToString(),
                Description = "API RESTful para gerenciamento de frota, motos, filiais e manutenções",
                Contact = new OpenApiContact { Name = "Equipe Mottu", Email = "suporte@mottu.com" },
                License = new OpenApiLicense { Name = "MIT License", Url = new Uri("https://opensource.org/licenses/MIT") }
            });
        }
    }
}

public class AuthorizeCheckOperationFilter : Swashbuckle.AspNetCore.SwaggerGen.IOperationFilter
{
    public void Apply(OpenApiOperation operation, Swashbuckle.AspNetCore.SwaggerGen.OperationFilterContext context)
    {
        if (operation.Security == null)
            operation.Security = new List<OpenApiSecurityRequirement>();

        operation.Security.Add(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                },
                new string[] {}
            }
        });
    }
}