using Microsoft.AspNetCore.Mvc;
using WordWise.Api.Models.Dto.User;

namespace WordWise.Api.Repositories.Interface
{
    public interface IAuthRepository
    {
        Task<ListUserDto> GetAllUserAsync(string? emailUser, string? roleFilter, int currentPage = 1, int itemPerPage = 20);
    }
}
