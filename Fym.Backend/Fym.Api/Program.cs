using Fym.Api.Middlewares;
using Microsoft.EntityFrameworkCore;
using Fym.Api.Data;
using Fym.Api.Services;
using Fym.Api.Models; 

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// En Program.cs
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// 1. CONFIGURACIÓN DE CORS PARA VITE + VUE 3
builder.Services.AddCors(options =>
{
    options.AddPolicy("VueClientPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // El puerto por defecto de Vite
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Fym API", Version = "v1" });

    // Configuración ultra-estándar para JWT en Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Autenticación JWT usando el esquema Bearer. Ejemplo: '12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer" // Al poner Type = Http y Scheme = Bearer, Swagger añade la palabra 'Bearer ' automáticamente
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

// ==========================================================
// REGISTRO DE NUESTRO DBCONTEXT (INYECCIÓN DE DEPENDENCIAS)
// ==========================================================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddScoped<IAuthService, AuthService>();

// 🔐 CONFIGURACIÓN DE AUTENTICACIÓN JWT
var secretKey = builder.Configuration["JwtSettings:Secret"] 
    ?? throw new InvalidOperationException("La clave secreta de JWT no está configurada.");

// builder.Services.AddAuthentication(options =>
// {
//     // Le decimos a .NET que use JWT por defecto para validar y retar las peticiones
//     options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//     options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
// })
// .AddJwtBearer(options =>
// {
//     options.RequireHttpsMetadata = false; // Cambiar a true en producción si usas HTTPS obligatorio
//     options.SaveToken = true;
//     options.TokenValidationParameters = new TokenValidationParameters
//     {
//         ValidateIssuerSigningKey = true,
//         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
//         ValidateIssuer = true,
//         ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
//         ValidateAudience = true,
//         ValidAudience = builder.Configuration["JwtSettings:Audience"],
//         ValidateLifetime = true,
//         ClockSkew = TimeSpan.Zero // Elimina el tiempo de gracia de 5 minutos por defecto
//     };
// });

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    // --- AÑADE ESTO PARA MANEJAR LOS ERRORES 401 Y 403 ---
    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            // Se activa cuando el token falta, es inválido o expiró (401)
            context.HandleResponse();
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync("{\"message\": \"No autorizado: Token inválido, expirado o faltante.\"}");
        },
        OnForbidden = context =>
        {
            // Se activa cuando el usuario está logueado pero no tiene el rol necesario (403)
            context.Response.StatusCode = 403;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync("{\"message\": \"Prohibido: No tienes los permisos necesarios para realizar esta acción.\"}");
        }
    };
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fym API v1");
    });
}

// ¡Crucial que UseCors vaya antes de la autenticación!
app.UseCors("VueClientPolicy");

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Activar el middleware de manejo de excepciones
app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
