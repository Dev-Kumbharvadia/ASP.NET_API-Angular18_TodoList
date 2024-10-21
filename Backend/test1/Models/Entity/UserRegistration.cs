namespace test1.Models
{
    public class UserRegisterModel
    {
        public string Username { get; set; }         // Username provided during registration
        public string Password { get; set; }         // Raw password provided by the user
        public string Email { get; set; }            // Email provided by the user
    }
}
