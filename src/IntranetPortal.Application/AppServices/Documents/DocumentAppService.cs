using IntranetPortal.AppEntities;
using IntranetPortal.AppEntities.Documents;
using IntranetPortal.AppEntities.UserProfiles;
using IntranetPortal.DocumentAcknowledgementRequestStatus;
using IntranetPortal.Documents;
using IntranetPortal.Documents.Dtos;
using IntranetPortal.DocumentStatuses;
using IntranetPortal.Permissions;
using IntranetPortal.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Uow;

namespace IntranetPortal.AppServices.Documents
{
    [Authorize]
    public class DocumentAppService : IntranetPortalAppService, IDocumentAppService
    {
        private readonly IRepository<Document, Guid> _documentRepository;
        private readonly IRepository<DocumentStatus, Guid> _documentStatusRepository;
        private readonly IRepository<DocumentAcknowledgementRequestStatuses, Guid> _documentAcknowledgementRequestStatus;
        private readonly IRepository<DocumentAcknowledgementRequests, Guid> _documentAcknowledgementRequest;
        private readonly IRepository<Department, Guid> _departmentRepository;
        private readonly IRepository<Designation, Guid> _designationRepository;
        private readonly IRepository<UserProfile, Guid> _userProfileRepository;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<IdentityUser, Guid> _identityUserRepository;

        public DocumentAppService(
            IConfiguration configuration,
            IRepository<DocumentStatus, Guid> documentStatusRepository,
            IRepository<Document, Guid> documentrepository,
            IRepository<DocumentAcknowledgementRequestStatuses, Guid> documentAcknowledgementRequestStatus,
            IRepository<DocumentAcknowledgementRequests, Guid> documentAcknowledgementRequest,
            IRepository<IdentityUser, Guid> identityUserRepository,
            IRepository<UserProfile, Guid> userProfileRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<Designation, Guid> designationRepository,
            IRepository<Department, Guid> departmentRepository)
        {
            _documentStatusRepository = documentStatusRepository;
            _documentRepository = documentrepository;
            _documentAcknowledgementRequestStatus = documentAcknowledgementRequestStatus;
            _documentAcknowledgementRequest = documentAcknowledgementRequest;
            _configuration = configuration;
            _unitOfWorkManager = unitOfWorkManager;
            _identityUserRepository = identityUserRepository;
            _userProfileRepository = userProfileRepository;
            _designationRepository = designationRepository;
            _departmentRepository = departmentRepository;
        }

        [Authorize(IntranetPortalPermissions.DocumentAdmin.Create)]
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

                    if (documentStatus.SystemName.ToLower() == "active")
                    {
                        var newDocument = new Document
                        {
                            Name = input.Name,
                            Description = input.Description,
                            DocumentUrl = input.DocumentUrl,
                            AcknowledgementDeadlineInDays = input.AcknowledgementDeadlineInDays,
                            DocumentStatusId = documentStatus.Id
                        };
                        await _documentRepository.InsertAsync(newDocument);

                        var documentQuery = await _documentRepository.GetQueryableAsync();
                        var documentStatusQuery = await _documentStatusRepository.GetQueryableAsync();
                        var userQuery = await _identityUserRepository.GetQueryableAsync();
                        var userProfileQuery = await _userProfileRepository.GetQueryableAsync();


                        var newDocumentQuery = (from document in documentQuery
                                                join status in documentStatusQuery
                                                on document.DocumentStatusId equals status.Id

                                                join creator in userQuery
                                                on document.CreatorId equals creator.Id into CreatorGroupJoin

                                                from creator in CreatorGroupJoin.DefaultIfEmpty()
                                                join modifierProfile in userQuery
                                                on document.LastModifierId equals modifierProfile.Id into ModifierGroupJoin

                                                from modifier in ModifierGroupJoin.DefaultIfEmpty()
                                                join creatorProfile in userProfileQuery
                                                on creator.Id equals creatorProfile.AbpUserId

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
                                                    CreatedByFullName = creator == null
                                                    ? null
                                                    : (string.IsNullOrEmpty(creatorProfile.MiddleName)
                                                    ? $"{creator.Name} {creator.Surname}"
                                                    : $"{creator.Name} {creatorProfile.MiddleName} {creator.Surname}"),
                                                    LastModificationTime = DateTime.Now,
                                                    LastModifiedByFullName = modifier == null
                                                    ? null
                                                    : (string.IsNullOrEmpty(creatorProfile.MiddleName)
                                                    ? $"{modifier.Name}{modifier.Surname}"
                                                    : $"{modifier.Name}{creatorProfile.MiddleName}{modifier.Surname}"),
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


        [Authorize(IntranetPortalPermissions.DocumentAdmin.Delete)]
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


        [Authorize(IntranetPortalPermissions.DocumentAdmin.Update)]
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
                    var userProfileQuery = await _userProfileRepository.GetQueryableAsync();


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
                                          on document.CreatorId equals creator.Id into creatorGroupJoin

                                          from creator in creatorGroupJoin.DefaultIfEmpty()
                                          join modifierProfile in userQuery
                                          on document.LastModifierId equals modifierProfile.Id into ModifierGroupJoin

                                          from modifier in ModifierGroupJoin.DefaultIfEmpty()
                                          join creatorProfile in userProfileQuery
                                          on creator.Id equals creatorProfile.AbpUserId

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
                                              CreatedByFullName = creator == null
                                              ? null
                                              : (string.IsNullOrEmpty(creatorProfile.MiddleName)
                                              ? $"{creator.Name} {creator.Surname}"
                                              : $"{creator.Name} {creatorProfile.MiddleName} {creator.Surname}"),
                                              LastModificationTime = DateTime.Now,
                                              LastModifiedByFullName = modifier == null
                                                    ? null
                                                    : (string.IsNullOrEmpty(creatorProfile.MiddleName)
                                                    ? $"{modifier.Name}{modifier.Surname}"
                                                    : $"{modifier.Name}{creatorProfile.MiddleName}{modifier.Surname}"),
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
                    var documentQuery = (await _documentRepository.FindAsync(id));
                    var userQuery = await _identityUserRepository.GetQueryableAsync();
                    var activeStatus = await _documentStatusRepository.FindAsync(x => x.SystemName == DocumentStatusConstants.ACTIVE);
                    var userProfileQuery = await _userProfileRepository.GetQueryableAsync();

                    var queryDto =  from query in userQuery
                                     join userProfile in userProfileQuery
                                     on query.Id equals userProfile.AbpUserId

                                     select new DocumentDto
                                     {
                                         Id = documentQuery.Id,
                                         Name = documentQuery.Name,
                                         Description = documentQuery.Description,
                                         DocumentUrl = documentQuery.DocumentUrl,
                                         AcknowledgementDeadlineInDays = documentQuery.AcknowledgementDeadlineInDays,
                                         DocumentStatusId = activeStatus.Id,
                                         DocumentStatusDisplayName = activeStatus.DisplayName,
                                         DocumentStatusSystemName = activeStatus.SystemName,
                                         CreatorId = documentQuery.CreatorId,
                                         CreatedByFullName = query == null ?
                                                   null :
                                                   string.IsNullOrEmpty(userProfile.MiddleName) ?
                                                   $"{query.Name} {query.Surname}" :
                                                   $"{query.Name} {userProfile.MiddleName} {query.Surname}",
                                         CreationTime = documentQuery.CreationTime,
                                         LastModifierId = CurrentUser.Id,
                                         LastModifiedByFullName = CurrentUser.Name + " " + CurrentUser.SurName,
                                         LastModificationTime = DateTime.Now
                                     };
                    var outputDto = queryDto.FirstOrDefault();
                    await uow.CompleteAsync();
                    Logger.LogInformation($"Activate document responded for User:{CurrentUser.Id}");
                    return outputDto;
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


        [Authorize(IntranetPortalPermissions.DocumentAdmin.Upload)]
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
                        throw new UserFriendlyException("File extension does not match", code: "400");
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


        [Authorize(IntranetPortalPermissions.DocumentAdmin.ActivateDocument)]
        public async Task<ActiveDocumentDto> ActivateDocumentAsync(Guid id)
        {
            try
            {
                using (var uow = _unitOfWorkManager.Begin())
                {
                    Logger.LogInformation($"ActivateDocument requested by User:{CurrentUser.Id}");
                    Logger.LogDebug($"ActivateDocument requested for Document:{(CurrentUser.Id, id)}");

                    var documentQuery= (await _documentRepository.FindAsync(id));
                    var userQuery = await _identityUserRepository.GetQueryableAsync();
                    var activeStatus = await _documentStatusRepository.FindAsync(x => x.SystemName == DocumentStatusConstants.ACTIVE);
                    var userProfileQuery = await _userProfileRepository.GetQueryableAsync();

                    if (documentQuery.DocumentStatusId == activeStatus.Id)
                    {
                        throw new UserFriendlyException("Document can only be activated if its current status is Inactive", code: "400");
                    }
                    documentQuery.DocumentStatusId = activeStatus.Id;

                    await _documentRepository.UpdateAsync(documentQuery);

                    var activeDocumentQuery = (from query in userQuery
                                               join userProfile in userProfileQuery
                                               on query.Id equals userProfile.AbpUserId

                                               select new ActiveDocumentDto
                                               {
                                                   Id = documentQuery.Id,
                                                   Name = documentQuery.Name,
                                                   Description = documentQuery.Description,
                                                   DocumentUrl = documentQuery.DocumentUrl,
                                                   AcknowledgementDeadlineInDays = documentQuery.AcknowledgementDeadlineInDays,
                                                   DocumentStatusId = activeStatus.Id,
                                                   DocumentStatusDisplayName = activeStatus.DisplayName,
                                                   DocumentStatusSystemName = activeStatus.SystemName,
                                                   CreatorId = documentQuery.CreatorId,
                                                   CreatedByFullName = query.UserName + " " + userProfile.MiddleName + " " + query.Surname,
                                                   CreationTime = documentQuery.CreationTime,
                                                   LastModifierId = CurrentUser.Id,
                                                   ModifiedByLastName = CurrentUser.Name + " " + CurrentUser.SurName,
                                                   LastModificationTime = DateTime.Now,
                                               }).FirstOrDefault();
                    await uow.CompleteAsync();
                    Logger.LogInformation($"Activate document responded for User:{CurrentUser.Id}");
                    return activeDocumentQuery;
                }
            }
            catch(Exception ex)
            {
                Logger.LogError(ex, nameof(ActivateDocumentAsync));
                throw new UserFriendlyException($"{ex}");
            }
        }


        [Authorize(IntranetPortalPermissions.DocumentAdmin.DeactivateDocument)]
        public async Task<DeactivateDocumentDto> DeactivateDocumentAsync(Guid id)
        {
            try
            {
                using (var uow = _unitOfWorkManager.Begin())
                {
                    Logger.LogInformation($"DeactivateDocument requested by User:{CurrentUser.Id}");
                    Logger.LogDebug($"DeactivateDocument requested for Document:{(CurrentUser.Id, id)}");

                    var documentQuery = (await _documentRepository.FindAsync(id));
                    var userQuery = await _identityUserRepository.GetQueryableAsync();
                    var activeStatus = await _documentStatusRepository.FindAsync(x => x.SystemName == DocumentStatusConstants.INACTIVE);
                    var userProfileQuery = await _userProfileRepository.GetQueryableAsync();


                    if (documentQuery.DocumentStatusId == activeStatus.Id)
                    {
                        throw new UserFriendlyException("Document can only be Inactive if its current status is Active", code: "400");
                    }
                    documentQuery.DocumentStatusId = activeStatus.Id;

                    await _documentRepository.UpdateAsync(documentQuery);

                    var activeDocumentQuery = (from query in userQuery
                                               join userProfile in userProfileQuery
                                               on query.Id equals userProfile.AbpUserId
                                               select new DeactivateDocumentDto
                                               {
                                                   Id = documentQuery.Id,
                                                   Name = documentQuery.Name,
                                                   Description = documentQuery.Description,
                                                   DocumentUrl = documentQuery.DocumentUrl,
                                                   AcknowledgementDeadlineInDays = documentQuery.AcknowledgementDeadlineInDays,
                                                   DocumentStatusId = activeStatus.Id,
                                                   DocumentStatusDisplayName = activeStatus.DisplayName,
                                                   DocumentStatusSystemName = activeStatus.SystemName,
                                                   CreatorId = documentQuery.CreatorId,
                                                   CreatedByFullName = query.UserName + " " + userProfile.MiddleName + " " + query.Surname,
                                                   CreationTime = documentQuery.CreationTime,
                                                   LastModifierId = CurrentUser.Id,
                                                   LastModifiedByFullname = CurrentUser.Name + " " + CurrentUser.SurName,
                                                   LastModificationTime = DateTime.Now,
                                               }).FirstOrDefault();
                    await uow.CompleteAsync();
                    Logger.LogInformation($"Deactivate document responded for User:{CurrentUser.Id}");
                    return activeDocumentQuery;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(DeactivateDocumentAsync));
                throw new UserFriendlyException($"{ex}");
            }
        }

        public async Task<PagedResultDto<DocumentDto>> GetPagedAndSortedDocumentListAsync(PagedAndSortedDocumentListDto input)
        {
            try
            {
                Logger.LogInformation($"GetPagedAndSortedDocumentList requested by User:{CurrentUser.Id}");
                Logger.LogDebug($"GetPagedAndSortedDocumentList requested for InternalApplication:{(CurrentUser.Id, input)}");

                using (var uow = _unitOfWorkManager.Begin())
                {
                    var documentQuery = await _documentRepository.GetQueryableAsync();
                    var documentStatusQuery = await _documentStatusRepository.GetQueryableAsync();
                    var userQuery = await _identityUserRepository.GetQueryableAsync();
                    var userProfileQuery = await _userProfileRepository.GetQueryableAsync();


                    #region Filter
                    if (input.FromDate.HasValue)
                    {
                        documentQuery = documentQuery.Where(x => x.CreationTime.Date >=  input.FromDate.Value);
                    }
                    if (input.ToDate.HasValue)
                    {
                        documentQuery = documentQuery.Where(x => x.CreationTime.Date <= input.ToDate.Value);
                    }
                    if (!string.IsNullOrWhiteSpace(input.SearchKeyword))
                    {
                        /*documentQuery = documentQuery.Where(x => x.Name.ToLower().Contains(input.SearchKeyword.ToLower().Trim()));*/
                        documentStatusQuery = documentStatusQuery.Where(x => x.SystemName.ToLower().Contains(input.SearchKeyword.ToLower().Trim()));
                    }
                    #endregion

                    #region Sorting

                    switch (input.Sorting)
                    {
                        case "CreationTime":
                            {
                                Expression<Func<Document, DateTime>> orderingFunction = (x => x.CreationTime);
                                documentQuery = input.SortType.ToLower() == "desc"
                                    ? documentQuery.OrderByDescending(orderingFunction)
                                    : documentQuery.OrderBy(orderingFunction);
                                break;
                            }

                        default:
                            {
                                Expression<Func<Document, string>> orderingFunction = (x => x.Name);
                                documentQuery = input.SortType.ToLower() == "desc"
                                    ? documentQuery.OrderByDescending(orderingFunction)
                                    : documentQuery.OrderBy(orderingFunction);
                                break;
                            }
                    }

                    #endregion

                    var totalCount = documentQuery.Count();

                     var query = from document in documentQuery
                                 join status in documentStatusQuery
                                 on document.DocumentStatusId equals status.Id

                                 join creator in userQuery
                                 on document.CreatorId equals creator.Id into CreatorGroupJoin

                                 from creator in CreatorGroupJoin.DefaultIfEmpty()
                                 join modifierProfile in userQuery
                                 on document.LastModifierId equals modifierProfile.Id into ModifierGroupJoin

                                 from modifier in ModifierGroupJoin.DefaultIfEmpty()
                                 join creatorProfile in userProfileQuery
                                 on creator.Id equals creatorProfile.AbpUserId

                                 select new
                                    {
                                        document.Id,
                                        document.Name,
                                        document.Description,
                                        document.DocumentUrl,
                                        document.DocumentStatusId,
                                        document.AcknowledgementDeadlineInDays,
                                        document.CreationTime,
                                        document.CreatorId,
                                        document.LastModificationTime,
                                        document.LastModifierId,
                                        status.SystemName,
                                        status.DisplayName,
                                        creator.UserName,
                                        CreatedByFullName = creator == null
                                        ? null 
                                        : (string.IsNullOrEmpty(creatorProfile.MiddleName) 
                                        ? $"{creator.Name} {creator.Surname}" 
                                        : $"{creator.Name} {creatorProfile.MiddleName} {creator.Surname}"),
                                       LastModifiedByFullName = modifier == null
                                       ? null
                                       : (string.IsNullOrEmpty(creatorProfile.MiddleName)
                                       ? $"{modifier.Name}{modifier.Surname}"
                                       : $"{modifier.Name}{creatorProfile.MiddleName}{modifier.Surname}"),
                                 };

                    var queryResult = query.Select(x => new DocumentDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        DocumentUrl = x.DocumentUrl,
                        DocumentStatusId = x.DocumentStatusId,
                        AcknowledgementDeadlineInDays = x.AcknowledgementDeadlineInDays,
                        CreatorId = x.CreatorId,
                        LastModifierId = x.LastModifierId,
                        LastModificationTime = x.LastModificationTime,
                        CreationTime = x.CreationTime,
                        DocumentStatusSystemName = x.SystemName,
                        DocumentStatusDisplayName = x.DisplayName,
                        CreatedByFullName = x.CreatedByFullName,
                        LastModifiedByFullName = x.LastModifiedByFullName
                    }).Skip(input.SkipCount).Take(input.MaxResultCount).ToList();

                    await uow.CompleteAsync();
                    Logger.LogInformation($"GetDocumentList responded for User:{CurrentUser.Id}");

                    return new PagedResultDto<DocumentDto>(totalCount, queryResult);

                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(GetPagedAndSortedDocumentListAsync));
                throw new UserFriendlyException($"{ex}");
            }
        }

       public async Task<List<GetDocumentAcknowledgementRequestStatusDto>> GetDocumentAcknowledgementRequestStatusAsync()
        {
            try
            {
                Logger.LogInformation($"GetDocumentAcknowledgementRequestStatus requested by User:{CurrentUser.Id}");
                Logger.LogDebug($"GetDocumentAcknowledgementRequestStatus requested for InternalApplication:{(CurrentUser.Id)}");

                using(var uow = _unitOfWorkManager.Begin())
                {
                    var records = await _documentAcknowledgementRequestStatus.ToListAsync();
                    await uow.CompleteAsync();
                    Logger.LogInformation($"GetDocumentAcknowledgementRequestStatuses responded for User:{CurrentUser.Id}");
                    return ObjectMapper.Map<List<DocumentAcknowledgementRequestStatuses>, List<GetDocumentAcknowledgementRequestStatusDto>>(records);

                }
            }
            catch(Exception ex)
            {
                Logger.LogError(ex, nameof(GetDocumentAcknowledgementRequestStatusAsync));
                throw new UserFriendlyException($"An exception was caught. {ex}");
            }
        }


        public async Task<PagedResultDto<ReponseForAcknowledgementRequestDto>> GetEmployeesForDocumentAcknowledgementRequestAsync(GetEmployeesForAcknowledgementRequestDto input)
        {
            try
            {
                Logger.LogInformation($"GetGetEmployeesForDocumentAcknowledgementRequest requested by User:{CurrentUser.Id}");
                Logger.LogDebug($"GetGetEmployeesForDocumentAcknowledgementRequest requested for InternalApplication:{(CurrentUser.Id, input)}");

                using (var uow = _unitOfWorkManager.Begin())
                {
                    var userIdAcknowledgementRequest = (await _documentAcknowledgementRequest.WithDetailsAsync(x => x.DocumentAcknowledgementRequestStatus)).Where(x =>
                        x.DocumentAcknowledgementRequestStatus.SystemName == DocumentAcknowledgementRequestStatusConstants.NEW ||
                        x.DocumentAcknowledgementRequestStatus.SystemName == DocumentAcknowledgementRequestStatusConstants.ACKNOWLEDGED &&
                        x.DocumentId == input.DocumentId).Select(x => x.AbpUserId);

                    var eligibleUsers = (await _identityUserRepository.GetQueryableAsync()).Where(x => !userIdAcknowledgementRequest.Any(c => c == x.Id));
                    var userProfile = (await _userProfileRepository.GetQueryableAsync()).Where(x => eligibleUsers.Any(a => a.Id == x.AbpUserId));
                    var departments = await _departmentRepository.GetQueryableAsync();
                    var designations = await _designationRepository.GetQueryableAsync();

                    var query = from profile in userProfile
                                join user in eligibleUsers on profile.AbpUserId equals user.Id
                                join department in departments on profile.DepartmentId equals department.Id
                                join designation in designations on profile.DesignationId equals designation.Id

                                select new
                                {
                                    user.Id,
                                    user.Name,
                                    profile.MiddleName,
                                    user.Surname,
                                    FullName = user.Name + " " + profile.MiddleName + " " + user.Surname,
                                    DesignationName = designation.Name,
                                    DesignationId = designation.Id,
                                    DepartmentName = department.Name,
                                    DepartmentId = department.Id,
                                    profile.ProfilePictureUrl,
                                    user.Email,
                                    user.PhoneNumber
                                };

                    #region Filtering
                    query = query.WhereIf(input.DepartmentId != null, x => input.DepartmentId.Contains(x.DepartmentId));
                    query = query.WhereIf(input.DesignationId != null, x => input.DesignationId.Contains(x.DesignationId));

                    if (!string.IsNullOrWhiteSpace(input.SearchKeyword))
                    {
                        query = query.Where(x =>
                        x.Name.ToLower().Contains(input.SearchKeyword.ToLower()) ||
                        x.Email.ToLower().Contains(input.SearchKeyword.ToLower()));
                    }
                    #endregion

                    #region Sorting
                    switch (input.Sorting)
                    {
                        case "Email":
                            query = input.SortType.ToLower() == "desc"
                                ? query.OrderByDescending(x => x.Email)
                                : query.OrderBy(x => x.Email);
                            break;

                        case "Department":
                            query = input.SortType.ToLower() == "desc"
                                ? query.OrderByDescending(x => x.DepartmentId)
                                : query.OrderBy(x => x.DepartmentId);
                            break;

                        case "Designation":
                            query = input.SortType.ToLower() == "desc"
                                ? query.OrderByDescending(x => x.DesignationId)
                                : query.OrderBy(x => x.DesignationId);
                            break;

                        default:
                            query = input.SortType.ToLower() == "desc"
                                ? query.OrderByDescending(x => x.FullName)
                                : query.OrderBy(x => x.FullName);
                            break;


                    }
                    #endregion
                    var totalCount = query.Count();
                    query = query.Skip(input.SkipCount).Take(input.MaxResultCount);

                    var queryResult = query.Select(x =>
                        new ReponseForAcknowledgementRequestDto
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Middlename = x.MiddleName,
                            Surname = x.Surname,
                            FullName = x.FullName,
                            Designation = x.DesignationName,
                            DesignationId = x.DesignationId,
                            Department = x.DepartmentName,
                            DepartmentId = x.DepartmentId,
                            ProfilePictureUrl = x.ProfilePictureUrl,
                            Email = x.Email,
                            PhoneNumber = x.PhoneNumber
                        }).ToList();

                    await uow.CompleteAsync();
                    Logger.LogInformation($"EmployeesList for EmployeeListForAcknowledgementRequest Responded for User :{CurrentUser.Id}");
                    return new PagedResultDto<ReponseForAcknowledgementRequestDto>(totalCount, queryResult);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(GetEmployeesForDocumentAcknowledgementRequestAsync));
                throw new UserFriendlyException($"An exception was caught. {ex}");
            }
        }

        public async Task<ResponseDto> RequestAcknowledgementForDocument(RequestAcknowledgementForDocumentInputDto input)
        {
            try
            {
                Logger.LogInformation($"RequestAcknowledgementForDocument requested by User:{CurrentUser.Id}");
                Logger.LogDebug($"RequestAcknowledgementForDocument requested for InternalApplication:{(CurrentUser.Id, input)}");

                if (input.AcknowledgmentRequestedToId == null || input.AcknowledgmentRequestedToId.Count == 0)
                {
                    throw new UserFriendlyException("Please select at least one Employee", code: "400");
                }

                var listOfEmployee = await GetEmployeesForDocumentAcknowledgementRequestAsync(new GetEmployeesForAcknowledgementRequestDto
                {
                    DocumentId = input.DocumentId
                    /*MaxResultCount = 1000*/
                });

                var filteredEmployees = listOfEmployee.Items.Where(x => input.AcknowledgmentRequestedToId.Contains(x.Id)).ToList();

                if(filteredEmployees.Count != input.AcknowledgmentRequestedToId.Count)
                {
                    throw new UserFriendlyException("One or more employees in the acknowledgement list could not be found.", code: "404");
                }

                using (var uow = _unitOfWorkManager.Begin())
                {
                    var document = await _documentRepository.FindAsync(x => x.Id == input.DocumentId);

                    if (document == null)
                    {
                        throw new UserFriendlyException("Document not found", code: "404");
                    }

                    var activeStatus = await _documentStatusRepository.FindAsync(x => x.SystemName == DocumentStatusConstants.ACTIVE);

                    if (document.DocumentStatusId != activeStatus.Id)
                    {
                        throw new UserFriendlyException("The Document you are trying to request is not Active", "400");
                    }

                    var newStatus = await _documentAcknowledgementRequestStatus.FindAsync(x => x.SystemName == DocumentAcknowledgementRequestStatusConstants.NEW);
                    var dueDateTime = document.CreationTime.AddDays(document.AcknowledgementDeadlineInDays);

                    var newDocumentRequest = filteredEmployees.Select(employee => new DocumentAcknowledgementRequests
                    {
                        AbpUserId = employee.Id,
                        DocumentId = document.Id,
                        DocumentAcknowledgementRequestStatusId = newStatus.Id,
                        DueDateTime = dueDateTime
                    }).ToList();
                    await uow.CompleteAsync();
                    await _documentAcknowledgementRequest.InsertManyAsync(newDocumentRequest);
                    return new ResponseDto { Code = 200, Success = true, Message = "Acknowledgement request is Successfully sent" };
                }
            }

            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(RequestAcknowledgementForDocument));
                throw new UserFriendlyException($"An exception was caught. {ex}");
            }
        }

        public async Task<PagedResultDto<DocumentAcknowledgementRequestsForMeDto>> GetDocumentAcknowledgementForMeAsync(GetDocumentAcknowledgementRequestsDto input)
        {
            try
            {
                Logger.LogInformation($"DocumentAcknowledgementForMe requested by User:{CurrentUser.Id}");
                Logger.LogDebug($"DocumentAcknowledgementForMe requested for InternalApplication:{(CurrentUser.Id, input)}");

                using (var uow = _unitOfWorkManager.Begin())    
                {

                    var documentQuery = await _documentRepository.GetQueryableAsync();
                    var documentAcknowledgementRequestQuery = (await _documentAcknowledgementRequest.GetQueryableAsync()).Where(x => x.AbpUserId == CurrentUser.Id);
                    var documentAcknowledgementRequestStatusQuery = await _documentAcknowledgementRequestStatus.GetQueryableAsync();
                    var userProfileQuery = await _userProfileRepository.GetQueryableAsync();
                    var userQuery = await _identityUserRepository.GetQueryableAsync();

                    #region Filter

                    documentAcknowledgementRequestQuery = documentAcknowledgementRequestQuery.WhereIf(input.DocumentAcknowledgementRequestStatusId != null,
                        x => input.DocumentAcknowledgementRequestStatusId.Contains(x.DocumentAcknowledgementRequestStatusId));

                    if (input.FromDate.HasValue)
                    {
                        documentAcknowledgementRequestQuery = documentAcknowledgementRequestQuery.Where(x =>
                        x.CreationTime.Date >= input.FromDate.Value);
                    }
                    if (input.ToDate.HasValue)
                    {
                        documentAcknowledgementRequestQuery = documentAcknowledgementRequestQuery.Where(x =>
                        x.CreationTime.Date <= input.ToDate.Value);
                    }
                    if (input.AcknowledgedFromDate.HasValue)
                    {
                        documentAcknowledgementRequestQuery = documentAcknowledgementRequestQuery.Where(x =>
                        x.AcknowledgedDateTime >= input.AcknowledgedFromDate.Value);
                    }
                    if (input.AcknowledgedToDate.HasValue)
                    {
                        documentAcknowledgementRequestQuery = documentAcknowledgementRequestQuery.Where(x =>
                        x.AcknowledgedDateTime <= input.AcknowledgedToDate.Value);
                    }
                    if (!string.IsNullOrWhiteSpace(input.SearchKeyword))
                    {
                        documentQuery = documentQuery.Where(x => x.Name.ToLower().Contains(input.SearchKeyword.ToLower().Trim()));
                    }

                    #endregion

                    #region Sorting

                    switch (input.Sorting)
                    {
                        case "CreationTime":
                            {
                                Expression<Func<Document, DateTime>> orderingFunction = (x => x.CreationTime);
                                documentQuery = input.SortType.ToLower() == "desc"
                                  ? documentQuery.OrderByDescending(orderingFunction)
                                  : documentQuery.OrderBy(orderingFunction);
                                break;
                            }
                        default:
                            {
                                Expression<Func<Document, string>> orderingFunction = (x => x.Name);
                                documentQuery = input.SortType.ToLower() == "desc"
                                  ? documentQuery.OrderByDescending(orderingFunction)
                                  : documentQuery.OrderBy(orderingFunction);
                                break;
                            }
                    }
                    #endregion

                    var query = from request in documentAcknowledgementRequestQuery
                                join document in documentQuery
                                on request.DocumentId equals document.Id

                                join status in documentAcknowledgementRequestStatusQuery
                                on request.DocumentAcknowledgementRequestStatusId equals status.Id

                                join abpUser in userQuery
                                on request.AbpUserId equals abpUser.Id

                                join abpUserProfile in userProfileQuery
                                on abpUser.Id equals abpUserProfile.AbpUserId

                                join creator in userQuery
                                on request.CreatorId equals creator.Id into CreatorGroupJoin
                                from creatorgroupjoin in CreatorGroupJoin.DefaultIfEmpty()

                                join creatorProfile in userProfileQuery
                                on creatorgroupjoin.Id equals creatorProfile.AbpUserId into CreatorProfileGroupJoin
                                from creatorProfile in CreatorProfileGroupJoin.DefaultIfEmpty()

                                join modifier in userQuery
                                on request.LastModifierId equals modifier.Id into ModifierGroupJoin
                                from modifiergroupjoin in ModifierGroupJoin.DefaultIfEmpty()

                                join modifierProfile in userProfileQuery
                                on modifiergroupjoin.Id equals modifierProfile.AbpUserId into ModifierProfileGroupJoin
                                from modifierProfile in ModifierProfileGroupJoin.DefaultIfEmpty()

                                select new
                                {
                                    request.Id,
                                    request.AbpUserId,
                                    abpUserFullName = $"{abpUser.Name} {abpUserProfile.MiddleName} {abpUser.Surname}",
                                    request.DocumentId,
                                    document.Name,
                                    document.Description,
                                    document.DocumentUrl,
                                    document.CreatorId,
                                    document.LastModifierId,
                                    creatorFullName = creatorgroupjoin == null ? null
                                    : (string.IsNullOrEmpty(creatorProfile.MiddleName)
                                    ? $"{creatorgroupjoin.Name} {creatorgroupjoin.Surname}"
                                    : $"{creatorgroupjoin.Name} {creatorProfile.MiddleName} {creatorgroupjoin.Surname}"),
                                    modifierFullName = modifiergroupjoin == null ? null
                                    : (string.IsNullOrEmpty(modifierProfile.MiddleName)
                                    ? $"{modifiergroupjoin.Name} {modifiergroupjoin.Surname}"
                                    :$"{modifiergroupjoin.Name} {modifierProfile.MiddleName} {modifiergroupjoin.Surname}"),
                                    document.CreationTime,
                                    document.LastModificationTime,
                                    request.DocumentAcknowledgementRequestStatusId,
                                    status.SystemName,
                                    status.DisplayName,
                                    request.AcknowledgedDateTime,
                                    request.DueDateTime
                                };


                    var totalCount = query.Count();
                    var queryResult = query.Select(x => new DocumentAcknowledgementRequestsForMeDto
                    {
                        Id = x.Id,
                        AcknowledgementRequestedToId = x.AbpUserId,
                        AcknowledgementRequestedToFullName = x.abpUserFullName,
                        DocumentId = x.DocumentId,
                        DocumentName = x.Name,
                        DocumentDescription = x.Description,
                        DocumentUrl = x.DocumentUrl,
                        CreatorId = x.CreatorId,
                        LastModifierId = x.LastModifierId,
                        CreatedByFullName = x.creatorFullName,
                        LastModifiedByFullName = x.modifierFullName,
                        CreationTime = x.CreationTime,
                        LastModificationTime = x.LastModificationTime,
                        DocumentAcknowledgementRequestStatusId = x.DocumentAcknowledgementRequestStatusId,
                        DocumentAcknowledgementRequestStatusSystemName = x.SystemName,
                        DocumentAcknowledgementRequestStatusDisplayName = x.DisplayName,
                        AcknowledgementDoneDateTime = x.AcknowledgedDateTime == null ? null : x.AcknowledgedDateTime,
                        DueDateTime = x.DueDateTime
                    }).Skip(input.SkipCount).Take(input.MaxResultCount).ToList();

                    await uow.CompleteAsync();
                    Logger.LogInformation($"DocumentAcknowledgementForMe responded for User:{CurrentUser.Id}");
                    return new PagedResultDto<DocumentAcknowledgementRequestsForMeDto>(totalCount, queryResult);
                }
            }
            catch(Exception ex)
            {
                Logger.LogError(ex, nameof(RequestAcknowledgementForDocument));
                throw new UserFriendlyException($"An exception was caught. {ex}");
            }
        }

        public async Task<PagedResultDto<DocumentAcknowledgementRequestDto>> GetPagedAndSortedDocumentAcknowledgmentRequestForAdminAsync(PagedAndSortedDocumentAcknowledgementRequestListDto input)
        {
            try
            {

            }
            catch (Exception ex)
            {

            }
        }


    }
}
