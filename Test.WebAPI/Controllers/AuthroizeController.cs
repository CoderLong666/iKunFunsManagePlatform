using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Test.WebAPI;
using Test.WebAPI.Modles;
using DbContext = Test.WebAPI.DbContext;

namespace JwtAuthSample.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthroizeController : ControllerBase
    {
        private readonly DbContext _dbContext;
        private readonly JwtSeetings _jwtSeetings;
        private readonly IJwtAuthManager _jwtAuthManager;


        public AuthroizeController(IOptions<JwtSeetings> jwtSeetingsOptions, IJwtAuthManager jwtAuthManager, DbContext dbContext)
        {
            _jwtSeetings = jwtSeetingsOptions.Value;
            _jwtAuthManager = jwtAuthManager;
            _dbContext = dbContext;
        }


        [HttpPost]
        public async Task<ActionResult> Login([FromBody] User user)
        {
            if (string.IsNullOrWhiteSpace(user.Name) || string.IsNullOrWhiteSpace(user.Password))
                return BadRequest();

            var userEntity = await _dbContext.Set<User>().Where(x => x.Name == user.Name).FirstOrDefaultAsync();
            if (userEntity == null) return BadRequest("用户名不存在");
            if (userEntity.Password != user.Password) return BadRequest("密码错误");
            try
            {
                var claims = new Claim[]
                {
                    new Claim(ClaimTypes.Name,userEntity.Name),
                    new Claim(ClaimTypes.Role,userEntity.Role),
                    new Claim("LegalName", userEntity.LegalName),
                    new Claim("Phone", userEntity.Phone),
                    new Claim("Email", userEntity.Email),
                };
                var token = _jwtAuthManager.Generate(userEntity.Name, claims);
                return Ok(new { token = token });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}