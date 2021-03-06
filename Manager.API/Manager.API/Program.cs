using AutoMapper;
using EscNet.IoC.Cryptography;
using EscNet.IoC.Hashers;
using Isopoh.Cryptography.Argon2;
using Manager.API.Token;
using Manager.API.ViewModels;
using Manager.Domain.Entities;
using Manager.Infra.Context;
using Manager.Infra.Interfaces;
using Manager.Infra.Repositories;
using Manager.Services.DTO;
using Manager.Services.Interfaces;
using Manager.Services.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();



#region AzureKeyVault

builder.Configuration.AddAzureKeyVault(
                            builder.Configuration["AzureKeyVault:Vault"],
                            builder.Configuration["AzureKeyVault:ClientId"],
                            builder.Configuration["AzureKeyVault:ClientSecret"]);

#endregion

#region Swagger-Auth 

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Manager API",
        Version = "v1",
        Description = "API constru?da na seria de v?deos no canal Lucas Eschechola",
        Contact = new OpenApiContact
        {
            Name = "Ang?lica Fran?a",
            Email = "gefranca94@gmail.com",
            Url = new Uri("https://github.com/angelicafranca94")

        }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Favor utilizar Bearer <TOKEN>",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
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
            new string[] { }
        }
    });
});

#endregion

#region JWT

var secretKey = builder.Configuration["Jwt:Key"];

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

#endregion

#region AutoMapper

var autoMapperConfig = new MapperConfiguration(config =>
{
    config.CreateMap<User, UserDTO>().ReverseMap();
    config.CreateMap<CreateUserViewModel, UserDTO>().ReverseMap();
    config.CreateMap<UpdateUserViewModel, UserDTO>().ReverseMap();

});

builder.Services.AddSingleton(autoMapperConfig.CreateMapper());

#endregion

#region Dependence Injection

//Scoped - Adiciona uma inst?ncia ?nica por requisi??o
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITokenGenerator, TokenGenerator>();

#endregion

#region Database

builder.Services.AddDbContext<ManagerContext>(options => options
    .UseSqlServer(builder.Configuration["SqlConnection:SqlConnectionString"])
    .EnableSensitiveDataLogging()
    .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole())),
ServiceLifetime.Transient);

#endregion

#region Cryp

//builder.Services.AddRijndaelCryptography(builder.Configuration["Cryptography"]);

#endregion

#region Hash

var config = new Argon2Config()
{
    Type = Argon2Type.DataIndependentAddressing,
    Version = Argon2Version.Nineteen,
    Threads = Environment.ProcessorCount,
    TimeCost = int.Parse(builder.Configuration["Hash:TimeCost"]),
    MemoryCost = int.Parse(builder.Configuration["Hash:MemoryCost"]),
    Lanes = int.Parse(builder.Configuration["Hash:Lanes"]),
    HashLength = int.Parse(builder.Configuration["Hash:HashLength"]),
    Salt = Encoding.UTF8.GetBytes(builder.Configuration["Hash:Salt"]),
};

builder.Services.AddArgon2IdHasher(config);

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
