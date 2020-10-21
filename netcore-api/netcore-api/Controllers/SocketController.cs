using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using netcore_api.Models;

namespace netcore_api.Controllers
{
    [Route("api/[action]")]
    [ApiController]
    public class SocketController : ControllerBase
    {
        private readonly SampleSocketMessageHandler chatMessageHandler;
        public SocketController(SampleSocketMessageHandler chatMessageHandler)
        {
            this.chatMessageHandler = chatMessageHandler;
        }

        [HttpPost]
        [ActionName("socket/messages")]
        public async Task SendMessage([FromBody] SendSocketMessageRequest request)
        {
            await this.chatMessageHandler.SendMessageAsync(request.SocketId, request.Message);
        }


        [HttpPost]
        [ActionName("socket/close")]
        public async Task RemoveSocket([FromBody] RemoveSocketRequest request)
        {
            await this.chatMessageHandler.RemoveSocket(request.SocketId, request.Message);
        }
    }    
}
