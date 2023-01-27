using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UserAuthentication.Dto;
using UserAuthentication.Hashing;
using UserAuthentication.Models;
using UserAuthentication.Service.AdminService;
using UserAuthentication.Token;
using Microsoft.AspNetCore.HttpOverrides;
using System.Runtime.InteropServices;
using Add_Database_Model.Models;

namespace UserAuthentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAdminService _service;
        private readonly IHashPassword _hash;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDBContext _context;
        public AdminController(IMapper mapper, IAdminService service, IHashPassword hash, IConfiguration configuration, ApplicationDBContext context)
        {
            _mapper = mapper;
            _service = service;
            _hash = hash;
            _configuration = configuration;
            _context = context;
        }

        [HttpGet("getdata"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> Get()
        {
            var admin = await _service.getAllAdmin();
            return Ok(admin);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAdmin([FromForm] AdminDto dto)
        {
            if(await _service.getByName(dto.Name) is not null)
            {
                return BadRequest($"The {dto.Name} is Exist");
            }
            
            if(await _service.getByEmail(dto.Email) is not null)
            {
                return BadRequest($"The {dto.Email} is Exist");
            }

            _hash.CraeteHashPassword(dto.Password, out byte[] passwordHash, out byte[] passwordSlot);

            var admin = _mapper.Map<Admin>(dto);
            admin.PasswordHash = passwordHash;
            admin.PasswordSlot = passwordSlot;
            admin.Eamil = dto.Email;
            //admin.IPAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
            await _service.RegisterAdmin(admin);


            return Ok(admin);

        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(AdminDto dto)
        {


            var admin = await _service.getByName(dto.Name);

            if (admin is null)
            {
                return BadRequest("The name is not exist");
            }
            if (!_hash.varifyPassword(dto.Password, admin.PasswordHash, admin.PasswordSlot))
            {
                return BadRequest("The password is wrong");
            }
            string token;
            RefreshToken refreshToken;
            if (admin.permision == 1)
            {
                token = CreateTokenAdmin(admin);

                 refreshToken =  GenerateRefreshToken(admin);
                SetRefreshToken(refreshToken, admin);
            }
            else
            {
                token = CreateToken(admin);
                 refreshToken =GenerateRefreshToken(admin);
                SetRefreshToken(refreshToken, admin);
            }
            return Ok(token);

        }
        [Authorize(Roles ="user")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Logout(int id)
        {
            var refreshToken =await _service.Logout(id);

            if(refreshToken.success == false)
            {
                return BadRequest("the logout is failed");
            }

            return Ok(refreshToken);

        }
        

        [HttpPost("refresh-token"), Authorize(Roles ="Admin")]
        public async Task<IActionResult> RefreshToken(AdminDto dto)
        {
            var admin = await _service.getByName(dto.Name);

            var refreshToken = Request.Cookies["refreshToken"];

            if (!admin.RefreshToken.Equals(refreshToken))
            {
                return Unauthorized("Invalid Refresh Token.");
            }
            else if (admin.TokenExpires < DateTime.Now)
            {
                return Unauthorized("Token expired.");
            }

            string token = CreateToken(admin);
            var newRefreshToken = GenerateRefreshToken(admin);
            SetRefreshToken(newRefreshToken, admin);

            return Ok(token);
        }
        private  RefreshToken GenerateRefreshToken(Admin admin)
        {
            var refreshToken = new RefreshToken
            {
                AdminId =admin.Id,
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now
            };

              _context.RefreshTokens.AddAsync(refreshToken);
            return refreshToken;
        }

        private void SetRefreshToken(RefreshToken newRefreshToken, Admin admin)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires
            };
            Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);

            admin.RefreshToken = newRefreshToken.Token;
            admin.TokenCreated = newRefreshToken.Created;
            admin.TokenExpires = newRefreshToken.Expires;
            _context.Admins.Update(admin);
            _context.SaveChanges();
        }

        private string CreateToken(Admin admin)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, admin.Name),
                new Claim(ClaimTypes.Role,"User")
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
        private string CreateTokenAdmin(Admin admin)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, admin.Name),
                new Claim(ClaimTypes.Role,"User"),
                new Claim(ClaimTypes.Role,"Admin")
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            

            return jwt;
        }

       




    }
}
