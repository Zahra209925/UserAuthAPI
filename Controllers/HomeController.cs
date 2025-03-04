using Microsoft.AspNetCore.Mvc;

namespace UserAuthAPI.Controllers
{
	[ApiController]
	[Route("/")]
	public class HomeController : ControllerBase
	{
		[HttpGet]
		public IActionResult Index()
		{
			return Ok("Welcome to the UserAuthAPI! Use /api/user for API endpoints.");
		}
	}
}