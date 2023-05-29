using IntranetPortal.AppEntities;
using IntranetPortal.AppEntities.Documents;
using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace IntranetPortal.DataSeed
{
    public class IntranetPortalDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IRepository<DocumentStatus, Guid> _documentStatusRepository;
        private readonly IRepository<Document, Guid> _documentRepository;
        private readonly IRepository<DocumentAcknowledgementRequestStatuses, Guid> _documentAcknowledgementRequestStatus;
        private readonly IRepository<Department, Guid> _department;
        private readonly IRepository<Designation, Guid> _designation;

        public IntranetPortalDataSeedContributor(
            IRepository<Department,Guid> department,
            IRepository<Designation,Guid> designation,
            IRepository<DocumentStatus, Guid> documentStatusRepository, 
            IRepository<Document, Guid> documentRepository,
            IRepository<DocumentAcknowledgementRequestStatuses, Guid> documentAcknowledgementRequestStatus)
        {
            _documentStatusRepository = documentStatusRepository;
            _documentRepository = documentRepository;
            _documentAcknowledgementRequestStatus = documentAcknowledgementRequestStatus;
            _department = department;
            _designation = designation;

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

            if(await _department.GetCountAsync() <= 0)
            {
                await _department.InsertAsync(
                    new Department
                    {
                        Name = "Development",
                        ReferenceId = "dev101",
                        Code = "101",
                        IsActive = true
                    },autoSave : true);
                await _department.InsertAsync(
                    new Department
                    {
                        Name = "QA",
                        ReferenceId = "qa101",
                        Code = "101",
                        IsActive = true
                    },autoSave : true);
            }

            if (await _designation.GetCountAsync() <= 0)
            {
                await _designation.InsertAsync(
                    new Designation
                    {
                        Name = "Intern",
                        ReferenceId = "200",
                        Code = "200",
                        IsActive = true
                    }, autoSave: true);
                await _designation.InsertAsync(
                    new Designation
                    {
                        Name = "Associate Software Engineer",
                        ReferenceId = "dev100",
                        Code = "100",
                        IsActive = true
                    }, autoSave: true);
            }

        }
    }
}   

