using System.Collections.Generic;
using DatingApp.API.Models;
using Newtonsoft.Json;

namespace DatingApp.API.Data
{
    public class seed
    {
        private readonly DataContext _context;

        public seed(DataContext context)
        {
            _context = context;

        }

        public void SeedUsers(){
            var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
            var users = JsonConvert.DeserializeObject<List<User>>(userData);

            foreach (var user in users)
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash("password", out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                user.Username = user.Username.ToLower();

                _context.Users.Add(user);
            }
            _context.SaveChanges();
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
    }
}