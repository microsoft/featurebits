// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using FeatureBits.Core;
using FeatureBits.Data.EF;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace FeatureBits.Data.WebApi.Test
{
    public class Startup
    {
        private const string FeatureBitsDbConnectionStringKey = "FeatureBitsDbContext";
        private IHostingEnvironment EnvironmentHost { get; }
        public IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment environment)
        {
            EnvironmentHost = environment;
            Configuration = BuildConfiguration();
        }

        /// <summary>
        /// Build configuration from AppSettings, Environment Variables, Azure Key Valut and (User Secrets - DEV only).
        /// </summary>
        /// <returns><see cref="IConfiguration"/></returns>
        private IConfigurationRoot BuildConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .AddUserSecrets(typeof(Startup).Assembly);

            if (EnvironmentHost.IsDevelopment())
            {
                // Re-add User secrets so it takes precedent for local development
                configurationBuilder.AddUserSecrets(typeof(Startup).Assembly);
            }

            return configurationBuilder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
#if !NET452
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
#else
            services.AddMvc();
#endif
            string featureBitsConnectionString = Configuration.GetConnectionString(FeatureBitsDbConnectionStringKey);
            services.AddDbContext<FeatureBitsEfDbContext>(options => options.UseSqlServer(featureBitsConnectionString));
            services.AddTransient<IFeatureBitsRepo, FeatureBitsEfRepo>((serviceProvider) =>
            {
                DbContextOptionsBuilder<FeatureBitsEfDbContext> options = new DbContextOptionsBuilder<FeatureBitsEfDbContext>();
                options.UseSqlServer(featureBitsConnectionString);
                var context = new FeatureBitsEfDbContext(options.Options);
                return new FeatureBitsEfRepo(context);
            });
            services.AddTransient<IFeatureBitEvaluator, FeatureBitEvaluator>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
