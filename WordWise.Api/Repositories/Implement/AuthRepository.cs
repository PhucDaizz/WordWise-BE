using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WordWise.Api.Data;
using WordWise.Api.Models.Domain;
using WordWise.Api.Models.Dto.User;
using WordWise.Api.Repositories.Interface;

namespace WordWise.Api.Repositories.Implement
{
    public class AuthRepository : IAuthRepository
    {
        private readonly WordWiseDbContext _dbContext;
        private readonly UserManager<ExtendedIdentityUser> _userManager;

        public AuthRepository(WordWiseDbContext dbContext, UserManager<ExtendedIdentityUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }
        public async Task<ListUserDto> GetAllUserAsync(
            string? emailUser,
            string? roleFilter,
            int currentPage = 1,
            int itemPerPage = 20)
        {
            var query = _dbContext.ExtendedIdentityUsers.AsQueryable();

            if (currentPage <= 0 || itemPerPage <= 0)
                throw new ArgumentException("Invalid pagination parameters.");

            // Lọc theo Email
            if (!string.IsNullOrEmpty(emailUser))
                query = query.Where(x => x.Email.Contains(emailUser));

            // Lọc theo Role (Admin/User)
            if (!string.IsNullOrEmpty(roleFilter))
            {
                var userIdsInRole = await _dbContext.UserRoles
                    .Join(_dbContext.Roles,
                        ur => ur.RoleId,
                        r => r.Id,
                        (ur, r) => new { ur.UserId, RoleName = r.Name })
                    .Where(x => x.RoleName == roleFilter)
                    .Select(x => x.UserId)
                    .ToListAsync();

                query = query.Where(u => userIdsInRole.Contains(u.Id));
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / itemPerPage);

            var users = await query
                .Skip((currentPage - 1) * itemPerPage)
                .Take(itemPerPage)
                .ToListAsync();

            var items = new List<InforUserDto>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                items.Add(new InforUserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    Gender = user.Gender,
                    Level = (int)user.Level,
                    PhoneNumber = user.PhoneNumber,
                    EmailConfirm = user.EmailConfirmed,
                    Roles = roles.ToList()
                });
            }

            return new ListUserDto
            {
                InforUsers = items,
                CurentPage = currentPage,
                ItemPerPage = itemPerPage,
                TotalPage = totalPages
            };
        }
    }
}
