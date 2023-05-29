using IntranetPortal.InternalApplication;
using IntranetPortal.InternalApplicationDto;
using IntranetPortal.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace IntranetPortal.InternalApplicationAppService
{
    [Authorize]
    public class InternalApplicationAppService : IntranetPortalAppService, IInternalApplication 
    {
        private readonly IRepository<InternalApplications.InternalApplication, Guid> _internalApplicationRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IConfiguration _configuration;

        public InternalApplicationAppService(
            IRepository<InternalApplications.InternalApplication,Guid> internalApplicationRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IConfiguration configuration)
        {
            _internalApplicationRepository = internalApplicationRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _configuration = configuration;
        }

        [Authorize(IntranetPortalPermissions.InternalApplicationAdmin.Create)]
        public async Task<GetInternalApplicationDto> CreateAsync(CreateInternalApplicationDto input)
        {
            try
            {
                Logger.LogInformation($"CreateInternalApplication requested by User:{CurrentUser.Id}");
                Logger.LogDebug($"CreateInternalApplication requested for InternalApplication:{(CurrentUser.Id, input)}");

                using (var uow = _unitOfWorkManager.Begin())
                {
                    var urlRegex = @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$";

                    if (await _internalApplicationRepository.AnyAsync(x => x.Name.ToLower() == input.Name.ToLower()))
                    {
                        throw new UserFriendlyException("An application with same name is already exists.", "400");
                    }

                    if (string.IsNullOrWhiteSpace(input.Name) || string.IsNullOrWhiteSpace(input.DisplayName))
                    {
                        throw new UserFriendlyException("Names cannot be empty.", "400");
                    }

                    if (!Regex.IsMatch(input.ApplicationUrl, urlRegex, RegexOptions.IgnoreCase))
                    {
                        throw new UserFriendlyException("Please enter a valid url.", "400");
                    }

                    var internalApplication = new InternalApplications.InternalApplication
                    {
                        Name = input.Name,
                        DisplayName = input.DisplayName,
                        Description = input.Description,
                        LogoUrl = input.LogoUrl,
                        OrderNumber = input.OrderNumber,
                        ApplicationUrl = input.ApplicationUrl,
                        IsActive = input.IsActive,
                    };
                    var records = await _internalApplicationRepository.InsertAsync(internalApplication);

                    if (await _internalApplicationRepository.CountAsync() == 0)
                    {
                        internalApplication.OrderNumber = 1;
                    }
                    else
                    {
                        var maxOrderNumber = await _internalApplicationRepository.MaxAsync(x => x.OrderNumber);
                        internalApplication.OrderNumber = maxOrderNumber + 1;
                    }
                    await uow.CompleteAsync();
                    Logger.LogInformation($"Create InternalApplication responded for User:{CurrentUser.Id}");
                    return ObjectMapper.Map<InternalApplications.InternalApplication, GetInternalApplicationDto>(records);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(CreateAsync));
                throw new UserFriendlyException($"An exception was caught, {ex}");
            }

        }

        [Authorize(IntranetPortalPermissions.InternalApplicationAdmin.Delete)]
        public async Task<string> DeleteAsync(Guid id)
        {
            try
            {
                Logger.LogInformation($"DeleteInternalApplication requested by User:{CurrentUser.Id}");
                Logger.LogDebug($"DeleteInternalApplication requested for InternalApplication:{(CurrentUser.Id, id)}");

                using (var uow = _unitOfWorkManager.Begin())
                {
                    var internalApplication = await _internalApplicationRepository.FindAsync(id);
                    if (internalApplication is not null)
                    {
                        await _internalApplicationRepository.DeleteAsync(id);
                        Logger.LogInformation("Deleted Successfully", "200");
                    }
                    else
                    {
                        throw new UserFriendlyException("Delete id does not match", "400");
                    }
                    await uow.CompleteAsync();
                    Logger.LogInformation($"DeleteInternalApplication responded for User:{CurrentUser.Id}");
                    return "Deleted Successfully";
                }
            }
            catch(Exception ex)
            {
                Logger.LogError(ex, nameof(DeleteAsync));
                throw new UserFriendlyException($"{ex}");
            }
        }


        [AllowAnonymous]
        public async Task<PagedResultDto<ActiveInternalApplicationDto>> GetActiveInternalApplications()
        {
            try
            {
                Logger.LogInformation($"GetActiveInternalApplication requested by User:{CurrentUser.Id}");
                Logger.LogDebug($"GetActiveInternalApplication requested for InternalApplication:{(CurrentUser.Id)}");

                using (var uow = _unitOfWorkManager.Begin())
                {
                    var internalApplicationList = (await _internalApplicationRepository.GetQueryableAsync()).Where(x => x.IsActive == true).OrderBy(x => x.OrderNumber);
                    var query =
                        from InternalApplication in internalApplicationList
                        select new ActiveInternalApplicationDto
                        {
                            Id = InternalApplication.Id,
                            Name = InternalApplication.Name,
                            Description = InternalApplication.Description,
                            LogouRL = InternalApplication.LogoUrl,
                            ApplicationUrl = InternalApplication.ApplicationUrl,
                            OrderNumber = InternalApplication.OrderNumber,
                        };
                    var totalCount = query.Count();

                    await uow.CompleteAsync();
                    Logger.LogInformation($"GetActive InternalApplication responded for User:{CurrentUser.Id}");
                    return new PagedResultDto<ActiveInternalApplicationDto>(totalCount, query.ToList());
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(GetActiveInternalApplications));
                throw new UserFriendlyException($"{ex}");

            }
        }

        [AllowAnonymous]
        public async Task<List<ChangeInternalApplicationOrderOutputDto>> ChangeInternalApplicationOrderAsync(ChangeInternalApplicationOrderInputDto input)
        {
            try
            {
                Logger.LogInformation($"ChangeInternalApplicationOrder requested by User:{CurrentUser.Id}");
                Logger.LogDebug($"ChangeInternalApplicationOrder requested for InternalApplication:{(CurrentUser.Id,input)}");

                using (var uow = _unitOfWorkManager.Begin())
                {
                    if (input is null || !input.Id.Any() || !input.OrderNumber.Any() || input.Id.Count != input.OrderNumber.Count)
                    {
                        throw new UserFriendlyException("Invalid Input", code: "400");
                    }
                    var outputList = new List<ChangeInternalApplicationOrderOutputDto>();
                    for (int i = 0; i < input.Id.Count; i++)
                    {
                        var internalApplication = await _internalApplicationRepository.GetAsync(input.Id[i]);
                        {
                            if (internalApplication is not null && internalApplication.OrderNumber != input.OrderNumber[i])
                            {
                                internalApplication.OrderNumber = input.OrderNumber[i];
                                await _internalApplicationRepository.UpdateAsync(internalApplication);
                                outputList.Add(new ChangeInternalApplicationOrderOutputDto { Id = internalApplication.Id, OrderNumber = internalApplication.OrderNumber });
                            }
                        }
                    }
                    await uow.CompleteAsync();
                    Logger.LogInformation($"ChangeInternalAllicationOrder responded for User:{CurrentUser.Id}");
                    return outputList;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(ChangeInternalApplicationOrderAsync));
                throw new UserFriendlyException($"{ex}");
            }
        }

        [AllowAnonymous]
        public async Task<List<GetInternalApplicationDto>> GetAllAsync()
        {
            try
            {
                Logger.LogInformation($"GetAllInternalApplication requested by User:{CurrentUser.Id}");
                Logger.LogDebug($"GetAllInternalApplication requested for InternalApplication:{(CurrentUser.Id)}");

                using (var uow = _unitOfWorkManager.Begin())
                {
                    var records = await _internalApplicationRepository.ToListAsync();
                    await uow.CompleteAsync();
                    Logger.LogInformation($"GetAllInternalApplication responded for User:{CurrentUser.Id}");
                    return ObjectMapper.Map<List<InternalApplications.InternalApplication>, List<GetInternalApplicationDto>>(records);
                }
            }
            catch(Exception ex)
            {
                Logger.LogError(ex, nameof(GetAllAsync));
                throw new UserFriendlyException($"{ex}");
            }
        }

        [AllowAnonymous]
        public async Task<GetInternalApplicationDto> GetByIdAsync(Guid id)
        {
            try
            {
                Logger.LogInformation($"GetInternalApplicationById requested by User:{CurrentUser.Id}");
                Logger.LogDebug($"GetInternalApplicationById requested for InternalApplication:{(CurrentUser.Id,id)}");

                using (var uow = _unitOfWorkManager.Begin())
                {
                    var internalApplication = await _internalApplicationRepository.FindAsync(id);
                    var result = ObjectMapper.Map<InternalApplications.InternalApplication, GetInternalApplicationDto>(internalApplication);

                    await uow.CompleteAsync();
                    Logger.LogInformation($"GetInternalApplicationById responded for User:{CurrentUser.Id}");
                    return result;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(GetByIdAsync));
                throw new UserFriendlyException($"{ex}");
            }
        }

        [Authorize(IntranetPortalPermissions.InternalApplicationAdmin.Update)]
        public async Task<UpdateInternalApplicationDto> UpdateAsync(UpdateInternalApplicationDto input)
        {
            try
            {
                Logger.LogInformation($"UpdateInternalApplication requested by User:{CurrentUser.Id}");
                Logger.LogDebug($"UpdateInternalApplication requested for InternalApplication:{(CurrentUser.Id, input)}");

                using (var uow = _unitOfWorkManager.Begin())
                {
                    var urlRegex = @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$";

                    if (!Regex.IsMatch(input.ApplicationUrl, urlRegex, RegexOptions.IgnoreCase))
                    {
                        throw new UserFriendlyException("Please add a valid Url");
                    }
                    if (string.IsNullOrEmpty(input.Name) || (string.IsNullOrEmpty(input.DisplayName)))
                    {
                        throw new UserFriendlyException("Name or DisplayName fields cannot be empty", code: "400");
                    }
                    if (await _internalApplicationRepository.AnyAsync(x => x.Name.ToLower() == input.Name.ToLower() && x.Id != input.Id))
                    {
                        throw new UserFriendlyException("Name already exists", code: "400");
                    }

                    var internalApplication = await _internalApplicationRepository.FindAsync(input.Id);

                    internalApplication.Name = input.Name;
                    internalApplication.DisplayName = input.DisplayName;
                    internalApplication.Description = input.Description;
                    internalApplication.LogoUrl = input.LogoUrl;
                    internalApplication.OrderNumber = internalApplication.OrderNumber;
                    internalApplication.ApplicationUrl = input.ApplicationUrl;
                    internalApplication.IsActive = input.IsActive;

                    await _internalApplicationRepository.UpdateAsync(internalApplication);
                    var outputDto = new UpdateInternalApplicationDto
                    {
                        Id = internalApplication.Id,
                        Name = internalApplication.Name,
                        DisplayName = internalApplication.DisplayName,
                        Description = internalApplication.Description,
                        LogoUrl = internalApplication.LogoUrl,
                        OrderNumber = internalApplication.OrderNumber,//internalApplication.OrderNumber,
                        ApplicationUrl = internalApplication.ApplicationUrl,
                        IsActive = internalApplication.IsActive
                    };

                    await uow.CompleteAsync();
                    Logger.LogInformation($"UpdateInternalApplication responded for User:{CurrentUser.Id}");
                    return outputDto;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(UpdateAsync));
                throw new UserFriendlyException("${ex}");
            }
        }

        [Authorize(IntranetPortalPermissions.InternalApplicationAdmin.LogoUpload)]
        public async Task<string> UploadLogo(IFormFile logo)
        {
            try
            {

                Logger.LogInformation($"UploadLogoInternalApplication requested by User:{CurrentUser.Id}");
                Logger.LogDebug($"UploadLogoInternalApplication requested for InternalApplication:{(CurrentUser.Id, logo)}");

                using (var uow = _unitOfWorkManager.Begin())
                {
                    if (logo is null)
                    {
                        throw new UserFriendlyException("Logo file is required");
                    }

                    var whitelistedExtension = _configuration["App:WhitelistedExtentionsForInternalApplicationLogos"].Split(',');
                    var fileExtension = Path.GetExtension(logo.FileName).TrimStart('.').ToLower();
                    if (!whitelistedExtension.Contains(fileExtension))
                    {
                        throw new UserFriendlyException("File extension is not allowed");
                    }
                    var basePath = _configuration["App:InternalApplicationLogoBaseLocation"];

                    if (!Directory.Exists(basePath))
                    {
                        Directory.CreateDirectory(basePath);
                    }
                    var randomizedLogoName = Path.GetFileNameWithoutExtension(logo.FileName) + Guid.NewGuid() + Path.GetExtension(logo.FileName);

                    var filePath = Path.Combine(basePath, randomizedLogoName);
                    using var fileStream = new FileStream(filePath, FileMode.Create);
                    await logo.CopyToAsync(fileStream);

                    await uow.CompleteAsync();
                    Logger.LogInformation($"UploadLogoInternalApplication responded for User:{CurrentUser.Id}");
                    return randomizedLogoName;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(UploadLogo));
                throw new UserFriendlyException($"{ex}");
            }
        }

        [AllowAnonymous]
        public async Task<PagedResultDto<GetInternalApplicationDto>> GetPagedAndSortedInternalApplicationListAsync(GetPagedAndSortedInternalApplicationListDto input)
        {
            try
            {

                Logger.LogInformation($"GetPageAndSortedInternalApplicationList requested by User:{CurrentUser.Id}");
                Logger.LogDebug($"GetPageAndSortedInternalApplicationList requested for InternalApplication:{(CurrentUser.Id, input)}");

                using (var uow = _unitOfWorkManager.Begin())
                {
                    var internalApplicationQuery = await _internalApplicationRepository.GetQueryableAsync();

                    #region Filter

                    if (input.FromDate.HasValue)
                    {
                        internalApplicationQuery = internalApplicationQuery.Where(x =>
                        x.CreationTime.Date >= input.FromDate.Value);
                    }
                    if (input.ToDate.HasValue)
                    {
                        internalApplicationQuery = internalApplicationQuery.Where(x =>
                        x.CreationTime.Date <= input.ToDate.Value);
                    }
                    if (!string.IsNullOrWhiteSpace(input.SearchKeyword))
                    {
                        internalApplicationQuery = internalApplicationQuery.Where(x =>
                        x.Name.ToLower().Contains(input.SearchKeyword.ToLower().Trim()) ||
                        x.DisplayName.ToLower().Contains(input.SearchKeyword.ToLower().Trim()) ||
                        x.IsActive.ToString().ToLower().Contains(input.SearchKeyword.ToLower().Trim()));
                    }

                    #endregion

                    #region Sorting

                    switch (input.Sorting)
                    {
                        case "DisplayName":
                            {
                                Expression<Func<InternalApplications.InternalApplication, string>> orderingFunction = (x => x.DisplayName);
                                internalApplicationQuery = input.SortType.ToLower() == "desc"
                                    ? internalApplicationQuery.OrderByDescending(orderingFunction)
                                    : internalApplicationQuery.OrderBy(orderingFunction);
                                break;
                            }

                        case "OrderNumber":
                            {
                                Expression<Func<InternalApplications.InternalApplication, string>> orderingFunction = (x => x.Name);
                                internalApplicationQuery = input.SortType.ToLower() == "desc"
                                    ? internalApplicationQuery.OrderByDescending(orderingFunction)
                                    : internalApplicationQuery.OrderBy(orderingFunction);
                                break;
                            }

                        default:
                            {
                                Expression<Func<InternalApplications.InternalApplication, int>> orderingFunction = (x => x.OrderNumber);
                                internalApplicationQuery = input.SortType.ToLower() == "desc"
                                    ? internalApplicationQuery.OrderByDescending(orderingFunction)
                                    : internalApplicationQuery.OrderBy(orderingFunction);
                                break;
                            }
                    }

                    #endregion

                    var totalCount = internalApplicationQuery.Count();
                    var queryResult = internalApplicationQuery.Select(x =>
                    new GetInternalApplicationDto
                    {
                        Name = x.Name,
                        DisplayName = x.DisplayName,
                        Description = x.Description,
                        IsActive = x.IsActive,
                        LogoUrl = x.LogoUrl,
                        ApplicationUrl = x.ApplicationUrl
                    }).Skip(input.SkipCount).Take(input.MaxResultCount).ToList();

                    await uow.CompleteAsync();
                    Logger.LogInformation($"GetPageAndSortedInternalApplicationList responded for User:{CurrentUser.Id}");
                    return new PagedResultDto<GetInternalApplicationDto>(totalCount, queryResult);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(GetPagedAndSortedInternalApplicationListAsync));
                throw new UserFriendlyException($"An exception was caught.{ex}");
            }
        }
    }
}
