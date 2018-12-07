using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using DatingApp.API.Helpers;
using AutoMapper;

namespace DatingApp.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(x=> x.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));//aqui va a buscar en el archivo appsettings.json la conexion default
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
            .AddJsonOptions(opt => {
                opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore; // este funciono para que desde postman mandara registro de un usuario con autorizacion de otro
            });
            services.AddCors();//este sirve para usar que se puedan llamar los servicios desde angular
            services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));
            services.AddAutoMapper();// sirve pr cundo queremso regresar una lista de una tabla pero solo siertos campos que tenemos en un modelo que se guarde los datos en el modelo automaticamente on una linea, ejemplo en userscontroller
            services.AddTransient<seed>();
            services.AddScoped<IAuthRepository, AuthRepository>();//el scoped es un servicio que sirve para el authrepostorycpueda mandar a llamar el iauthrepository pero solo una vez por cada http request
            //Este de abajo sirve para autenticar con el token para entrar a controladores
            services.AddScoped<IDatingRepository, DatingRepository>(); // sirve como atajo para agregar, borrar usuarios, consultar usuario y usuarios
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
                    .GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddScoped<LogUserActivity>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, seed seeder) // el seeder es un metodo para agregar registros  base de datos
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // Este app useexceptionhandler sirve para el manejo de errores, en lugar de usar mucho try catch este funciona globalmente
                // Solo funciona cuando el proyecto esta en produccion desde el launchSettings.json de properties               
                app.UseExceptionHandler(builder =>{
                    builder.Run(async context => { 
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            context.Response.AddApplicationError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message);
                        }
                    });
                });
                //app.UseHsts();
            }

            //app.UseHttpsRedirection();
            //seeder.SeedUsers(); // metodo que utiliza un archivo json para cargar 10 usuarios con foto
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());//siempre va antes que el de mvc
            app.UseAuthentication();//para aplicar la autorizacion y codigo
            app.UseMvc();
        }
    }
}
