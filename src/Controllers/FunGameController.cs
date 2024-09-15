using Microsoft.AspNetCore.Mvc;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Testing.Tests;

namespace Milimoe.Oshima.Core.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FunGameController(ILogger<UserDailyController> logger) : ControllerBase
    {
        private readonly ILogger<UserDailyController> _logger = logger;

        [HttpGet("test")]
        public List<string> GetTest()
        {
            return FunGameSimulation.StartGame(false);
        }
        
        [HttpPost("post")]
        public string PostName([FromBody] string name)
        {
            return NetworkUtility.JsonSerialize($"Your Name received successfully: {name}.");
        }
    }
}
