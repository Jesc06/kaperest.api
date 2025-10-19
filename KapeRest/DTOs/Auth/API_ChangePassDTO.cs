namespace KapeRest.DTOs.Auth
{
    public class API_ChangePass
    {
        public string Email { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
