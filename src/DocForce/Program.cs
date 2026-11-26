using DocForce.Application.Extensions;
using DocForce.Infrastructure.Extensions;
using DocForce.Infrastructure.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddApplication();
builder.Services.AddInfrastructure(configuration);


#region swagger

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "DocForce.API", Version = "V1" });

    opt.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
    {
        Description = "Input JWT Token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    opt.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [
            new OpenApiSecuritySchemeReference("bearer", document)
        ] = []
    });
});

#endregion


#region asp.net core region

builder.Services.AddControllers();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    var jwtOption = configuration.GetSection(JwtOption.Section).Get<JwtOption>()!;

    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidIssuer = jwtOption.Issuer,
        ValidateIssuer = true,
        ValidAudience = jwtOption.Audience,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtOption.Key)),
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddAuthorization();

#endregion

var app = builder.Build();

app.UseHsts();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
