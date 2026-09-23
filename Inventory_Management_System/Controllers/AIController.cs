using Inventory_Management_System.Services;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Management_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIController : ControllerBase
    {
        private readonly IAIService _aiService;

        public AIController(IAIService aiService)
        {
            _aiService = aiService;
        }

        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] AIRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest(new
                {
                    message = "Question cannot be empty."
                });
            }

            var response = await _aiService.AskAsync(request.Message);

            return Ok(new
            {
                question = request.Message,
                response = response
            });
        }
    }

    public class AIRequest
    {
        public string Message { get; set; }
    }
}
