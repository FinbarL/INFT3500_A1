using INFT3500.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SendGrid;
using SendGrid.Helpers.Mail;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie("Cookies", config =>
    {
        config.Cookie.Name = "INFT3500.Cookie";
        config.LoginPath = "/Account/Login";
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole",
        policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireCustomerRole",
        policy => policy.RequireRole("Customer"));
    options.AddPolicy("RequireStaffRole",
        policy => policy.RequireRole("Staff"));
});
builder.Services.AddControllersWithViews();
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});

builder.Services.AddDbContext<StoreDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration["ConnectionStrings:StoreDbContextConnection"]);
});
builder.Services.AddSession();
var apiKey = "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";
var client = new SendGridClient(apiKey);
var msg = new SendGridMessage()
{
    From = new EmailAddress("c3331609@uon.edu.au", "Finbar Laffan"),
    Subject = "Sending with Twillio Sendgrid",
    PlainTextContent = "Hello World",
};
msg.AddTo(new EmailAddress("shadyswords@gmail.com", "Joe Bloggs"));
var response = await client.SendEmailAsync(msg);
Console.WriteLine(response.IsSuccessStatusCode ? "Email queued successfully!" : "Something went wrong!");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
} else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();