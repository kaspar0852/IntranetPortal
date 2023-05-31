using IntranetPortal.Departments.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace IntranetPortal.Departments
{
    public interface IDepartment
    {
        Task<List<GetDepartmentDto>> GetDepartmentAsync();
        Task<PagedResultDto<ActiveDepartmentDto>> GetActiveDepartmentAsync();

    }
}
