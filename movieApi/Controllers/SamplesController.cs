using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace movieApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // protects all routes within the controller
    public class SamplesController : ControllerBase
    {
        // GET: api/<Samples>
        [HttpGet]
        [AllowAnonymous] // even if the controller is protected, treat this route as unprotected
        public string Get()
        {
            return "Hello User";
        }

        // GET api/<Samples>/5
        [Authorize(Roles = "Admin")] // protect only this route
        [HttpGet("{id}")]
        
        public string Get(int id)
        {
            return "Hello Admin";
        }

        // POST api/<Samples>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<Samples>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<Samples>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
