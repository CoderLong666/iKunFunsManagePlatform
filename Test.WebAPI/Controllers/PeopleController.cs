using JBBS.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;
using Test.WebAPI.Modles;

namespace Test.WebAPI.Controllers
{
    [SwaggerTag("人员管理")]
    public class PeopleController : EntitiesController
    {
        public PeopleController(ILogger<EntitiesController> logger, IOptionsMonitor<AppSettings> appSettings, DbContext dbContext) : base(logger, appSettings, dbContext)
        {
        }

        /// <summary>
        /// 查询人员
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "QueryPeople")]
        [ApiExplorerSettings(GroupName = "V1", IgnoreApi = false)]
        public async Task<List<People>> QueryPeople()
        {
            return await DbContext.Peoples.ToListAsync();
        }

        /// <summary>
        /// 新建人员
        /// </summary>
        /// <returns></returns>
        [HttpPut(Name = "CreatePeople")]
        public async Task<Guid> CreatePeople(People people)
        {
            var entity = new People
            {
                Name = people.Name,
                Sex = people.Sex,
                Birth = people.Birth,
            };
            await DbContext.AddAsync(entity);
            await DbContext.SaveChangesAsync();
            return entity.Id;
        }
    }
}
