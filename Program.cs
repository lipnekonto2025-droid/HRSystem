using HRSystem.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<HRContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HRContext")));

var app = builder.Build();

// Tworzy bazę z klas (Code First) i wypełnia ją danymi przykładowymi.
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<HRContext>();
    context.Database.EnsureCreated();
    DbInitializer.Seed(context);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRequestLocalization("pl-PL"); // przecinek jako separator dziesiętny
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
