using IntranetPortal.UserProfiles.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.UserProfiles
{
    public interface IUserProfileAppService
    {
        Task<List<UserProfileDto>> GetUserProfileAsync();
        Task<UserProfileDto> GetUserProfileByIdAsync(Guid id);
    }
}
