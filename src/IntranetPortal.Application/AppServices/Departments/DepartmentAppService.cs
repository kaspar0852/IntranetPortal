using IntranetPortal.AppEntities;
using IntranetPortal.Departments.Dtos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace IntranetPortal.AppServices.Departments
{
    public class DepartmentAppService : IntranetPortalAppService
    {
        private readonly IRepository<Department, Guid> _departmentRepostiory;
        private readonly IUnitOfWorkManager _unitOfWorkManager;


        public DepartmentAppService(
            IRepository<Department, Guid> departmentRepostiory,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _departmentRepostiory = departmentRepostiory;
            _unitOfWorkManager = unitOfWorkManager;
        }

        
        public async Task<List<GetDepartmentDto>> GetDepartmentAsync()
        {
            try
            {
                Logger.LogInformation($"GetDepartment requested by User:{CurrentUser.Id}");
                Logger.LogDebug($"GetDepartment requested for DocumentStatuses:{CurrentUser.Id}");

                using (var uow = _unitOfWorkManager.Begin())
                {
                    var records = await _departmentRepostiory.ToListAsync();
                    uow.CompleteAsync();
                    Logger.LogInformation($"GetDepartment responded for User:{CurrentUser.Id}");
                    return ObjectMapper.Map<List<Department>, List<GetDepartmentDto>>(records);
                }
            }
            catch(Exception ex) 
            {
                Logger.LogError(ex, nameof(GetDepartmentAsync));
                throw new UserFriendlyException($"An exception was caught. {ex}");
            }
        }
    }
}
