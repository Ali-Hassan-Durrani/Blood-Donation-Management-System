using BloodDonationApp.Components;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<SeekerService>();
builder.Services.AddScoped<DonorService>();
builder.Services.AddScoped<BloodBankService>();
builder.Services.AddScoped<HospitalService>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<RequestBloodService>();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<InventoryService>();
builder.Services.AddScoped<DonationRequestService>();
builder.Services.AddScoped<DonationService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddBlazoredSessionStorage();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
