using myProject.Interfaces;
using myProject;
using myProject.Services;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// register all project services (ice cream, user, active-user, SignalR)
builder.Services.AddProjectServices();

// Add authentication
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(cfg =>
    {
        cfg.RequireHttpsMetadata = false;
        cfg.TokenValidationParameters = UserTokenService.GetTokenValidationParameters();

        // allow SignalR websocket connections to send access_token as query string
        cfg.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"].ToString();
                var path = context.HttpContext?.Request?.Path;
                var pathValue = path.HasValue ? path.Value.ToString() : string.Empty;
                if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(pathValue) && pathValue.StartsWith("/activityHub", System.StringComparison.OrdinalIgnoreCase))
                {
                    context.Token = accessToken;
                }
                return System.Threading.Tasks.Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(cfg =>
    {
        cfg.AddPolicy("AllUsers", policy => policy.RequireClaim("usertype", "Admin", "Agent", "User"));
        cfg.AddPolicy("Admin", policy => policy.RequireClaim("usertype", "Admin"));
        cfg.AddPolicy("Agent", policy => policy.RequireClaim("usertype", "Agent"));
        cfg.AddPolicy("User", policy => policy.RequireClaim("usertype", "User"));
        // cfg.AddPolicy("ClearanceLevel1", policy => policy.RequireClaim("ClearanceLevel", "1", "2")
        //     || policy.RequireClaim("usertype", "Admin")
        // );
        cfg.AddPolicy("ClearanceLevel2", policy => policy.RequireClaim("ClearanceLevel", "2"));
    });
builder.Services.AddHttpContextAccessor();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "myProject", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
            { new OpenApiSecurityScheme
                    {
                     Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer"}
                    },
                new string[] {}
            }
    });
});

// UseActiveUser is already called by AddProjectServices()

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    });
}
app.UseDefaultFiles(new DefaultFilesOptions
{
    DefaultFileNames = new List<string> { "login.html" }
});
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Map ActivityHub at /activityHub (KS Pizza uses /activityHub)
app.MapHub<myProject.Hubs.ActivityHub>("/activityHub");

app.Run();
