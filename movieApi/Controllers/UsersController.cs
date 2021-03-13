using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AuthenticationPlugin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using movieApi.Data;
using movieApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace movieApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private MovieDbContext _dbContext;

        private IConfiguration _configuration;
        private readonly AuthService _auth;

        // controller constructor
        // all configuration of the controller is done here at runtime
        public UsersController(MovieDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _auth = new AuthService(_configuration);

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
                Password = SecurePasswordHasherHelper.Hash(user.Password), // hash plaintext password before saving to database
                Role = "Users"

            };

            _dbContext.User.Add(userObj);
            _dbContext.SaveChanges();

            return StatusCode(StatusCodes.Status201Created);

        }

        [HttpPost]
        public IActionResult Login([FromBody] User user)
        {
            // get the user from the db
            var userEmail = _dbContext.User.Where(u => u.Email == user.Email).SingleOrDefault();


            // if user was not found, return notFound
            if (userEmail == null)
            {
                return NotFound();
            }

            // if plaintext password doesn't match hashed password, return unauthorized
            if (!SecurePasswordHasherHelper.Verify(user.Password, userEmail.Password))
            {
                return Unauthorized();
            }

            // otherwise, generate JWT Token

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Email, user.Email),

                // add role metadata to claim so it can be used for authorization for each request
                new Claim(ClaimTypes.Role, userEmail.Role) 
            };

            var token = _auth.GenerateAccessToken(claims);

            // return token to the client
            return new ObjectResult(new
            {
                access_token = token.AccessToken,
                expires_in = token.ExpiresIn,
                token_type = token.TokenType,
                creation_time = token.ValidFrom,
                expiration_time = token.ValidTo,
                user_id = userEmail.Id

            });


        }
    }
}
