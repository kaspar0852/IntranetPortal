using IntranetPortal.Documents.Dtos;
using IntranetPortal.InternalApplicationDto;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
    }
}
