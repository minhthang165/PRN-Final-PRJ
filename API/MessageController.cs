using Microsoft.AspNetCore.Mvc;
using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Service;
using PRN_Final_Project.Service.Dto;
using PRN_Final_Project.Service.Interface;

namespace PRN_Final_Project.API
{
    [ApiController]
    [Route("api/message")]
    public class MessageController : Controller
    {
        private readonly IMessageService messageService;

        public MessageController(IMessageService messageService)
        {
            this.messageService = messageService;
        }

        [HttpGet("get/{conversation_id}")]
        public async Task<ActionResult<List<Message>>> GetMessagesInConversation(int conversation_id)
        {
            var messages = await messageService.getMessageInConversation(conversation_id);
            if (messages == null || !messages.Any())
            {
                return NotFound("No messages found for this conversation.");
            }
            return Ok(messages);
        }


        [HttpPost("post")]
        public async Task<IActionResult> AddMessage([FromBody] MessageDTO messageDTO)
        {
            try
            {
                var result = await messageService.addMessage(messageDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("delete/{message_id}")]
        public async Task<IActionResult> DeleteMessage(int message_id)
        {
            try
            {
                var result = await messageService.deleteMessage(message_id);
                if (result == null)
                {
                    return NotFound("Message not found");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("update")]
        public async Task<IActionResult> UpdateMessage([FromBody] MessageDTO messageDTO)
        {
            try
            {
                var result = await messageService.updateMessage(messageDTO);
                if (result == null)
                {
                    return NotFound("Message not found");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}