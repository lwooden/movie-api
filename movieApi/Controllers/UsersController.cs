using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using movieApi.Data;
using movieApi.Models;

namespace movieApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private MovieDbContext _dbContext;

        public UsersController(MovieDbContext dbContext)
        {
            _dbContext = dbContext;

        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_dbContext.User);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var user = _dbContext.User.Find(id);

            if (user == null)
            {
                BadRequest("User Does Not Exists!");
            }
            return Ok(user);
        }


            [HttpPost]
        public IActionResult Register([FromBody] User user)
        {
            var emailExists = _dbContext.User.Where(u => u.Email == user.Email).SingleOrDefault();

            if (emailExists != null)
            {
                return BadRequest("User Email already exists!");
            }

            var userObj = new User
            {
                Name = user.Name,
                Email = user.Email,
                Password = user.Password,
                Role = "Users"

            };

            _dbContext.User.Add(userObj);
            _dbContext.SaveChanges();

            return StatusCode(StatusCodes.Status201Created);

        }
    }
}
