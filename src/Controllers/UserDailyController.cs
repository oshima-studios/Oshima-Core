using Microsoft.AspNetCore.Mvc;
using Milimoe.Oshima.Core.Models;
using Milimoe.Oshima.Core.Utils;

namespace Milimoe.Oshima.Core.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserDailyController(ILogger<UserDailyController> logger) : ControllerBase
    {
        private readonly ILogger<UserDailyController> _logger = logger;

        [HttpGet("{user_id}", Name = "GetUserDaily")]
        public UserDaily Get(long user_id)
        {
            return UserDailyUtil.GetUserDaily(user_id);
        }
    }
}
