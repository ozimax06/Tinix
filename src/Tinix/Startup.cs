using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tinix.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.StaticFiles;

namespace Tinix
{
	public class Startup
	{
		public IConfiguration Configuration { get; set;}
		public Startup(IConfiguration configuration)
		{
		
			Configuration = configuration;
		}

		

		// This method gets called by the runtime. Use this method to add services to the container.
		//get everything ready, load the stuff
		public void ConfigureServices(IServiceCollection services)
		{
		
			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
				.AddCookie(options =>
				{
					options.LoginPath = "/Admin/LogIn";
					options.LogoutPath = "/Admin/LogOff";
				});

			services.AddMvc();
			services.AddMemoryCache();
			services.AddSingleton<IBlog, FileBlogService>();
			services.AddSingleton<IAdminService, AdminService>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		//configure stuff
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory )
		{

			loggerFactory.AddFile("Logs/tinix-{Date}.log");


			app.UseAuthentication();

			

			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json");

		

			var config = builder.Build();

			ApplicationContext.PostsPerPage = config.GetSection("Blog").GetValue<int>("PostsPerPage");
			ApplicationContext.Title = config.GetSection("Blog").GetValue<string>("Title");
			ApplicationContext.Subtitle = config.GetSection("Blog").GetValue<string>("Subtitle");
			ApplicationContext.Layout = config.GetSection("Blog").GetValue<string>("Layout");

			AdministratorContext.Salt = config.GetSection("Admin").GetValue<string>("Salt");
			AdministratorContext.Username = config.GetSection("Admin").GetValue<string>("Username");
			AdministratorContext.Hash = config.GetSection("Admin").GetValue<string>("Hash");



			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseBrowserLink();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}

			var provider = new FileExtensionContentTypeProvider();
			provider.Mappings.Remove(".xml");

			//app.UseStaticFiles();

			app.UseStaticFiles(new StaticFileOptions()
			{
				ContentTypeProvider = provider
			});


			app.UseMvc(routes =>
			{
				routes.MapRoute(
					"default",
					"{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}