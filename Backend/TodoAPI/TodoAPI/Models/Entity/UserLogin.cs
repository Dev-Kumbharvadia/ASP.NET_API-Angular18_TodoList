namespace TodoAPI.Models
{
    public class UserLoginModel
    {
        public string Username { get; set; }         // Username provided during login
        public string Password { get; set; }         // Raw password provided by the user
    }
}
