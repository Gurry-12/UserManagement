using UserManagement.Application.Dtos;

namespace UserManagement.Application.Services.AbstractServices
{
    public interface IUserService
    {
        Task AddUserAsync(AddUpdateUserDto user);
        Task<List<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(int id);
        Task UpdateUserAsync(AddUpdateUserDto user);
        Task DeleteUserAsync(int id);

        Task<UserSummaryDto> GetSummaryAsync();
        Task UpdateUserActionAsync(UpdateActionDto updateActionDto);
        Task UpdateUserRoleAsync(UpdateRolesDto updateRoleDto);
        Task UpdateUserStatusAsync(UpdateStatusDto updateStatusDto);


    }
}
