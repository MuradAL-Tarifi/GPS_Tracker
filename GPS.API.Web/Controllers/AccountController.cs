using GPS.Domain.DTO;
using GPS.Domain.Views;
using GPS.Services.Users;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GPS.API.Web.Controllers
{
    /// <summary>
    ///  Account API
    /// </summary>
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userService"></param>
        public AccountController(IUserService userService)
        {
            _userService = userService;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v1/account/login")]
        [Produces(typeof(ReturnResult<UserView>))]
        public async Task<IActionResult> Login(string username, string password)
        {
            var result = await _userService.GetByUserNameAndPasswordAsync(username, password);
            return Ok(result);
        }
    }
}
