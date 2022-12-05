using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using UserAuthentication.Dto;
using UserAuthentication.Hashing;
using UserAuthentication.Models;
using UserAuthentication.Service.AdminService;

namespace UserAuthentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAdminService _service;
        private readonly IHashPassword _hash;
        

        public AdminController(IMapper mapper, IAdminService service, IHashPassword hash)
        {
            _mapper = mapper;
            _service = service;
            _hash = hash;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAdmin(AdminDto dto)
        {
            _hash.CraeteHashPassword(dto.Password, out byte[] passwordHash, out byte[] passwordSlot);

            var admin = _mapper.Map<Admin>(dto);
            admin.PasswordHash = passwordHash;
            admin.PasswordSlot = passwordSlot;

            await _service.RegisterAdmin(admin);

            return Ok(admin);

        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(AdminDto dto)
        {


            var admin = await _service.getName(dto.Name);

            if ( admin is null)
            {
                return BadRequest("The name is not exist");
            }
            if (!_hash.varifyPassword(dto.Password, admin.PasswordHash, admin.PasswordSlot))
            {
                return BadRequest("The password is wrong");
            }

            return Ok(admin);
            
        }

       
    }
}
