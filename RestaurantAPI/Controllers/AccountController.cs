using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;
using RestaurantAPI.Services;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RestaurantAPI.Controllers
{

    [Route("api/account")]
    [ApiController]
    public class AccountController:ControllerBase
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService=accountService;
        }

        [HttpPost("register")]
        public ActionResult RegisterUser([FromBody] RegisterUserDto dto )
        {
            _accountService.RegisterUser(dto);
            return Ok();

        }

        [HttpPost("login")]
        public ActionResult Login([FromBody]LoginDto dto)
        {
            string token = _accountService.GenerateJwt(dto);
            return Ok(token);
        }
    }
}
