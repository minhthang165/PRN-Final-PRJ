using Microsoft.AspNetCore.Mvc;
using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Service.Dto;
using PRN_Final_Project.Service.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN_Final_Project.API
{
    [ApiController]
    [Route("api/conversation-user")]
    public class ConversationUserController : ControllerBase
    {
        private readonly IConversationUserService _conversationUserService;

        public ConversationUserController(IConversationUserService conversationUserService)
        {
            _conversationUserService = conversationUserService;
        }

        /// <summary>
        /// Get conversation of a user
        /// </summary>
        [HttpGet("conversation/{userId}")]
        public async Task<ActionResult<List<Conversation>>> GetAllConversation(int userId)
        {
            try
            {
                var conversations = await _conversationUserService.GetConversationsByUserIdAsync(userId);
                return Ok(conversations);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get all users in a conversation
        /// </summary>
        [HttpGet("get-users/{conversationId}")]
        public async Task<ActionResult<List<Conversation_user>>> GetAllUsersInConversation(int conversationId)
        {
            try
            {
                var users = await _conversationUserService.GetUsersInConversationAsync(conversationId);
                return Ok(users);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Add user to group
        /// </summary>
        [HttpPost("add-user")]
        public async Task<ActionResult<user>> AddUserToGroup([FromBody] ConversationUserDTO conversationUserDTO)
        {
            try
            {
                var result = await _conversationUserService.AddUserToConversationAsync(
                    conversationUserDTO.user_id,
                    conversationUserDTO.conversation_id);
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Remove a user from conversation
        /// </summary>
        [HttpDelete("{conversationId}/users/{userId}")]
        public async Task<ActionResult<string>> RemoveUser(int conversationId, int userId)
        {
            try
            {
                var result = await _conversationUserService.RemoveUserFromConversationAsync(userId, conversationId);
                return Ok("User removed successfully");
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}