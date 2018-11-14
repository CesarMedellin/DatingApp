using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        public AuthController(IAuthRepository repo)
        {
            _repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto){
            //validar request
            //los archivos dto o los objetos dto sirven para que al momento de llenar un 
            //metodo o funcion no se tenga que enviar todos los campos como parametros 
            //y solo enviarlo en la clase del dto para un mejor orden
//hola
            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();
            if (await _repo.UserExists(userForRegisterDto.Username))
            {
                return BadRequest("Usuario ya existe");
            }

            var usertToCreate = new User{
                Username = userForRegisterDto.Username
            };
            var cratedUser = await _repo.Register(usertToCreate, userForRegisterDto.Password);

            return StatusCode(201);
        }
    }
}