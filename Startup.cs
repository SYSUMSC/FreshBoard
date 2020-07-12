using System;
using System.Linq;
using FreshBoard.Data;
using FreshBoard.Data.Identity;
using FreshBoard.Hubs;
using FreshBoard.Middlewares;
using FreshBoard.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FreshBoard
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<BrotliCompressionProvider>();
                options.EnableForHttps = true;
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "image/svg+xml" });
            });

            // services.Configure<CookiePolicyOptions>(options =>
            // {
            //     // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            //     options.CheckConsentNeeded = context => true;
            //     options.MinimumSameSitePolicy = SameSiteMode.None;
            // });

            services.AddDbContext<FreshBoardDbContext>(options =>
            {
                options.UseLazyLoadingProxies();
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddAuthentication(o =>
            {
                o.DefaultScheme = IdentityConstants.ApplicationScheme;
                o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies();

            services.AddIdentityCore<FreshBoardUser>(o =>
            {
                o.Password.RequireNonAlphanumeric = false;
                o.Stores.MaxLengthForKeys = 128;
                o.User.RequireUniqueEmail = true;
            })
            .AddSignInManager()
            .AddUserManager<UserManager<FreshBoardUser>>()
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<FreshBoardDbContext>()
            .AddDefaultTokenProviders()
            .AddErrorDescriber<TranslatedIdentityErrorDescriber>();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/SignIn";
                options.LogoutPath = "/SignOut";
            });

            services.AddEntityFrameworkSqlite();

            services.AddTransient<IEmailSender, EmailSender>()
                .Configure<EmailOptions>(Configuration.GetSection("Email"));

            services.AddTransient<ISmsSender, SmsSender>()
                .Configure<SmsOptions>(Configuration.GetSection("Sms"));
            services.AddScoped<IPuzzleService, PuzzleService>();
            services.AddSession();
            services.AddScoped<WebSocketPuzzle>();

            var mvcBuilder = services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest);
#if DEBUG
            if (Environment.IsDevelopment())
                mvcBuilder.AddRazorRuntimeCompilation();
#endif

            services.AddSignalR();

            services.AddControllersWithViews().AddRazorRuntimeCompilation();

            services.AddTransient<GitService>()
                .Configure<GitTaskOptions>(options =>
                {
                    options.GitOrg = "SYSUMSC";
                    options.GitRepo = "Blogs";
                });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/Error", "?code={0}");

            app.Use(async (context, next) =>
            {
                if (!context.Request.Path.StartsWithSegments("/Hackathon", StringComparison.CurrentCultureIgnoreCase))
                    await next();
            });

            app.UseResponseCompression();

            //app.UseLiveReload();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseCookiePolicy();
            app.UseSession();
            app.UseWebSockets();

            app.UseWebSocketPuzzle();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
                {
                    endpoints.MapDefaultControllerRoute();
                    endpoints.MapHub<ChatHub>("/ChatHub");
                });
        }
    }
}
