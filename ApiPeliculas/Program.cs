using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.PeliculasMapper;
using ApiPeliculas.Repositorio;
using ApiPeliculas.Repositorio.IRepositorio;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(opciones =>
    opciones.UseSqlServer(builder.Configuration.GetConnectionString("ConexionSql"))
);
// Error 500 schema QUITAR SI ES NECESARIO
builder.Services.AddSwaggerGen(c =>
{
    c.CustomSchemaIds(type => type.FullName); // Usa el nombre completo con namespace
});
// Soporte para identity AUTH
builder.Services.AddIdentity<AppUsuario, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();


// Soporte para cache
builder.Services.AddResponseCaching();

// Versionamiento config 
var apiVersionBuilder = builder.Services.AddApiVersioning(opcion =>
{
    // Asumimos que por default se usa la version 1.0 Si no se especifica
    opcion.AssumeDefaultVersionWhenUnspecified = true;
    opcion.DefaultApiVersion = new ApiVersion(1, 0);
    opcion.ReportApiVersions = true;

    // Dejamos de pedir la version en string
    //opcion.ApiVersionReader = ApiVersionReader.Combine( 
    //    new QueryStringApiVersionReader("api-version")
    //);
});

apiVersionBuilder.AddApiExplorer(
    opciones =>
    {
        opciones.GroupNameFormat = "'v'VVV";
        opciones.SubstituteApiVersionInUrl = true; // Para pasarle por la url la version que queremos usar
    }
    );

// Agregamos los repositorios
builder.Services.AddScoped<ICategoriaRepositorio, CategoriaRepositorio>();
builder.Services.AddScoped<IPeliculaRepositorio, PeliculaRepositorio>();
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();

var key = builder.Configuration.GetValue<string>("ApiSettings:Secreta"); // KEY

// Soporte para versionamiento de la api
builder.Services.AddApiVersioning();


//Agregamos el automapper
builder.Services.AddAutoMapper(typeof(PeliculasMapper));

// Configuramos la autenticacion
builder.Services.AddAuthentication
    (
        x => 
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }
    ).AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
            ValidateIssuer = false,
            ValidateAudience = false
        };

    }
    );

builder.Services.AddControllers(opcion =>
{
    // Cache profile, para reutilizarlo y notener que poner configuracion en todas partes
    opcion.CacheProfiles.Add("PorDefecto20Segundos", new CacheProfile() { Duration = 20 });
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
            "Autenticación JWT usando el esquema Bearer. \r\n\r\n" +
            "Ingresa la palabra 'Bearer' seguido de un [espacio] y después su token en el campo de abajo. \r\n\r\n" +
            "Ejemplo: \"Bearer tkdauisdd1\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
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
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });

    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1.0",
        Title = "Peliculas Api V1",
        Description = "Api de Peliculas",
        TermsOfService = new Uri("https://maxweb.com"),
        Contact = new OpenApiContact
        {
            Name = "MaximoDev",
            Url = new Uri("https://maxweb.com")
        },
        License = new OpenApiLicense
        {
            Name = "Licencia personal",
            Url = new Uri("https://maxweb.com")
        }
    });
    // Config version 2
    options.SwaggerDoc("v2", new OpenApiInfo
    {
        Version = "v2.0",
        Title = "Peliculas Api V2",
        Description = "Api de Peliculas",
        TermsOfService = new Uri("https://maxweb.com"),
        Contact = new OpenApiContact
        {
            Name = "MaximoDev",
            Url = new Uri("https://maxweb.com")
        },
        License = new OpenApiLicense
        {
            Name = "Licencia personal",
            Url = new Uri("https://maxweb.com")
        }
    });
});


//Soporte para CORS
// Se pueden habilitar un dominio o multiples dominios
// Cualquier dominio
// usuamos el ejemplo de dominio localhost:3223
// Usamos * para permitir todos
builder.Services.AddCors(p => p.AddPolicy("PoliticaCors", build =>
{
    //build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
    build.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(opciones =>
    {
        // Definimos las versiones aqui
        opciones.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiPeliculasV1");
        opciones.SwaggerEndpoint("/swagger/v2/swagger.json", "ApiPeliculasV2");
    });
}

// Soporte para imagenes (Archivos estaticos)
app.UseStaticFiles();
app.UseHttpsRedirection();
// Soporte para cors
app.UseCors("PoliticaCors");

app.UseAuthentication(); // Para proteger los metodos
app.UseAuthorization();

app.MapControllers();

app.Run();
