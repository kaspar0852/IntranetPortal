using IntranetPortal.InternalApplications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace IntranetPortal.DataSeed
{
    public class InternalApplicationDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IRepository<InternalApplications.InternalApplication, Guid> _internalApplicationRepository;

        public InternalApplicationDataSeedContributor(IRepository<InternalApplication, Guid> internalApplicationRepository)
        {
            _internalApplicationRepository = internalApplicationRepository;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (await _internalApplicationRepository.GetCountAsync() <= 0)
            {
                await _internalApplicationRepository.InsertAsync(
                    new InternalApplications.InternalApplication
                    {
                        Name = "Saugat",
                        DisplayName = "Sau",
                        Description = "Test",
                        OrderNumber = 2,
                        IsActive = true,
                        LogoUrl = "string",
                        ApplicationUrl = "string"
                    },
                    autoSave : true
                    );
                await _internalApplicationRepository.InsertAsync(
                    new InternalApplications.InternalApplication
                    {
                        Name = "Prabin",
                        DisplayName = "Prab",
                        Description = "Test",
                        OrderNumber = 1,
                        IsActive = true,
                        LogoUrl = "string",
                        ApplicationUrl = "string"
                    },
                    autoSave: true
                    );
            }
        }
    }
}
