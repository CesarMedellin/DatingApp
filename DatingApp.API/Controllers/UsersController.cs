using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))] // cada vez que entre a un metodo de este controlador se ejecutara esta funcion que actualiza la ultima hora activa del usuario
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
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
        {
            // sirve para la paginacion, primero se trae el id y luego el usuario del login
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userFromRepo = await _repo.GetUser(currentUserId);
            // luego al userparams se le da el id del usuario que hizo la peticion
            userParams.UserId = currentUserId;
            if (string.IsNullOrEmpty(userParams.Gender))
            {
                // aqui dependiendo el genero del que sea el del login mandara el genero contrario
                userParams.Gender = userFromRepo.Gender == "male" ? "female" : "male";
            }
            var users = await _repo.GetUsers(userParams);
            
            var usersToReturn = _mapper.Map<IEnumerable<UserForLIstDto>>(users);//el ienumerable es porque regresa una lista
            
            // f
            Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            return Ok(usersToReturn);
        }
        [HttpGet("{id}", Name="GetUser")] // el nombre sirve para que se pueda utilizar el metodo desde otro metodo
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repo.GetUser(id);

            var userToReturn = _mapper.Map<UserForDetailedDto>(user);

            return Ok(userToReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto)
        {
           if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) // Asi se valida si el usuario que esta solicitando la peticion sea el mismo al que desea modificar
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

        [HttpPost("{id}/Like/{recipientId}")]
        public async Task<IActionResult> LikeUser(int id, int recipientId)
        {
           if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) // Asi se valida si el usuario que esta solicitando la peticion sea el mismo al que desea modificar
           {
               return Unauthorized();
           }

           var like = await _repo.GetLike(id, recipientId);
           if (like != null)
           {
               return BadRequest("Ya te gusta este usuario");
           }
           if (await _repo.GetUser(recipientId) == null)
           {
               return NotFound();
           }
           like = new Like
           {
               LikerId = id,
               LikeeId = recipientId
           };
           _repo.Add<Like>(like);
           if (await _repo.SaveAll())
           {
               return Ok();
           }
           else {
               return BadRequest("Fallo al dar me gusta el usuario");
           }

        }
    }
}