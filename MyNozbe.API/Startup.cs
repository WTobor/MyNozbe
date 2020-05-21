using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MyNozbe.Database;
using MyNozbe.Database.Repositories;
using MyNozbe.Domain.Interfaces;
using MyNozbe.Domain.Models;
using MyNozbe.Domain.Services;
using MyNozbe.Domain.Validators;

namespace MyNozbe.API
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
            services
                .AddDbContext<DatabaseContext>(options =>
                    options.UseInMemoryDatabase("myNozbeDatabase"));

            services.AddScoped<DatabaseContext>();
            services.AddScoped<IDbOperations<TaskModel>, TaskRepository>();
            services.AddScoped<IDbOperations<ProjectModel>, ProjectRepository>();
            services.AddScoped<TaskService>();
            services.AddScoped<ProjectService>();

            services.AddControllers()
                //solution based on https://dotnetcoretutorials.com/2020/03/15/fixing-json-self-referencing-loop-exceptions/
                .AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
                .AddFluentValidation(fv =>
                {
                    fv.RegisterValidatorsFromAssemblyContaining<TaskModelValidator>();
                    fv.RegisterValidatorsFromAssemblyContaining<ProjectModelValidator>();
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyNozbe API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app.UseSwagger();

            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyNozbe API V1"); });
        }
    }
}