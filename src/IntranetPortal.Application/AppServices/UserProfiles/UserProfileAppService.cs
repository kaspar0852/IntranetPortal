using IntranetPortal.AppEntities;
using IntranetPortal.AppEntities.UserProfiles;
using IntranetPortal.InternalApplication;
using IntranetPortal.UserProfiles.Dtos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Uow;
using Volo.Abp.Users;

namespace IntranetPortal.AppServices.UserProfiles
{
    public class UserProfileAppService : IntranetPortalAppService
    {
        private readonly IRepository<UserProfileStatus, Guid> _userStatusRepository;
        private readonly IRepository<UserProfile, Guid> _userProfileRepository;
        private readonly IRepository<IdentityUser, Guid> _identityUserRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<Department, Guid> _departmentRepository;
        private readonly IRepository<Designation, Guid> _designationRepository;

        public UserProfileAppService(
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<UserProfileStatus, Guid> userStatusRepository,
            IRepository<UserProfile, Guid> userProfileRepository, 
            IRepository<IdentityUser, Guid> identityUserRepository, 
            IRepository<Department, Guid> departmentRepository, 
            IRepository<Designation, Guid> designationRepository)
        {
            _userStatusRepository = userStatusRepository;
            _userProfileRepository = userProfileRepository;
            _identityUserRepository = identityUserRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _departmentRepository = departmentRepository;
            _designationRepository = designationRepository;
        }

        public async Task<List<UserProfileDto>> GetUserProfileAsync()
        {
            try
            {
                Logger.LogInformation($"GetUserProfile requested by User:{CurrentUser.Id}");
                Logger.LogDebug($"GetUserProfile requested for DocumentStatuses:{CurrentUser.Id}");

                using (var uow = _unitOfWorkManager.Begin())
                {
                    var identityUserQuery = await _identityUserRepository.GetQueryableAsync();
                    var departmentQuery = await _departmentRepository.GetQueryableAsync();
                    var designationQuery = await _designationRepository.GetQueryableAsync();
                    var userProfileQuery = await _userProfileRepository.GetQueryableAsync();

                    var currentUserId = CurrentUser.Id;

                    var currentUser = await _identityUserRepository.FindAsync(x => x.Id == currentUserId);

                    var query = from user in userProfileQuery
                                join identityUser in identityUserQuery
                                on user.AbpUserId equals identityUser.Id

                                join department in departmentQuery
                                on user.DepartmentId equals department.Id

                                join designation in designationQuery
                                on user.DesignationId equals designation.Id
                                where currentUserId == user.AbpUserId
                                select new
                                {
                                    user.Id,
                                    user.AbpUserId,
                                    identityUser.UserName,
                                    identityUser.Name,
                                    identityUser.Email,
                                    user.MiddleName,
                                    identityUser.Surname,
                                    DesignationName = designation.Name,
                                    user.DesignationId,
                                    DepartmentName = department.Name,
                                    user.DepartmentId,
                                    user.ProfilePictureUrl,
                                    user.HiredDate,
                                    user.DateOfBirth,
                                    user.CreationTime,
                                };
                    var queryResultDto = query.Select(x => new UserProfileDto
                    {
                        Id = x.Id,
                        AbpUserId = x.AbpUserId,
                        UserName = x.UserName,
                        Name = x.Name,
                        MiddleName = x.MiddleName,
                        Email = x.Email,
                        SurName = x.Surname,
                        DesignationId = x.DesignationId,
                        DesignationName = x.DesignationName,
                        DepartmentId = x.DepartmentId,
                        DepartmentName = x.DepartmentName,
                        ProfilePicutreUrl = x.ProfilePictureUrl,
                        HiredDate = x.HiredDate,
                        DateOfBirth = x.DateOfBirth,
                        CreationTime = x.CreationTime
                    }).ToList();
                    await uow.CompleteAsync();
                    Logger.LogInformation($"GetUserProfile responded for User:{CurrentUser.Id}");

                    return queryResultDto;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(GetUserProfileAsync));
                throw new UserFriendlyException($"An exception was caught. {ex}");
            }
        }

        public async Task<UserProfileDto> GetUserProfileByIdAsync(Guid id)
        {
            try
            {
                Logger.LogInformation($"GetUserProfileById requested by User:{CurrentUser.Id}");
                Logger.LogDebug($"GetUserProfileById requested for Document:{(CurrentUser.Id, id)}");

                using (var uow = _unitOfWorkManager.Begin())
                {
                    var identityUserQuery = await _identityUserRepository.GetQueryableAsync();
                    var departmentQuery = await _departmentRepository.GetQueryableAsync();
                    var designationQuery = await _designationRepository.GetQueryableAsync();
                    var userProfileQuery = await _userProfileRepository.GetQueryableAsync();

                    if (await _identityUserRepository.FindAsync(id) == null)
                    {
                        throw new UserFriendlyException("User not found", code: "400");
                    }

                    var query = from user in userProfileQuery
                                join identityUser in identityUserQuery
                                on user.AbpUserId equals identityUser.Id

                                join department in departmentQuery
                                on user.DepartmentId equals department.Id

                                join designation in designationQuery
                                on user.DesignationId equals designation.Id
                                where identityUser.Id == id
                                select new
                                {
                                    user.Id,
                                    user.AbpUserId,
                                    identityUser.UserName,
                                    identityUser.Name,
                                    identityUser.Email,
                                    user.MiddleName,
                                    identityUser.Surname,
                                    DesignationName = designation.Name,
                                    user.DesignationId,
                                    DepartmentName = department.Name,
                                    user.DepartmentId,
                                    user.ProfilePictureUrl,
                                    user.HiredDate, 
                                    user.DateOfBirth,
                                    user.CreationTime,
                                };
                    var queryResultDto = query.Select(x => new UserProfileDto
                    {
                        Id = x.Id,
                        AbpUserId = x.AbpUserId,
                        UserName = x.UserName,
                        Name = x.Name,
                        MiddleName = x.MiddleName,
                        Email = x.Email,
                        SurName = x.Surname,
                        DesignationId = x.DesignationId,
                        DesignationName = x.DesignationName,
                        DepartmentId = x.DepartmentId,
                        DepartmentName = x.DepartmentName,
                        ProfilePicutreUrl = x.ProfilePictureUrl,
                        HiredDate = x.HiredDate,
                        DateOfBirth = x.DateOfBirth,
                        CreationTime = x.CreationTime
                    }).FirstOrDefault();

                    await uow.CompleteAsync();
                    Logger.LogInformation($"GetUserProfileById responded for User:{CurrentUser.Id}");

                    return queryResultDto;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(GetUserProfileByIdAsync));
                throw new UserFriendlyException($"An exception was caught. {ex}");
            }
        }
    }
}
