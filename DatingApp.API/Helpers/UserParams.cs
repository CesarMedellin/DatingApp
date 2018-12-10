namespace DatingApp.API.Helpers
{
    public class UserParams
    {
        // este helper sirve para medir recibir la paginacion en la que un usuario mande la pticion
        // las clases que ya tienen valor son las que tienen un valor por default... No se en que caso se llega al valor por default
        // creo que cada public son los parametros que se reciben en la url para filtrar
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        private int pageSize = 10;
        public int PageSize
        {
            get { return pageSize;}
            set { pageSize = (value > MaxPageSize) ? MaxPageSize : value;}
        }

        public int UserId { get; set; }
        public string Gender { get; set; }

        public int MinAge { get; set; } = 18;
        public int MaxAge { get; set; } = 99;

        public string OrderBy { get; set; }
        
    }
}