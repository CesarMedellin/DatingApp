using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using System;
using System.IdentityModel.Tokens.Jwt;
using AutoMapper;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase//el controllerbase sirve para que el controlador funcione sin vista y el controller sirve solo para que utilice la vista
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        public AuthController(IAuthRepository repo, IConfiguration config, IMapper mapper)
        {
            _mapper = mapper;
            _config = config;
            _repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
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

            var usertToCreate = _mapper.Map<User>(userForRegisterDto);
            var createdUser = await _repo.Register(usertToCreate, userForRegisterDto.Password);
            var userToReturn = _mapper.Map<UserForDetailedDto>(createdUser);
            return CreatedAtRoute("GetUser", new {controller = "Users", id = createdUser.Id}, userToReturn); // asi se llama al metodo getuser del controlador
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            //Primero Busca el usuario para revisar si existe
            var userFromRepo = await _repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);

            //Aqui se retorna si el usuario o contraseña no son correctos, no se muestra cual fue el incorrecto
            //Porque no queremos que se enteren si el usuario fue el incorrecto porque podrian forzar contraseñas
            if (userFromRepo == null)
            {
                return Unauthorized();
            }


            //El claim sirve para guardar dentro del token datos importantes que durante la sesion el usuario vaya a utilizar
            //En este claim guardamos como el nameidentifier el id del usuario y como name el nombre de usuario
            var claims = new[]{
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username)
            };

            //Para validar los claim dentro del servidor y token se tiene que crear una credencial aqui abajo se cre la key
            //el token se manda a llamar en el token que tengamos en el appjson.settings
            //el token de ahi debe de ser un string largo y random
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            //y aqui abajo se pasa la key y se encripta con un hash
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            //Aqui solo se guardan todos los atributos que contendra el token:
            //como sus credenciales, sus claims y su fecha de expiracion que sera de un dia despues del login
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),//puede ser addhours para validar que se guarde el token horas
                SigningCredentials = creds
            };

            //Este nos permite crear el token basado en el tokenDescription de arriba
            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescription);
            var user = _mapper.Map<UserForLIstDto>(userFromRepo);
            return Ok(new
            {
                //Aqui se escribe el token que le regresaremos a los clientes
                token = tokenHandler.WriteToken(token),
                user 
            });
        }
    }
}