using IntranetPortal.AppEntities.Documents;
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
    public class DocumentStatusDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IRepository<DocumentStatus, Guid> _documentStatusRepository;
        private readonly IRepository<Document, Guid> _documentRepository;

        public DocumentStatusDataSeedContributor(IRepository<DocumentStatus, Guid> documentStatusRepository, IRepository<Document, Guid> documentRepository)
        {
            _documentStatusRepository = documentStatusRepository;
            _documentRepository = documentRepository;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (await _documentStatusRepository.GetCountAsync() <= 0)
            {
                await _documentStatusRepository.InsertAsync(
                    new DocumentStatus
                    {
                        SystemName = "Active",
                        DisplayName = "Active"
                    },
                    autoSave: true
                    );
                await _documentStatusRepository.InsertAsync(
                    new DocumentStatus
                    {
                        SystemName = "Inactive",
                        DisplayName = "Cancel"
                    },
                    autoSave: true
                    );
            }
                var documentStatuses = await _documentStatusRepository.GetListAsync();
            

            if(await _documentRepository.GetCountAsync() <=0)
                {
                    await _documentRepository.InsertAsync(
                        new Document
                        {
                            Name = "CV",
                            Description = "this is the CV",
                            DocumentUrl = "string",
                            AcknowledgementDeadlineInDays = 0,
                            DocumentStatusId = documentStatuses[0].Id
                        },autoSave: true
                        );
                }
        }
    }
}

