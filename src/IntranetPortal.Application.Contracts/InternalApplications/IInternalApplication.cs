using IntranetPortal.InternalApplicationDto;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace IntranetPortal.InternalApplication
{
    public interface IInternalApplication
    {
        Task<GetInternalApplicationDto> GetByIdAsync(Guid id);

        Task<PagedResultDto<ActiveInternalApplicationDto>> GetActiveInternalApplications();

        Task<List<GetInternalApplicationDto>> GetAllAsync();

        Task<List<ChangeInternalApplicationOrderOutputDto>> ChangeInternalApplicationOrderAsync(ChangeInternalApplicationOrderInputDto input);

        Task<UpdateInternalApplicationDto> UpdateAsync(UpdateInternalApplicationDto input);

        Task<GetInternalApplicationDto> CreateAsync(CreateInternalApplicationDto input);

        Task<string> DeleteAsync(Guid id);

        Task<string> UploadLogo(IFormFile logo);

        Task<PagedResultDto<GetInternalApplicationDto>> GetPagedAndSortedInternalApplicationListAsync(GetPagedAndSortedInternalApplicationListDto input);



    }
}