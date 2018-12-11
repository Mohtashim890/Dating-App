using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System;
using DatingApp.API.Helpers;
using DatingApp.API.Models;

namespace DatingApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;

        public UsersController(IDatingRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams) // userParams coming from query string
        {
            // Filtering
            // we need currently loggedin user's id and gender
            // fetch that user from repo and update userParams
            // set gender in userParams to opposite to current user's gender if null
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userFromRepo = await _repo.GetUser(currentUserId);
            userParams.UserId = currentUserId;
            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = userFromRepo.Gender == "male" ? "female": "male";
            }

            var users = await _repo.GetUsers(userParams);
            // using automapper to fill data in UserForDetailedDto's
            // we also need to tell AutoMapper explicitly about Classes and there properties
            // to map using automapper profile (See AutoMapperProfile.cs)
            // IEnumerable list is sendable instead of PagedList..
            var userForReturn = _mapper.Map<IEnumerable<UserForListsDto>>(users);
            // add pagination in http response
            Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            return Ok(userForReturn);
        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repo.GetUser(id);
            // using automapper to store data in UserForDetailedDto's
            var userForReturn = _mapper.Map<UserForDetailedDto>(user);
            return Ok(userForReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser (int id, UserForUpdateDto userForUpdateDto) {
            // check if path id and token id is same to prevent user from updating other accounts
            if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await _repo.GetUser(id);
            // map values
            _mapper.Map(userForUpdateDto, userFromRepo);
            // maping is not async so call SaveAll()
            if(await _repo.SaveAll())
                return NoContent();
            
            throw new Exception($"updating user with {id} failed on save");
        }

        [HttpPost("{id}/like/{recipientId}")]
        public async Task<IActionResult> LikeUser(int id, int recipientId)
        {
            // check if request is sent by current(tokanized) user
            if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            // check if like already exist
            var like = await _repo.GetLike(id, recipientId);
            if(like != null)
                return BadRequest("You already liked this user");
            // check if recipient exist in db
            if(await _repo.GetUser(recipientId) == null)
                return NotFound();
            
            // adding like is not async so no need to user await...it is storing data
            // into memory so need to call SaveAll()
            like = new Like{LikerId = id, LikeeId = recipientId};
            _repo.Add<Like>(like);
            if(await _repo.SaveAll())
                return Ok();
            // if SaveAll() failed than
            return BadRequest("Failed to like user");
        }

    }
}