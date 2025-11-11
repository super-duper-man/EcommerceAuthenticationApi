using AuthenticationApi.Application.Dtos;
using AuthenticationApi.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Resource.Share.Lib.Responses;

namespace AuthenticationApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController(IUser userInterface) : ControllerBase
    {
        [HttpPost("Register")]
        public async Task<ActionResult<Response>> Register([FromBody] AppUserDto userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await userInterface.Register(userDto);

            return result.Flag ? Ok(result) : BadRequest(Request);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<Response>> Login([FromBody] LoginDto loginDto)
        {
        
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

           var result = await userInterface.Login(loginDto);
    
           return result.Flag ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<GetUserDto>> GetUser(int id)
        {
            if(id <= 0)
                return BadRequest("Invalid user Id!");

            var user = await userInterface.GetUser(id);

            return user is not null ? Ok(user) : NotFound($"User by Id {id} not found!");
        }
    }
}
