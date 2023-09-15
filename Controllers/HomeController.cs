using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using inmobiliaria.Models;

namespace inmobiliaria.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
         ViewBag.Id = TempData["Id"];
            if(TempData.ContainsKey("Mensaje"))
            {
                ViewBag.Mensaje = TempData["Mensaje"];

            }
        return View();
    }

    public IActionResult Privacy()
    {
         
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
         
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
//controlador
//dotnet-aspnet-codegenerator controller -name "PagosController" -outDir "Controllers" -namespace "inmobiliaria.Controllers" -f -actions
//vistas
//dotnet-aspnet-codegenerator view Index List -outDir "Views/Pagos" -udl --model inmobiliaria.Models.Pagos -f
//dotnet-aspnet-codegenerator view Create Create -outDir "Views/Pagos" -udl --model inmobiliaria.Models.Pagos -f
//dotnet-aspnet-codegenerator view Edit Edit -outDir "Views/Pagos" -udl --model inmobiliaria.Models.Pagos -f
//dotnet-aspnet-codegenerator view Details Details -outDir "Views/Pagos" -udl --model inmobiliaria.Models.Pagos -f
//dotnet-aspnet-codegenerator view Delete Delete -outDir "Views/Pagos" -udl --model inmobiliaria.Models.Pagos -f