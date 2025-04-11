namespace WordWise.Api.Models.Dto.User
{
    public class InforUserDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public bool Gender { get; set; }
        public int Level { get; set; }
        public string PhoneNumber { get; set; }
        public bool EmailConfirm { get; set; }
        public List<string> Roles { get; set; }
    }
}
