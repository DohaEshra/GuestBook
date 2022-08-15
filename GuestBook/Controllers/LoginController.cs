using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GuestBook.Models;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace GuestBook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        guestbookContext _context;
        User user;
        public LoginController(guestbookContext _db)
        {
            this._context = _db;
        }

        [HttpPost]
        public IActionResult LoginPost(string email, string password)
        {
            user = _context.Users.Where(a => a.Email == email && a.Password == password).FirstOrDefault();
            if (user != null)// an existing user
            {
                //generate token 
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MySecretKey00000"));

                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var data = new List<Claim>();
                data.Add(new Claim("ID", user.UserId.ToString()));
                data.Add(new Claim("email", user.Email));
                data.Add(new Claim("Password", user.Password));


                var token = new JwtSecurityToken(
                   claims: data,
                   expires: DateTime.Now.AddMinutes(120),
                   signingCredentials: credentials);

                return Ok(new JwtSecurityTokenHandler().WriteToken(token));

            }
            else// not registered
            {
                return Unauthorized(); //401
            }
        }
    }
}
