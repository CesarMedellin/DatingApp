using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")] // este es para que se pueda acceder al controlador mas facil ejemplo http://localhost:5000/api/users
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        public UsersController(IDatingRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _repo.GetUsers();
            
            var usersToReturn = _mapper.Map<IEnumerable<UserForLIstDto>>(users);//el ienumerable es porque regresa una lista
            
            return Ok(usersToReturn);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repo.GetUser(id);

            var userToReturn = _mapper.Map<UserForDetailedDto>(user);

            return Ok(userToReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto)
        {
           if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
           {
               return Unauthorized();
           }

           var userFromRepo = await _repo.GetUser(id);

           _mapper.Map(userForUpdateDto, userFromRepo); // lo que hace aqui es que el userfromrepo va a copiar los datos de userforupdate en los campos que tengan iguales
           if (await _repo.SaveAll()) // este ejecuta el comando de saveall que guarda todos los cambios que se han hecho en el metodo, en la base de datos
           {
               return NoContent();
           }
            throw new Exception($"Updating user {id} failed on save");
        }
    }
}