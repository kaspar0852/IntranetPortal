using IntranetPortal.AppEntities.Documents;
using System;
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
        private readonly IRepository<DocumentAcknowledgementRequestStatuses, Guid> _documentAcknowledgementRequestStatus;

        public DocumentStatusDataSeedContributor(IRepository<DocumentStatus, Guid> documentStatusRepository, IRepository<Document, Guid> documentRepository, IRepository<DocumentAcknowledgementRequestStatuses, Guid> documentAcknowledgementRequestStatus)
        {
            _documentStatusRepository = documentStatusRepository;
            _documentRepository = documentRepository;
            _documentAcknowledgementRequestStatus = documentAcknowledgementRequestStatus;
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

            if(await _documentAcknowledgementRequestStatus.GetCountAsync() <= 0)
            {
                await _documentAcknowledgementRequestStatus.InsertAsync(
                    new DocumentAcknowledgementRequestStatuses
                    {
                        SystemName = "New",
                        DisplayName = "New"
                    },autoSave: true
                    );
                await _documentAcknowledgementRequestStatus.InsertAsync(
                    new DocumentAcknowledgementRequestStatuses
                    {
                        SystemName = "Revoked",
                        DisplayName = "Revoked"
                    },autoSave : true
                    );
                await _documentAcknowledgementRequestStatus.InsertAsync(
                    new DocumentAcknowledgementRequestStatuses
                    {
                        SystemName = "Acknowledged",
                        DisplayName = "Acknowledged"
                    },autoSave : true
                    );
            }

/*            if(await _documentAcKnowledgementRequestStatusRepository.GetCountAsync() <= 0)
            {
                await _documentAcKnowledgementRequestStatusRepository.InsertAsync(
                    new DocumentAcknowledgementRequestStatuses
                    {
                        SystemName = "New",
                        DisplayName = "New"
                    },autoSave: true
                    );
                await _documentAcKnowledgementRequestStatusRepository.InsertAsync(
                    new DocumentAcknowledgementRequestStatuses
                    {
                        SystemName = "Revoked",
                        DisplayName = "Revoked"
                    },autoSave:true);
                await _documentAcKnowledgementRequestStatusRepository.InsertAsync(
                    new DocumentAcknowledgementRequestStatuses
                    {
                        SystemName = "Acknowledged",
                        DisplayName = "Acknowledged"
                    },autoSave:true);
            }*/

        }
    }
}

