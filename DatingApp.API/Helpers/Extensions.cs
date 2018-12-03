using System;
using Microsoft.AspNetCore.Http;

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