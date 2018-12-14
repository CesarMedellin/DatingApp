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
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/users/{userId}/[controller]")] // este es para que se pueda acceder al controlador mas facil ejemplo http://localhost:5000/api/users
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDatingRepository _repo;
        public MessagesController(IDatingRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;

        }

        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id) {
            
         if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) // Asi se valida si el usuario que esta solicitando la peticion sea el mismo al que desea modificar
           {
               return Unauthorized();
           }
        var messageFromRepo = await _repo.GetMessage(id);

            if (messageFromRepo == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(messageFromRepo);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMessagesForUser(int userId, [FromQuery]MessageParams messageParams)
        {
             if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) // Asi se valida si el usuario que esta solicitando la peticion sea el mismo al que desea modificar
           {
               return Unauthorized();
           }
            messageParams.UserId = userId;
           var messagesFromRepo = await _repo.GetMessagesForUser(messageParams);

           var messages = _mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);

           Response.AddPagination(messagesFromRepo.CurrentPage, messagesFromRepo.PageSize, 
           messagesFromRepo.TotalCount, messagesFromRepo.TotalPages);

           return Ok(messages);
        }

        [HttpGet("thread/{recipientId}")]
        public async Task<IActionResult> GetMessageThread(int userId, int recipientId)
        {
         if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) // Asi se valida si el usuario que esta solicitando la peticion sea el mismo al que desea modificar
           {
               return Unauthorized();
           }
           var messageFromRepo = await _repo.GetMessagesThread(userId, recipientId);
           var messageThread = _mapper.Map<IEnumerable<MessageToReturnDto>>(messageFromRepo);

           return Ok(messageThread);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForCreationDto messageForCreation)
        {
            var sender = await _repo.GetUser(userId);
         if (sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) // Asi se valida si el usuario que esta solicitando la peticion sea el mismo al que desea modificar
           {
               return Unauthorized();
           }

           messageForCreation.SenderId = userId;
           var recipient = await _repo.GetUser(messageForCreation.RecipientId);

           if (recipient == null)
           {
               return BadRequest("No se pudo encontrar al usuario u ocurrio un problema");
           }
           var message = _mapper.Map<Message>(messageForCreation);
           _repo.Add(message); // con el sender y recipient se cargan los valores automaticamente y en el mapper quien sabe como pero los detecta y los pone en el mapeo de lo que se regresara
           if (await _repo.SaveAll())
           {
             var messageToReturn = _mapper.Map<MessageToReturnDto>(message);

               return CreatedAtRoute("GetMessage", new {id = message.Id}, messageToReturn); // primero crea el mensaje y con esta linea lo devuelve entrando en la otra funcion y se guarda en la variable messagetoreturn
           }
           else
           {
              throw new Exception("Error al crear el mensaje"); 
           }
        }

               [HttpPost("{id}")]
               public async Task<IActionResult> DeleteMessage(int id, int userId)
               {
                 if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) // Asi se valida si el usuario que esta solicitando la peticion sea el mismo al que desea modificar
                    {
                        return Unauthorized();
                    }

                    var messageFromRepo = await _repo.GetMessage(id);
                    if (messageFromRepo.SenderId == userId)
                    {
                        messageFromRepo.SenderDeleted = true;
                    }
                    if (messageFromRepo.RecipientId == userId)
                    {
                        messageFromRepo.RecipientDeleted = true;
                    }
                    if (messageFromRepo.SenderDeleted && messageFromRepo.RecipientDeleted)
                    {
                        _repo.Delete(messageFromRepo);
                    }
                    if (await _repo.SaveAll())
                    {
                        return NoContent();
                    }
                    else
                    {
                        throw new Exception("Error al borrar mensaje");
                    }
               }
    [HttpPost("{id}/read")]
    public async Task<IActionResult> MarkMessageAsRead(int userId, int id)
    {
        if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) // Asi se valida si el usuario que esta solicitando la peticion sea el mismo al que desea modificar
        {
            return Unauthorized();
        }
        var message = await _repo.GetMessage(id);
        if (message.RecipientId != userId)
        {
            return Unauthorized();
        }
        message.IsRead = true;
        message.DateRead = DateTime.Now;
        if (await _repo.SaveAll())
        {
            return NoContent();
        }
        else
        {
            throw new Exception("Error");
        }
    }

    }
}