using JBBS.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;
using Test.WebAPI.Modles;

namespace Test.WebAPI.Controllers
{
    [SwaggerTag("用户管理")]
    public class UserController : EntitiesController
    {
        public UserController(ILogger<EntitiesController> logger, IOptionsMonitor<AppSettings> appSettings, DbContext dbContext) : base(logger, appSettings, dbContext)
        {
        }


        /// <summary>
        /// 新建用户
        /// </summary>
        /// <returns></returns>
        [HttpPut(Name = "CreateUser")]
        public async Task<Guid> CreateUser(CreateUserRequest request)
        {
            var entity = new User
            {
                Name = request.Name,
                Password = request.Password,
                LegalName = request.LegalName,
                Phone = request.Phone,
                Email = request.Email,
            };
            await DbContext.AddAsync(entity);
            await DbContext.SaveChangesAsync();
            return entity.Id;
        }
        public class CreateUserRequest
        {
            /// <summary>
            /// 用户名（唯一，登录用）
            /// </summary>
            public string Name { get; set; } = "";

            /// <summary>
            /// 密码
            /// </summary>
            public string Password { get; set; } = "";

            /// <summary>
            /// 姓名
            /// </summary>
            public string LegalName { get; set; } = "";

            /// <summary>
            /// 电话
            /// </summary>
            public string Phone { get; set; } = "";

            /// <summary>
            /// 邮箱
            /// </summary>
            public string Email { get; set; } = "";
        }




    }
}
