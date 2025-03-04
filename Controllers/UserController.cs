using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;


namespace UserAuthAPI.Controllers
{
	[ApiController]
	[Route("api/user")]
	public class UserController : ControllerBase
	{
		// In-memory tietokanta käyttäjille
		private static readonly Dictionary<string, (string PasswordHash, bool IsLoggedIn, bool IsLocked)> Users = new();

		/// <summary>
		/// Rekisteröi uusi käyttäjä.
		/// </summary>
		[HttpPost("register")]
		public IActionResult Register([FromBody] User user)
		{
			// Tarkista, että käyttäjätunnus ja salasana eivät ole tyhjiä
			if (string.IsNullOrWhiteSpace(user.UserName) || string.IsNullOrWhiteSpace(user.Password))
				return BadRequest("Käyttäjätunnus ja salasana eivät voi olla tyhjiä.");

			// Tarkista, onko käyttäjä jo olemassa
			if (Users.ContainsKey(user.UserName))
				return BadRequest("Käyttäjä on jo olemassa.");

			// Hashaa salasana ja tallenna käyttäjä
			string passwordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);
			Users[user.UserName] = (passwordHash, false, false);

			return Ok("Käyttäjä rekisteröity onnistuneesti.");
		}

		/// <summary>
		/// Kirjaa käyttäjän sisään.
		/// </summary>
		[HttpPost("login")]
		public IActionResult Login([FromBody] User loginRequest)
		{
			// Tarkista, löytyykö käyttäjä
			if (!Users.ContainsKey(loginRequest.UserName))
				return NotFound("Käyttäjää ei löydy.");

			var user = Users[loginRequest.UserName];

			// Tarkista, onko käyttäjä lukittu
			if (user.IsLocked)
				return Unauthorized("Käyttäjä on lukittu.");

			// Tarkista, onko salasana oikein
			if (!BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.PasswordHash))
				return Unauthorized("Virheellinen salasana.");

			// Päivitä kirjautumistila
			Users[loginRequest.UserName] = (user.PasswordHash, true, user.IsLocked);
			return Ok("Kirjautuminen onnistui.");
		}

		/// <summary>
		/// Tarkista, onko käyttäjä kirjautunut sisään.
		/// </summary>
		[HttpGet("isLoggedIn/{username}")]
		public IActionResult IsLoggedIn(string username)
		{
			// Tarkista, löytyykö käyttäjä
			if (!Users.ContainsKey(username))
				return NotFound("Käyttäjää ei löydy.");

			return Ok(new { IsLoggedIn = Users[username].IsLoggedIn });
		}

		/// <summary>
		/// Hae kaikki käyttäjät.
		/// </summary>
		[HttpGet("all")]
		public IActionResult GetAllUsers()
		{
			// Palauta käyttäjät ilman salasanoja
			return Ok(Users.Select(u => new { UserName = u.Key, u.Value.IsLoggedIn, u.Value.IsLocked }));
		}

		/// <summary>
		/// Lukitse käyttäjä.
		/// </summary>
		[HttpPost("lock/{username}")]
		public IActionResult LockUser(string username)
		{
			// Tarkista, löytyykö käyttäjä
			if (!Users.ContainsKey(username))
				return NotFound("Käyttäjää ei löydy.");

			var user = Users[username];

			// Lukitse käyttäjä ja kirjaa ulos
			Users[username] = (user.PasswordHash, false, true);
			return Ok("Käyttäjä lukittu onnistuneesti.");
		}
	}

	/// <summary>
	/// Käyttäjämalli.
	/// </summary>
	public class User
	{
		public string UserName { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
	}
}

