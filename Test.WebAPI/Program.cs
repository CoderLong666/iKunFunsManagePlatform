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
        options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";//-------------------1.修改返回时间格式
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();



//-----------------------------------------------------------------------------------------------2.开启swagger注释
builder.Services.AddSwaggerGen(options =>
{
    var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName), true);//第二个参数true表示注释文件包含了控制器的注释

    //-----------------------------------------------------------------------------------------------3.添加版本控制
    typeof(APIVersion).GetEnumNames().ToList().ForEach(version =>
    {
        options.SwaggerDoc(version, new OpenApiInfo
        {
            Title = $"版本：{version}",
            Version = version,
            Description = $"这是版本号：{version}"
        });
    });

    //AddSecurityDefinition用于声明一个安全认证，注意，只是声明，并未指定接口必须要使用认证
    //OpenApiSecurityScheme对象就是描述的认证信息
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
    });
    //AddSecurityRequirement是将上面声明的认证作用于所有接口
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


//-----------------------------------------------------------------------------------------------4.链接pg数据库
builder.Services.AddDbContext<Test.WebAPI.DbContext>(builder => { builder.UseNpgsql("Host=localhost;Port=5432;Username=postgres;Password=123456;Database=test_01;"); });

//-----------------------------------------------------------------------------------------------5.配置Jwt
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
            options.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"版本选择：{version}");
        });
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();// 5.
app.UseAuthorization();
app.MapControllers();

app.Run();
