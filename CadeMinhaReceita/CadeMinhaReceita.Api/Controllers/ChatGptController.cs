using CadeMinhaReceita.Domain.Contracts.Domain;
using Microsoft.AspNetCore.Mvc;

namespace CadeMinhaReceita.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatGptController : ControllerBase
    {
        private readonly ILogger<ChatGptController> _logger;
        private readonly IChatGptService _chatGptService;

        public ChatGptController(ILogger<ChatGptController> logger, IChatGptService chatGptService)
        {
            _logger = logger;
            _chatGptService = chatGptService;
        }

        [HttpGet(Name = "TalkWith")]
        public async Task<IActionResult> TalkWith([FromQuery] string message)
        {
            var response = await _chatGptService.TalkWith(message);
            return Ok(response);
        }
    }
}
