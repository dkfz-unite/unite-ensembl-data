using Microsoft.AspNetCore.Mvc;

namespace Ensembl.Data.Web.Controllers;

[Route("api/")]
public class DefaultController : Controller
{
	[HttpGet]
	public IActionResult Get()
	{
		var date = DateTime.Now;

		return Json(date);
	}
}
