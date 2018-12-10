using System;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DatingApp.API.Helpers
{
    public static class Extensions
    {
        // En el AddApplicationError se hace la validacion de los errores que ocurran en algun proceso del servidor
        // para enviarse a angular
                public static void AddApplicationError(this HttpResponse response, string message) {
            response.Headers.Add("Application-Error", message);
            response.Headers.Add("Access-Control-Expose-Headers","Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");

        }

        public static void AddPagination(this HttpResponse response, int currentPage, int itemsPerPage, int totalItems, int totalPages)
        { // esta exetensio funciona para el paginado de las aplicaiones y se tiene que comprobar si todo v bien o hubo error y se tiene que poner en los servicios del startup
            var paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);
            var camelCaseFormatter = new JsonSerializerSettings();
            camelCaseFormatter.ContractResolver = new CamelCasePropertyNamesContractResolver();
            response.Headers.Add("Pagination", JsonConvert.SerializeObject(paginationHeader, camelCaseFormatter)); // la informacion la tiene que retornar en un json serializado, y al recibirla se tiene que convertir en un json normal
            response.Headers.Add("Access-Control-Expose-Headers","Pagination"); // regresa los headers con el nombre de Pagination
        }

        // Calular la edad de persona
        public static int CalculateAge(this DateTime TheDateTime){
            var age = DateTime.Today.Year - TheDateTime.Year;
            if (TheDateTime.AddYears(age)> DateTime.Today)
            {
                age--;
            }
            return age;
        }
    }
}