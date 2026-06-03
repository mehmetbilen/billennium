using AspNetStatic;
using BillenniumWeb.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents();

var staticResources = new List<ResourceInfoBase>
{
    // Keep your Razor pages mapped manually
    new PageResource("/"),
    new PageResource("/about-us"),
    new PageResource("/capabilities"),
    new PageResource("/architect"),
    new PageResource("/initialize"),
};

var webRootPath = builder.Environment.WebRootPath;
var allFiles = Directory.GetFiles(webRootPath, "*.*", SearchOption.AllDirectories);
foreach (var filePath in allFiles)
{
    // Convert physical path (C:\...\wwwroot\css\app.css) to web route (/css/app.css)
    var relativePath = filePath.Replace(webRootPath, "").Replace("\\", "/");
    staticResources.Add(new BinResource(relativePath));
}

builder.Services.AddSingleton<IStaticResourcesInfoProvider>(
    new StaticResourcesInfoProvider(staticResources)
);

var app = builder.Build();

app.UseStaticFiles();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>();

// Generate static files when the app is launched with the "ssg" argument.
//   dotnet run -- ssg
// Output folder: <project>/output
if (args.HasSsgArg())
{
    app.GenerateStaticContent(
        Path.Combine(app.Environment.ContentRootPath, "output"),
        exitWhenDone: true,
        alwaysDefaultFile: true);
}

app.Run();
