namespace SaleOnline.Models
{
    public class LoginInfo
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public LoginInfo()
        {
            Email = "";
            Password = "";
        }
    }
}
