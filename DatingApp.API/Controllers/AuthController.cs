using System;
using AutoMapper;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    // ASP.NET > 2.1 have following services:
    // [Route("api/[controller]")] enforce attribute routing...[controller] == [___]Controller.cs
    // [ApiController] Automatically validate our requests (Model States)
    // For new controller, it inherits this base 'ControllerBase' (IActionResults defined in them)
    // without view support (b/c view is provided by Angular in our case)
    // helps in infering 'from where data is coming' for controller methods ie. they can have data
    // directly from http requests via DTO's
    [Route("api/[controller]")]
    [ApiController]  
    public class AuthController : ControllerBase
    {
        // inject AuthController Repository
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        public AuthController(IAuthRepository repo, IConfiguration config, IMapper mapper)
        {
            _mapper = mapper;
            _config = config;
            _repo = repo;

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            //  to make data consistent in db use only lowercase
            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            if (await _repo.UserExist(userForRegisterDto.Username))
                return BadRequest("username already exist");

            var userToCreate = new User
            {
                Username = userForRegisterDto.Username
            };

            var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);

            // return CreatedAtRoute();
            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            //  to make data consistent in db use only lowercase
            userForLoginDto.Username = userForLoginDto.Username.ToLower();
            var userFromRepo = await _repo.Login(userForLoginDto.Username, userForLoginDto.Password);

            if (userFromRepo == null)
                return Unauthorized();

            // build claims for the token
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username)
            };

            // to get hashed key (credentials) for this token used by server to authenticate token
            var key = new SymmetricSecurityKey(Encoding.UTF8
            .GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // token's body
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            // token handler allows us to create token
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // we will also send a Dto that contains user information (UserForListDto)
            // along with token so that we can use it like for user photo on nav-bar
            var user = _mapper.Map<UserForListsDto>(userFromRepo);
            return Ok( new {
                token = tokenHandler.WriteToken(token),
                user 
            });

        }

    }
}