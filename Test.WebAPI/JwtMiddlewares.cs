using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace Test.WebAPI
{
    public static class JwtMiddlewares
    {
        public static IApplicationBuilder UseJwtAuthToken(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JwtAuthTokenMiddleware>();
        }
    }

    public class JwtAuthTokenMiddleware : IMiddleware
    {
        private readonly IJwtAuthManager _JwtAuthManager;
        private readonly JwtSeetings _jwtSeetings;

        public JwtAuthTokenMiddleware(IOptions<JwtSeetings> jwtSeetingsOptions, IJwtAuthManager jwtAuthManager)
        {
            _JwtAuthManager = jwtAuthManager;
            _jwtSeetings = jwtSeetingsOptions.Value;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            //if (_JwtAuthManager.IsCurrentRevokedToken())
            //{
            //    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            //    return;
            //}

            //await next.Invoke(context);
            return;
        }
    }

    public interface IJwtAuthManager
    {
        /// <summary>
        /// 生成令牌
        /// </summary>
        /// <param name="id">ID（每个ID任何时期都只能拥有最多一个有效的令牌）</param>
        /// <param name="claims"></param>
        /// <returns></returns>
        JwtTokenHelper Generate(string id, Claim[] claims);
    }

    public class JwtAuthManager : IJwtAuthManager
    {
        private readonly IJwtAuthManager _JwtAuthManager;
        private readonly JwtSeetings _jwtSeetings;

        public JwtAuthManager(IOptions<JwtSeetings> jwtSeetingsOptions, IJwtAuthManager jwtAuthManager)
        {
            _JwtAuthManager = jwtAuthManager;
            _jwtSeetings = jwtSeetingsOptions.Value;
        }

        public JwtTokenHelper Generate(string id, Claim[] claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSeetings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _jwtSeetings.Issuer,
                _jwtSeetings.Audience,
                claims,
                DateTime.Now,
                DateTime.Now.AddMinutes(30),
                creds
                );
            return new JwtTokenHelper { Id = id, Token = new JwtSecurityTokenHandler().WriteToken(token) };
        }
    }
    public class JwtTokenHelper
    {
        public string Id { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTime AccessExpireTime { get; set; }
        public DateTime RefreshExpireTime { get; set; }
        public bool Revoked { get; set; }
    }
    public class JwtSeetings
    {
        /// <summary>
        /// 谁颁发的jwt
        /// </summary>
        public string Issuer { get; set; } = "http://localhost:5000";

        /// <summary>
        /// 谁使用这个jwt
        /// </summary>
        public string Audience { get; set; } = "http://localhost:5000";

        /// <summary>
        /// secret是保存在服务器端的，jwt的签发生成也是在服务器端的，secret就是用来进行jwt的签发和jwt的验证，
        /// 所以，它就是你服务端的私钥，在任何场景都不应该流露出去。一旦客户端得知这个secret, 那就意味着客户端是可以自我签发jwt了
        /// 通过jwt header中声明的加密方式进行加盐secret组合加密，然后就构成了jwt的第三部分
        /// </summary>
        public string SecretKey { get; set; } = "coder1234567890987654321";
    }
}
