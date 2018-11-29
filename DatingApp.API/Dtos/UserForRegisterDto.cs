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
    }
}