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

        [HttpGet("v/{user_id}", Name = "ViewUserDaily")]
        public UserDaily View(long user_id)
        {
            return UserDailyUtil.ViewUserDaily(user_id);
        }

        [HttpGet("r/{user_id}", Name = "RemoveUserDaily")]
        public string Remove(long user_id)
        {
            return UserDailyUtil.RemoveDaily(user_id);
        }
    }
}
