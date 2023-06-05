using IntranetPortal.AppEntities;
using IntranetPortal.AppEntities.UserProfiles;
using IntranetPortal.Employees;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace IntranetPortal.AppServices.Employees
{
    public class SearchEmployeeAppService : IntranetPortalAppService
    {
        private readonly ICurrentUser _currentUser;
        private readonly IRepository<Department, Guid> _departmentRepository;
        private readonly IRepository<UserProfile, Guid> _userProfileRepository;
        private readonly IRepository<Designation, Guid> _designationRepositroy;
        private readonly IRepository<IdentityUser, Guid> _identityUserRepository;

        public SearchEmployeeAppService(
            IRepository<IdentityUser, Guid> identityUserRepository,
            IRepository<Department, Guid> departmentRepository,
            IRepository<UserProfile, Guid> userProfileRepository, 
            IRepository<Designation, Guid> designationRepositroy)
        {
            _identityUserRepository = identityUserRepository;
            _departmentRepository = departmentRepository;
            _userProfileRepository = userProfileRepository;
            _designationRepositroy = designationRepositroy;
        }

        public async Task<List<EmployeeDto>> SearchEmployeeAsync(SearchEmployeeDto input)
        {

            Logger.LogInformation($"SearchEmployee requested by User:{CurrentUser.Id}");
            Logger.LogDebug($"SearchEmployee requested for Document:{(CurrentUser.Id, input)}");
            try
            {
                var userQuery = await _identityUserRepository.GetQueryableAsync();
                var userProfileQuery = await _userProfileRepository.GetQueryableAsync();
                var departmentQuery = await _departmentRepository.GetQueryableAsync();
                var designationsQuery = await _designationRepositroy.GetQueryableAsync();

                var employees = from abpuser in userQuery
                                join userProfile in userProfileQuery on abpuser.Id equals userProfile.AbpUserId into userProfiles
                                from userProfile in userProfiles.DefaultIfEmpty()
                                join department in departmentQuery on userProfile.DepartmentId equals department.Id into departments
                                from department in departments.DefaultIfEmpty()
                                join designation in designationsQuery on userProfile.DesignationId equals designation.Id into designations
                                from designation in designations.DefaultIfEmpty()
                                select new EmployeeDto
                                {
                                    AbpUserId = abpuser.Id,
                                    UserName = abpuser.UserName,
                                    Name = abpuser.Name,
                                    MiddleName = userProfile.MiddleName ?? string.Empty,
                                    SurName = abpuser.Surname,
                                    FullName = userProfile == null ? null
                                            : (string.IsNullOrEmpty(userProfile.MiddleName)
                                            ? $"{abpuser.Name} {abpuser.Surname}"
                                            : $"{abpuser.Name} {userProfile.MiddleName} {abpuser.Surname}"),
                                    DesignationName = designation.Name,
                                    DesignationId = designation.Id,
                                    DepartmentName = department.Name,
                                    DepartmentId = department.Id,
                                    ProfilePictureUrl = userProfile.ProfilePictureUrl,
                                    Email = abpuser.Email,
                                    PhoneNumber = abpuser.PhoneNumber ?? string.Empty,
                                    HiredDate = userProfile.CreationTime,
                                };

                if (string.IsNullOrWhiteSpace(input.SearchKeyword))
                {
                    //This returns whole list of employess
                    return employees.ToList();
                }
                else
                {
                    var normalizedSearchText = input.SearchKeyword.ToLower();

                    employees = employees.Where(emp =>
                        emp.Name.ToLower().Contains(normalizedSearchText)
                        || emp.SurName.ToLower().Contains(normalizedSearchText)
                        || emp.UserName.ToLower().Contains(normalizedSearchText)
                        || emp.Email.ToLower().Contains(normalizedSearchText)
                        || emp.PhoneNumber.ToLower().Contains(normalizedSearchText)
                        || emp.MiddleName.ToLower().Contains(normalizedSearchText)
                    );

                    var result = employees.ToList();
                    if (result.Count == 0)
                    {
                        throw new UserFriendlyException("No employees found matching the search criteria.");
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException($"An exception was caught.{ex}");
            }
        }

    }
}
