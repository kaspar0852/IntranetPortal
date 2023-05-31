using IntranetPortal.Designations.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace IntranetPortal.Designations
{
    public interface IDesignationAppService
    {
        Task<List<GetDesignationDto>> GetDesignationAsync();
        Task<PagedResultDto<ActiveDesignationDto>> GetActivateDesignationAsync();
    }
}
