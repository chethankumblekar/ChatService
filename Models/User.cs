namespace PlayGround.ChatService.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string OAuthSubject { get; set; }
        public string OAuthIssuer { get; set; }
    }
}