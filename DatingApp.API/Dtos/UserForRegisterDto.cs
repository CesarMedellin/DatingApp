namespace DatingApp.API.Dtos
{            //los archivos dto o los objetos dto sirven para que al momento de llenar un 
            //metodo o funcion no se tenga que enviar todos los campos como parametros 
            //y solo enviarlo en la clase del dto para un mejor orden
    public class UserForRegisterDto
    {
        public string Username { get; set; }  
        public string Password { get; set; }
    }
}