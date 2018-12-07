using System;
using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.Dtos
{            //los archivos dto o los objetos dto sirven para que al momento de llenar un 
            //metodo o funcion no se tenga que enviar todos los campos como parametros 
            //y solo enviarlo en la clase del dto para un mejor orden
    public class UserForRegisterDto
    {
        [Required]
        public string Username { get; set; }  

        
        [StringLength(8, MinimumLength = 4, ErrorMessage = "Debes escribir una contraseña de entre 4 y 8 caracteres")]
        [Required]
        public string Password { get; set; }

        [Required]
        public string Gender { get; set; }
        [Required]
        public string KnownAs { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string City { get; set; }
        
        [Required]
        public string Country { get; set; }
        
        [Required]
        public DateTime Created { get; set; }
        [Required]
        public DateTime LastActive { get; set; }

        public UserForRegisterDto()
        {
            Created = DateTime.Now;
            LastActive = DateTime.Now;
        }
    }
}