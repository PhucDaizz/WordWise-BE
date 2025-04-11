namespace WordWise.Api.Models.Dto.User
{
    public class ListUserDto
    {
        public IEnumerable<InforUserDto> InforUsers { get; set; }
        public int CurentPage { get; set; }
        public int ItemPerPage { get; set; }
        public int TotalPage { get; set; }
    }
}
