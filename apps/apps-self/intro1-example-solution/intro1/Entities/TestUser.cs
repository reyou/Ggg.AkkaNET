namespace intro1.Entities
{
    public class TestUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public TestUser(string userName, string password, string email)
        {
            this.Username = userName;
            this.Password = password;
            this.Email = email;
        }


    }
}