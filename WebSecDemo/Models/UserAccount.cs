namespace WebSecDemo.Models
{
    public class UserAccount
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
    }


    public class UserAccountHashed
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
    }

}