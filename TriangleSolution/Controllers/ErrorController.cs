using Microsoft.AspNetCore.Mvc;
using Triangles.Models;

namespace Triangles.Controllers;

public class ErrorController : Controller
{
    [Route("error")]
    public IActionResult Index()
    {
        var view = View("Error", new ErrorViewModel("An unexpected error occurred."));
        view.StatusCode = StatusCodes.Status500InternalServerError;
        return view;
    }
}
