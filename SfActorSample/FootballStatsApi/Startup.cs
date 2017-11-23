using System;
using AutoMapper;
using FootballStatsApi.Common.Contracts;
using FootballStatsApi.Dal.Common.Dto;
using FootballStatsApi.Dal.Common.Repositories;
using FootballStatsApi.Dal.SqlServer.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;

namespace FootballStatsApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info
                {
                    Title = "Football Stats API",
                    Version = "v1",
                    Contact = new Contact
                    {
                        Email = "apisupport@techniqly.com",
                        Name = "API Support",
                        Url = "https://github.com/farooqam/sf-actors"
                    }
                });
            });

            services.TryAddSingleton(provider =>
            {
                return new MapperConfiguration(config =>
                    {
                        config.CreateMap<TeamStatsDto, GetTeamStatsResponse>();
                        config.CreateMap<PutTeamStatsRequest, TeamStatsDto>();
                    })
                    .CreateMapper();
            });

            var sqlRepositorySettingsConfigSection = Configuration.GetSection("FootballStatsApi.Dal.SqlServer");

            var sqlRepositorySettings = new TeamStatsRepositorySettings
            {
                ConnectionString = sqlRepositorySettingsConfigSection["connectionString"],
                QueryTimeout = TimeSpan.Parse(sqlRepositorySettingsConfigSection["queryTimeout"])
            };

            services.TryAddSingleton(typeof(TeamStatsRepositorySettings), provider => sqlRepositorySettings);
            services.TryAddScoped<ITeamStatsRepository, TeamStatsRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Football Stats API V1"));
            app.UseMvc();
        }
    }
}