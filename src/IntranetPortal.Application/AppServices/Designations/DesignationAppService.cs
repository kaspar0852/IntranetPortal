using IntranetPortal.AppEntities;
using IntranetPortal.Designations.Dtos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace IntranetPortal.AppServices.Designations
{
    public class DesignationAppService : IntranetPortalAppService
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<Designation, Guid> _designationRepository;

        public DesignationAppService(
            IRepository<Designation, Guid> designationRepository,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _designationRepository = designationRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task<List<GetDesignationDto>> GetDesignationAsync()
        {
            try
            {
                Logger.LogInformation($"GetDepartment requested by User:{CurrentUser.Id}");
                Logger.LogDebug($"GetDepartment requested for DocumentStatuses:{CurrentUser.Id}");

                using (var uow = _unitOfWorkManager.Begin())
                {
                    var records = await _designationRepository.ToListAsync();
                    await uow.CompleteAsync();
                    Logger.LogInformation($"GetDepartment responded for User:{CurrentUser.Id}");
                    return ObjectMapper.Map<List<Designation>, List<GetDesignationDto>>(records);
                }
            }
            catch(Exception ex)
            {
                Logger.LogError(ex, nameof(GetDesignationAsync));
                throw new UserFriendlyException($"An exception was caught. {ex}");
            }
        }
    }
}
