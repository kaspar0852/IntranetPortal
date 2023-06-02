using IntranetPortal.Documents.Dtos;
using IntranetPortal.InternalApplicationDto;
using IntranetPortal.Responses;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace IntranetPortal.Documents
{
    public interface IDocumentAppService
    {
        Task<DocumentDto> CreateAsync(CreateDocumentDto input);
        Task<string> DeleteDocumentAsync(Guid id);
        Task<DocumentDto> UpdateDocumentAsync(Guid id, UpdateDocumentDto input);
        Task<DocumentDto> GetDocumentByIdAsync(Guid id);
        Task<List<GetDocumentStatusDto>> GetDocumentStatusAsync();
        Task<string> UploadDocumentAsync(IFormFile document);
        Task<ActiveDocumentDto> ActivateDocumentAsync(Guid id);
        Task<DeactivateDocumentDto> DeactivateDocumentAsync(Guid id);
        Task<PagedResultDto<DocumentDto>> GetPagedAndSortedDocumentListAsync(PagedAndSortedDocumentListDto input);
        Task<List<GetDocumentAcknowledgementRequestStatusDto>> GetDocumentAcknowledgementRequestStatusAsync();
        Task<PagedResultDto<ReponseForAcknowledgementRequestDto>> GetEmployeesForDocumentAcknowledgementRequestAsync(GetEmployeesForAcknowledgementRequestDto input);
        Task<ResponseDto> RequestAcknowledgementForDocument(RequestAcknowledgementForDocumentInputDto input);
        Task<PagedResultDto<DocumentAcknowledgementRequestsForMeDto>> GetDocumentAcknowledgementForMeAsync(GetDocumentAcknowledgementRequestsDto input);
        Task<PagedResultDto<DocumentAcknowledgementRequestDto>> GetPagedAndSortedDocumentAcknowledgmentRequestForAdminAsync(PagedAndSortedDocumentAcknowledgementRequestListDto input);
        Task<ResponseDto> RevokeDocumentAcknowledgementRequestAsync(RevokeDocumentAcknowledgementRequestDto input);
        Task<DocumentDto> ReuploadDocumentAsync(ReuploadDocumentInputDto input);
        Task<ResponseDto> AcknowledgeDocumentRequestAsync(Guid id);
    }
}
