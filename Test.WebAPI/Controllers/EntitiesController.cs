using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Data;
using Test.WebAPI;

namespace JBBS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "iKun")]//小黑子禁入
    public abstract class EntitiesController : ControllerBase
    {
        protected readonly Test.WebAPI.DbContext DbContext;
        protected readonly AppSettings AppSettings;
        protected readonly ILogger<EntitiesController> Logger;

        protected EntitiesController(ILogger<EntitiesController> logger, IOptionsMonitor<AppSettings> appSettings,
            Test.WebAPI.DbContext dbContext)
        {
            AppSettings = appSettings.CurrentValue;
            DbContext = dbContext;
            Logger = logger;
        }



    }
}
