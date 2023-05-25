using IntranetPortal.AppEntities.Documents;
using IntranetPortal.Documents;
using IntranetPortal.Documents.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;

namespace IntranetPortal.AppServices.Documents
{
    [Authorize]
    public class DocumentAppService : IntranetPortalAppService, IDocumentAppService
    {
        private readonly IRepository<Document, Guid> _documentRepository;
        private readonly IRepository<DocumentStatus, Guid> _documentStatusRepository;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<IdentityUser, Guid> _identityUserRepository;

        public DocumentAppService(
            IConfiguration configuration,
            IRepository<DocumentStatus, Guid> documentStatusRepository,
            IRepository<Document,Guid> documentrepository,
            IRepository<IdentityUser,Guid> identityUserRepository,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _documentStatusRepository = documentStatusRepository;
            _documentRepository = documentrepository;
            _configuration = configuration;
            _unitOfWorkManager = unitOfWorkManager;
            _identityUserRepository = identityUserRepository;
        }
    
        public async Task<DocumentDto> CreateAsync(CreateDocumentDto input)
        {
            try
            {
                Logger.LogInformation($"CreateDocument requested by User:{CurrentUser.Id}");
                Logger.LogDebug($"CreateDocument requested for Document:{(CurrentUser.Id, input)}");

                using(var uow = _unitOfWorkManager.Begin())
                {
                    var documentStatus = await _documentStatusRepository.FirstOrDefaultAsync(x => x.SystemName.ToLower().Trim().Equals("active"));

                    if (await _documentRepository.AnyAsync(x => x.Name.ToLower() == input.Name.ToLower()))
                    {
                        throw new UserFriendlyException("Document name must be unique", "400");
                    }

                    if (!File.Exists(input.DocumentUrl))
                    {
                        throw new UserFriendlyException("The file doesn't exist.");
                    }

                    if (documentStatus.SystemName == "active")
                    {
                        var newDocument = new Document
                        {
                            Name = input.Name,
                            Description = input.Description,
                            DocumentUrl = input.DocumentUrl,
                            AcknowledgementDeadlineInDays = input.AcknowledgementDeadlineInDays,
                            DocumentStatusId = documentStatus.Id,

                        };
                        await _documentRepository.InsertAsync(newDocument);

                        var documentQuery = await _documentRepository.GetQueryableAsync();
                        var documentStatusQuery = await _documentStatusRepository.GetQueryableAsync();
                        var userQuery = await _identityUserRepository.GetQueryableAsync();

                        var newDocumentQuery = (from document in documentQuery
                                                join status in documentStatusQuery
                                                on document.DocumentStatusId equals status.Id

                                                join creator in userQuery
                                                on document.CreatorId equals creator.Id

                                                select new DocumentDto
                                                {
                                                    Id = document.Id,
                                                    Name = document.Name,
                                                    Description = document.Description,
                                                    DocumentUrl = document.DocumentUrl,
                                                    AcknowledgementDeadlineInDays = document.AcknowledgementDeadlineInDays,
                                                    DocumentStatusId = document.DocumentStatusId,
                                                    DocumentStatusSystemName = status.SystemName,
                                                    DocumentStatusDisplayName = status.DisplayName,
                                                    CreatorId = document.CreatorId,
                                                    LastModifierId = CurrentUser.Id,
                                                    CreationTime = document.CreationTime,
                                                    LastModificationTime = DateTime.Now,
                                                    CreatedByFullName = creator.UserName,
                                                    LastModifiedByFullName = creator.UserName,
                                                }).FirstOrDefault();

                        /*return ObjectMapper.Map<Document, DocumentDto>(newDocument, newDocumentQuery);*/
                        await uow.CompleteAsync();
                        Logger.LogInformation($"Create document responded for User:{CurrentUser.Id}");
                        return ObjectMapper.Map<Document, DocumentDto>(newDocument, newDocumentQuery);
                    }
                    else
                    {
                        throw new UserFriendlyException("System must be active", code: "400");
                    }
                }
                
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(CreateAsync));
                throw new UserFriendlyException($"{ex}");
            }
        }

        public async Task<string> DeleteDocumentAsync(Guid id)
        {
            try
            {
                Logger.LogInformation($"DeleteDocument Requested by the user:{id}");
                Logger.LogDebug($"DeleteDocument requested for Document:{(CurrentUser.Id, id)}");
                

                using (var uow = _unitOfWorkManager.Begin())
                {
                    var document = await _documentRepository.FindAsync(id);

                    if (document == null)
                    {
                        throw new UserFriendlyException("Delete id does not match", "400");
                        
                    }
                    else
                    {
                        await _documentRepository.DeleteAsync(id);
                        Logger.LogInformation("Deleted Successfully", "200");
                    }
                    await uow.CompleteAsync();
                    Logger.LogInformation($"DeleteDocument responded for User:{CurrentUser.Id}");
                    return "Deleted Successfully";
                }

            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(DeleteDocumentAsync));
                throw new UserFriendlyException($"{ex}");
            }
        }

        public async Task<DocumentDto> UpdateDocumentAsync(Guid id, UpdateDocumentDto input)
        {
            try
            {
                Logger.LogInformation($"UpdateDocument requested by User:{CurrentUser.Id}");
                Logger.LogDebug($"UpdateDocument requested for Document:{(CurrentUser.Id, input)}");

                using (var uow = _unitOfWorkManager.Begin())
                {

                    var documentToUpdate = await _documentRepository.FindAsync(id);
                    var documentStatus = await _documentStatusRepository.FirstOrDefaultAsync(x => x.SystemName.ToLower().Trim().Equals("active"));

                    if (documentToUpdate == null)
                    {
                        throw new UserFriendlyException("Id not found", code: "400");
                    }
                    else
                    {
                        if (documentStatus.Id == documentToUpdate.DocumentStatusId)     
                        {
                            input.Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.Name.ToLower().Trim().Replace("  ", " "));
                            ObjectMapper.Map(input, documentToUpdate);
                            await _documentRepository.UpdateAsync(documentToUpdate);

                            var documentQuery = (await _documentRepository.GetQueryableAsync()).Where(x => x.Id == id);
                            var documentstatusQuery = await _documentStatusRepository.GetQueryableAsync();
                            var userQuery = await _identityUserRepository.GetQueryableAsync();

                            var result = (from document in documentQuery
                                          join status in documentstatusQuery
                                          on document.DocumentStatusId equals status.Id

                                          join creator in userQuery
                                          on document.CreatorId equals creator.Id

                                          select new DocumentDto
                                          {
                                              Id = document.Id,
                                              Name = document.Name,
                                              Description = document.Description,
                                              DocumentUrl = document.DocumentUrl,
                                              AcknowledgementDeadlineInDays = document.AcknowledgementDeadlineInDays,
                                              DocumentStatusId = document.DocumentStatusId,
                                              DocumentStatusSystemName = status.SystemName,
                                              DocumentStatusDisplayName = status.DisplayName,
                                              CreatorId = document.CreatorId,
                                              LastModifierId = CurrentUser.Id,
                                              CreationTime = document.CreationTime,
                                              LastModificationTime = DateTime.Now,
                                          }).FirstOrDefault();
                            await uow.CompleteAsync();
                            Logger.LogInformation($"UpdatedDocument responded for the User:{CurrentUser.Id}");
                            return ObjectMapper.Map<Document, DocumentDto>(documentToUpdate, result);
                        }
                        else
                        {
                            throw new UserFriendlyException("System must be active", code:"400");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(UpdateDocumentAsync));
                throw new UserFriendlyException($"{ex}");
            }
        }

        public async Task<DocumentDto> GetDocumentByIdAsync(Guid id)
        {
            try
            {
                Logger.LogInformation($"GetDocumentById requested by User:{CurrentUser.Id}");
                Logger.LogDebug($"GetDocumentById requested for Document:{(CurrentUser.Id, id)}");

                using(var uow = _unitOfWorkManager.Begin())
                {
                    var records = await _documentRepository.FindAsync(id);
                    if (records == null)
                    {
                        throw new UserFriendlyException("No Document found", code: "400");
                    }
                    await uow.CompleteAsync();
                    Logger.LogInformation($"GetDocumentById responded by User:{(CurrentUser.Id)}");
                    return ObjectMapper.Map<Document, DocumentDto>(records);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(GetDocumentByIdAsync));
                throw new UserFriendlyException("${ex}");
            }

        }

        public async Task<List<GetDocumentStatusDto>> GetDocumentStatusAsync()
        {
            try
            {
                Logger.LogInformation($"GetDocumentStatuses requested by User:{CurrentUser.Id}");
                Logger.LogDebug($"GetDocumentStatuses requested for DocumentStatuses:{CurrentUser.Id}");
                using (var uow = _unitOfWorkManager.Begin())
                {
                    var records = await _documentStatusRepository.ToListAsync();
                    await uow.CompleteAsync();
                    Logger.LogInformation($"GetDocumentStatuses responded for User:{CurrentUser.Id}");
                    return ObjectMapper.Map<List<DocumentStatus>, List<GetDocumentStatusDto>>(records);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(GetDocumentStatusAsync));
                throw new UserFriendlyException($"An exception was caught. {ex}");
            }

        }

        public async Task<string> UploadDocumentAsync(IFormFile document)
        {
            try
            {
                Logger.LogInformation($"UploadDocumet requested by User:{CurrentUser.Id}");
                Logger.LogDebug($"UploadDocumet requested for Document:{(CurrentUser.Id, document)}");

                using (var uow = _unitOfWorkManager.Begin())
                {
                    if (document == null)
                    {
                        throw new UserFriendlyException("document file is required");
                    }
                    var whitelistedExtension = _configuration["App:WhitelistedExtensionsForDocuments"].Split(',');
                    var fileExtension = Path.GetExtension(document.FileName).TrimStart('.').ToLower();
                    if (!whitelistedExtension.Contains(fileExtension))
                    {
                        throw new UserFriendlyException("File extension is not allowed");
                    }
                    var basePath = _configuration["App:DocumentsBaseLocation"];

                    if (!Directory.Exists(basePath))
                    {
                        Directory.CreateDirectory(basePath);
                    }
                    var randomizedDocumentName = Path.GetFileNameWithoutExtension(document.FileName) + Guid.NewGuid() + Path.GetExtension(document.FileName);

                    var filePath = Path.Combine(basePath, randomizedDocumentName);
                    using var fileStream = new FileStream(filePath, FileMode.Create);
                    await document.CopyToAsync(fileStream);

                    await uow.CompleteAsync();
                    Logger.LogInformation($"UploadDocument responded for User:{CurrentUser.Id}");
                    return filePath;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(UploadDocumentAsync));
                throw new UserFriendlyException($"{ex}");
            }
        }
    }
}
