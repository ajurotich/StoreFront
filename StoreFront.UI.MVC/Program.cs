using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StoreFront.Data.EF.Models;
using StoreFront.UI.MVC.Data;

namespace StoreFront.UI.MVC;

public class Program {
	public static void Main(string[] args) {
		var builder = WebApplication.CreateBuilder(args);

		// Add services to the container.
		var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
		builder.Services.AddDbContext<ApplicationDbContext>(options =>
			options.UseSqlServer(connectionString));
		builder.Services.AddDbContext<McstoreContext>(options =>
			options.UseSqlServer(connectionString));
		
		builder.Services.AddDatabaseDeveloperPageExceptionFilter();

		builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
			.AddRoles<IdentityRole>()
			.AddRoleManager<RoleManager<IdentityRole>>()
			.AddEntityFrameworkStores<ApplicationDbContext>();
		builder.Services.AddControllersWithViews();

		//register session for shopping cart
		builder.Services.AddSession(
			options => {
				options.IdleTimeout = TimeSpan.FromMinutes(10);// duration a session is stored in memory (default is 20min)
				options.Cookie.HttpOnly = true; // Allows us to set cookie options over nonHTTPS secure connections
				options.Cookie.IsEssential = true; // cannot be declined for session to work
			}
		);

		var app = builder.Build();

		// Configure the HTTP request pipeline.
		if(app.Environment.IsDevelopment()) {
			app.UseMigrationsEndPoint();
		}
		else {
			app.UseExceptionHandler("/Home/Error");
			// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
			app.UseHsts();
		}

		app.UseHttpsRedirection();
		app.UseStaticFiles();

		app.UseRouting();

		//this line below MUST come AFTER UseRouting and BEFORE UseAuthentication
		app.UseSession();// implement the session service in our app

		app.UseAuthorization();

		app.MapControllerRoute(
			name: "default",
			pattern: "{controller=Home}/{action=Index}/{id?}");
		app.MapRazorPages();

		app.Run();
	}
}
