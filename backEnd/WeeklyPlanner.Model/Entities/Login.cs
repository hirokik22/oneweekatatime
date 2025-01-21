namespace WeeklyPlanner.Model.Entities
{
    public class Login
    {
        public int LoginId { get; set; }  // Corresponds to 'loginid' in the db
        public string Email { get; set; }  // Corresponds to 'email' in the db
        public string PasswordHash { get; set; }  // Corresponds to 'passwordhash' in the db
    }
}
