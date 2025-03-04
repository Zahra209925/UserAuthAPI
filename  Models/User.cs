namespace UserAuthAPI.Models
{
	public class User
	{
		public string UserName { get; set; } = string.Empty; // Ei-nullattava oletusarvolla
		public string Password { get; set; } = string.Empty; // Ei-nullattava oletusarvolla
		public bool IsLoggedIn { get; set; } = false; // Kirjautumistila
		public bool IsLocked { get; set; } = false; // Lukitustila
	}
}