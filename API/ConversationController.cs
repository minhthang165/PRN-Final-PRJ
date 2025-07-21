using Microsoft.AspNetCore.Mvc;
using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Service.Dto;
using PRN_Final_Project.Service.Interface;

namespace PRN_Final_Project.API
{
    [ApiController]
    [Route("api/conversation")]
    public class ConversationController : Controller
    {
        private readonly IConversationService _conversationService;

        public ConversationController(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        [HttpGet("group/{conversation_id}")]
        public async Task<ActionResult> GetGroup(int conversation_id)
        {
            try
            {
                var conversation = await _conversationService.GetConversationByIdAsync(conversation_id);
                if (conversation == null)
                {
                    return NotFound("Conversation not found");
                }
                return Ok(conversation);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("group/create")]
        public async Task<ActionResult> CreateGroup([FromBody] ConversationDTO conversationDTO)
        {
            try
            {
                var result = await _conversationService.CreateConversationAsync(conversationDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("group/delete/{conversation_id}")]
        public async Task<ActionResult> DeleteGroup(int conversation_id)
        {
            try
            {
                var result = await _conversationService.DeleteConversationAsync(conversation_id);
                if (result == null)
                {
                    return NotFound("Conversation not found");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("group/update/{conversation_id}")]
        public async Task<ActionResult> UpdateGroup(int conversation_id, [FromBody] ConversationDTO conversationDTO)
        {
            try
            {
                var result = await _conversationService.UpdateConversationAsync(conversationDTO);
                if (result == null)
                {
                    return NotFound("Conversation not found");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("find-one-to-one/{userId1}/{userId2}")]
        public async Task<ActionResult> FindOneToOneConversation(int userId1, int userId2)
        {
            try
            {
                var conversation = await _conversationService.findOneToOneConversationAsync(userId1, userId2);
                if (conversation == null)
                {
                    return NotFound("One-to-one conversation not found");
                }
                return Ok(conversation);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}