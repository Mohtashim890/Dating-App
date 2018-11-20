using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{
    // ASP.NET > 2.1 have following services:
    // [Route("api/[controller]")] enforce attribute routing...[controller] == [___]Controller.cs
    // Automatically validate our requests 
    // For new controller, it inherits this base 'ControllerBase' (IActionResults defined in them)
    // without view support (b/c view is provided by Angular in our case)
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        //inject DataContext Class into it using constructor
        // save it into readonly variable
        private readonly DataContext _context;
        public ValuesController(DataContext context)
        {
            _context = context;
        }
        // GET api/values
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetValues()
        {
            var values = await _context.Values.ToListAsync();
            return Ok(values);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetValue(int id)
        {
            var value = await _context.Values.FirstOrDefaultAsync(x => x.Id == id);
            return Ok(value);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
