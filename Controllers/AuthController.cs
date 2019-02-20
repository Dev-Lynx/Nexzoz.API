using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Nexzoz.API.Data.Interfaces;
using Nexzoz.API.Models.Entities;
using Nexzoz.API.ViewModels;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Nexzoz.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        #region Properties

        #region Internals
        UserManager<NexzozUser> UserManager { get; }
        IMapper Mapper { get; }
        IAuthRepository Auth { get; }
        IConfiguration Configuration { get; }
        #endregion

        #endregion

        #region Constructors
        public AuthController(UserManager<NexzozUser> userManager, IAuthRepository auth, IConfiguration configuration, IMapper mapper)
        {
            UserManager = userManager;
            Auth = auth;
            Configuration = configuration;
            Mapper = mapper;
        }
        #endregion

        #region Methods
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterationViewModel model)
        {
            if (model == null) return BadRequest("The registeration model is null");
            if (await Auth.UserExists(model.Email)) return BadRequest("Username already exists");

            IEnumerable<object> errors = null;
            if ((errors = await Auth.TryCreateUser(model.Email, model.Password)).Count() > 0)
                return StatusCode(201, errors);

            var user = await Auth.Register(model.Email, model.Password);
            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginViewModel model)
        {
            var user = await Auth.Login(model.Email, model.Password);
            if (user == null) return Unauthorized();

            var token = await Auth.GenerateJwt(model.Email, model.Password);
            return Ok(token);
        }
        #endregion

    }
}
