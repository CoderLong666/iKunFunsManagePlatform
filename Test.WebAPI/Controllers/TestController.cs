using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Test.WebAPI.Modles;

namespace Test.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "iKun")]
    public class TestController : Controller
    {
        private readonly DbContext _dbContext;

        public TestController(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// 查询人员
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "Query")]
        [ApiExplorerSettings(GroupName = "V1", IgnoreApi = false)]
        public async Task<List<People>> Get()
        {
            return await _dbContext.Peoples.ToListAsync();
        }

        /// <summary>
        /// 新建人员
        /// </summary>
        /// <returns></returns>
        [HttpPut(Name = "Creat")]
        public async Task<Guid> Creat(People people)
        {
            var entity = new People
            {
                Name = people.Name,
                Sex = people.Sex,
                Birth = people.Birth,
            };
            await _dbContext.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity.Id;
        }
    }
}
