using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
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
        public async Task<IActionResult> GetUsers()
        {
            var users = await _repo.GetUsers();
            // using automapper to store data in UserForDetailedDto's
            // we also need to tell AutoMapper explicitly about Classes and there properties
            // to map using automapper profile (See AutoMapperProfile.cs)
            var userForReturn = _mapper.Map<IEnumerable<UserForListsDto>>(users);
            return Ok(userForReturn);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repo.GetUser(id);
            // using automapper to store data in UserForDetailedDto's
            var userForReturn = _mapper.Map<UserForDetailedDto>(user);
            return Ok(userForReturn);
        }
    }
}