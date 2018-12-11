using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        public MessagesController(IDatingRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;

        }

        [HttpGet("{id}", Name = "GetMessage")] // for CreatedAtRoute http response
        public async Task<IActionResult> GetMessage(int userId, int id)
        {
            // validate user
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            var messageFromRepo = await _repo.GetMessage(id);
            if(messageFromRepo == null)
                return NotFound();
            return Ok(messageFromRepo);
        }
        
        [HttpGet] // for returning messages pages
        public async Task<IActionResult> GetMessagesForUser(int userId, [FromQuery] MessageParams messageParams)
        {
            // validate user
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            // ????
            messageParams.UserId = userId;
            // get messages from repo (returned paged list of messages)
            var messagesFromRepo = await _repo.GetMessagesForUser(messageParams);

            // formate messages before sending as response (IEnumeable for page list)
            var messages = _mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);

            // add pagination header to our response
            Response.AddPagination(messagesFromRepo.CurrentPage, messagesFromRepo.PageSize,
                 messagesFromRepo.TotalCount, messagesFromRepo.TotalPages);

            return Ok(messages);

        }

        [HttpGet("thread/{recipientId}")]
        public async Task<IActionResult> GetMessageThread (int userId, int recipientId)
        {
            // validate user
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            var messageFromRepo = await _repo.GetMessageThread(userId, recipientId);
            var messageThread = _mapper.Map<IEnumerable<MessageToReturnDto>>(messageFromRepo);
            return Ok(messageThread);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForCreationDto  messageForCreationDto)
        {
            // only for autoMapper so that it can take sender info from memory
            var sender = await _repo.GetUser(userId);
            // validate user
            if(sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            messageForCreationDto.SenderId = userId;
            var recipient = await _repo.GetUser(messageForCreationDto.RecipientId);
            if(recipient == null)
                return BadRequest("could not find user");
            // make message object from dto
            var message = _mapper.Map<Message>(messageForCreationDto);    
            // add to db
            _repo.Add(message);
            if(await _repo.SaveAll())
            {
                // NOTE: we have recipient in memory so autoMapper automatically populate message with its information
                // we also need to get sender info as well (see at start of this method)
                // autoMapper looks into memory if source object has empty props
                var messageToReturn = _mapper.Map<MessageToReturnDto>(message);
                return CreatedAtRoute("GetMessage", new {id = message.Id}, messageToReturn); // gives feedback
            }
            
            throw new Exception ("Creating the message failed on save");

        }

        [HttpPost("{id}")] // for deleting message
        public async Task<IActionResult> DeleteMessage (int id, int userId)
        {
            // validate user
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            var messageFromRepo = await _repo.GetMessage(id); // id of message to delete
            // if's for deleting message at only one side
            if (messageFromRepo.SenderId == userId)
                messageFromRepo.SenderDeleted = true;
            if (messageFromRepo.RecipientId == userId)
                messageFromRepo.RecipientDeleted = true;
            // if for deleting from db
            if(messageFromRepo.SenderDeleted && messageFromRepo.RecipientDeleted == true)
                _repo.Delete(messageFromRepo);
            
            if (await _repo.SaveAll())
                return NoContent();
            
            throw new Exception ("Deleting the message failed");
        }
    
        [HttpPost("{id}/read")] // to handle inbox and Unread messages...id == message id
        public async Task<IActionResult> MarkMessageAsRead (int userId, int id)
        {
            // validate user
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            var message = await _repo.GetMessage(id);
            // mark 'read' only messages that received with recipientId == userid
            if(message.RecipientId != userId)
                return Unauthorized();

            message.IsRead = true;
            message.DateRead = DateTime.Now;
            await _repo.SaveAll();
            return NoContent();
        }
    }
}