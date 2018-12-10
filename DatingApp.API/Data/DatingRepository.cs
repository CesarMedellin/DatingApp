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
           users = users.Where(u => u.Gender == UserParams.Gender);

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

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}