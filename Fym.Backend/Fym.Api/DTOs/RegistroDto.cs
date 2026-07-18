namespace Fym.Api.DTOs
{
    public class RegistroDto
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}