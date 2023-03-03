using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using Test.WebAPI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";//-------------------1.�޸ķ���ʱ���ʽ
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();



//-----------------------------------------------------------------------------------------------2.����swaggerע��
builder.Services.AddSwaggerGen(options =>
{
    var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName), true);//�ڶ�������true��ʾע���ļ������˿�������ע��

    //-----------------------------------------------------------------------------------------------3.��Ӱ汾����
    typeof(APIVersion).GetEnumNames().ToList().ForEach(version =>
    {
        options.SwaggerDoc(version, new OpenApiInfo
        {
            Title = $"�汾��{version}",
            Version = version,
            Description = $"���ǰ汾�ţ�{version}"
        });
    });

    //AddSecurityDefinition��������һ����ȫ��֤��ע�⣬ֻ����������δָ���ӿڱ���Ҫʹ����֤
    //OpenApiSecurityScheme���������������֤��Ϣ
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
    });
    //AddSecurityRequirement�ǽ�������������֤���������нӿ�
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
});


//-----------------------------------------------------------------------------------------------4.����pg���ݿ�
builder.Services.AddDbContext<Test.WebAPI.DbContext>(builder => { builder.UseNpgsql("Host=localhost;Port=5432;Username=postgres;Password=123456;Database=test_01;"); });

//-----------------------------------------------------------------------------------------------5.����Jwt
var jwtSeetings = new JwtSeetings();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = jwtSeetings.Issuer,
            ValidAudience = jwtSeetings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSeetings.SecretKey))
        };
    });







var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        typeof(APIVersion).GetEnumNames().ToList().ForEach(version =>
        {
            options.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"�汾ѡ��{version}");
        });
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();// 5.
app.UseAuthorization();
app.MapControllers();

app.Run();
