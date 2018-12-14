using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _context;

        public DatingRepository(DataContext context)
        {
            _context = context;

        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            return await _context.Likes.FirstOrDefaultAsync(u => u.LikerId == userId && u.LikeeId == recipientId);
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await _context.Photos.Where(u => u.UserId == userId).FirstOrDefaultAsync(p => p.IsMain);
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);
            return photo;
        }

        public async Task<User> GetUser(int id)
        {
           var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == id);
           return user;
        }

        //public async Task<IEnumerable<User>> GetUsers()
        //{
         //  var users = await _context.Users.Include(p => p.Photos).ToListAsync();
          // return users;
        //}

        public async Task<PagedList<User>> GetUsers(UserParams UserParams)
        {

           var users = _context.Users.Include(p => p.Photos).AsQueryable();
            // de todos los usuarios excluira el que hizo la peticion
           users = users.Where(u => u.Id != UserParams.UserId);
            // de todos los usuarios traera por default el genero contrario al usuario de la peticion

            // aqui es para saber si entrara a la pagina de todos los usuarios o a la lista de los likes
            if (!UserParams.Likees && !UserParams.Likers)
            {
                users = users.Where(u => u.Gender == UserParams.Gender);
            }


           // usuarios que les di likes
           if (UserParams.Likers)
           {
               var userLikers = await GetUserLikes(UserParams.UserId, UserParams.Likers);
               users = users.Where(u => userLikers.Contains(u.Id));
           }

           // usuarios que me dieron like
            if (UserParams.Likees)
            {
               var userLikees = await GetUserLikes(UserParams.UserId, UserParams.Likers);
               users = users.Where(u => userLikees.Contains(u.Id));
            }
            // este sirve para filtrar por la edad maxima y minima
            if (UserParams.MinAge != 18 || UserParams.MaxAge != 99)
            {
                var mincumpleaños = DateTime.Today.AddYears(-UserParams.MaxAge - 1);
                var maxcumpleaños = DateTime.Today.AddYears(-UserParams.MinAge);

                users = users.Where(u => u.DateOfBirth >= mincumpleaños && u.DateOfBirth <= maxcumpleaños);
            }
            if (!string.IsNullOrEmpty(UserParams.OrderBy))
            {
                switch (UserParams.OrderBy)
                {
                    case "created":
                        users = users.OrderByDescending(u => u.Created);
                        break;
                    default:
                    users = users.OrderByDescending(u => u.LastActive);
                    break;
                }
            }
           // aqui no afecta en nada, sigue retornando el mismo tipo de objeto pero ahora los regresa con el filtro
           return await PagedList<User>.CreateAsync(users, UserParams.PageNumber, UserParams.PageSize);
        }

        private async Task<IEnumerable<int>> GetUserLikes(int id, bool likers) {
            var user = await _context.Users.Include(x => x.Likers).Include(x => x.Likees).FirstOrDefaultAsync(u => u.Id == id);
            if (likers)
            {
                return user.Likers.Where(u => u.LikeeId == id).Select(i => i.LikerId);
            }
            else {
                return user.Likees.Where(u => u.LikerId == id).Select(i => i.LikeeId);
            }
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);
        }

        public Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams)
        {
            var messages = _context.Messages.Include(u => u.Sender).ThenInclude(p=>p.Photos).Include(u => u.Recipient).ThenInclude(p=>p.Photos).AsQueryable();
            switch (messageParams.MessageContainer)
            {
                case "Inbox":
                    messages = messages.Where(u=>u.RecipientId == messageParams.UserId && u.RecipientDeleted == false);
                    break;
                case "Outbox":
                    messages = messages.Where(u=>u.SenderId == messageParams.UserId && u.SenderDeleted == false);
                    break;
                default:
                messages = messages.Where(u=>u.RecipientId == messageParams.UserId && u.RecipientDeleted == false && u.IsRead == false);
                break;
            }
            messages = messages.OrderByDescending(d=>d.MessageSent);
            return PagedList<Message>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<Message>> GetMessagesThread(int userId, int recipientId)
        {
             var messages = await _context.Messages
             .Include(u => u.Sender).ThenInclude(p=>p.Photos)
             .Include(u => u.Recipient).ThenInclude(p=>p.Photos)
             .Where(m => m.RecipientId == userId && m.RecipientDeleted == false && m.SenderId == recipientId 
             || m.RecipientId == recipientId && m.SenderId == userId && m.SenderDeleted == false)
             .OrderByDescending(m=>m.MessageSent)
             .ToListAsync();
             return messages;
        }
    }
}