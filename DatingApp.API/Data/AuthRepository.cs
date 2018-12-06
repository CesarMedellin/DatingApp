using System;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;

        public AuthRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<User> Login(string username, string password)
        {
            //lineaprocedimiento
           var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(x => x.Username == username);
           if (user == null)
           {
               return null;
           }
           if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
           {
               return null;
           }
           return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
             using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt)){
                //aqui arriba se le pone la key del passwordsalt y con esa key se generara un passwordhash aqui abajo
                //se tendra que comparar el passwordhash de aqui abajo con el que esta guardado con el usuario para saber si es la contraseña correcta
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i]!= passwordHash[i])
                    {
                        return false;
                    }
                    
                }
            }
            return true;
        }

        public async Task<User> Register(User user, string password)
        {
           byte[] passwordHash, passwordSalt;
           CreatePasswordHash(password, out passwordHash, out passwordSalt);//el out es de que una vez que se actualize el valor del passwordHash dentro del metodo entonces el valor se retornara solo
           //lineaprocedimiento
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512()){
                //la variable hmac manda a llamar el encriptado de seguridad hmacsha512 para utilizar sus funciones de encriptacion
                //el password salt sirve para dificultar la forma de saber si un dos usuarios tienen la misma contraseña para el hash
                passwordSalt = hmac.Key;//genera una key
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));//hay que convertir el string password en array de bytes y asi se hace
            }
            
        }
        public async Task<bool> UserExists(string username)
        {
            //lineaprocedimiento
            if (await _context.Users.AnyAsync(x=>x.Username == username))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}