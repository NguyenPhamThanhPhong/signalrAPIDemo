using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using signalr.Model;
using signalr.Services;
using System.Security.Claims;

namespace signalr.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly List<User> _messages = new List<User>()
        {
            new User(){Id = "1", Name = "phong1", Logincode = "1234"},
            new User(){Id = "2", Name = "phong2", Logincode="4567"},
            new User(){Id = "3", Name = "phong3", Logincode="7890"}
        };
        private readonly TokenGenerator _tokenGenerator;
        public PostController(TokenGenerator tokenGenerator)
        {
            _tokenGenerator = tokenGenerator;
        }

        [HttpGet("/user/login/{loginCode}")]
        public IActionResult Login(string loginCode)
        {
            var user = _messages.FirstOrDefault(x => x.Logincode == loginCode);
            if (user == null)
            {
                return NotFound();
            }
            var token = _tokenGenerator.GenerateAccessToken(user);
            Request.HttpContext.Response.Cookies.Append("token", token, new CookieOptions()
            {
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true
            });

            return Ok(user);
        }
        [Authorize]
        [HttpPost("/post/create")]
        public IActionResult Create([FromBody] string post)
        {
            var name = User.FindFirst(ClaimTypes.Name)?.Value;
            if(name==null)
                return BadRequest("not yet identified");
            var notification = new Notification()
            {
                Id = Guid.NewGuid().ToString(),
                Title = "Thong bao moi",
                Message = $"{name}, thang loz nay` vua tao post",
                RedirectUrl = "https://.... - dẫn link tới cái post bên UI",
                IsRead = false
            };
            return Ok(notification);
        }
    }
}
