using IntranetPortal.AppEntities;
using IntranetPortal.AppEntities.Documents;
using IntranetPortal.AppEntities.UserProfiles;
using System;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using System.Linq;

namespace IntranetPortal.DataSeed
{
    public class IntranetPortalDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IRepository<DocumentStatus, Guid> _documentStatusRepository;
        private readonly IRepository<Document, Guid> _documentRepository;
        private readonly IRepository<DocumentAcknowledgementRequestStatuses, Guid> _documentAcknowledgementRequestStatus;
        private readonly IRepository<Department, Guid> _department;
        private readonly IRepository<Designation, Guid> _designation;
        private readonly IRepository<UserProfile, Guid> _userProfileRepository;
        private readonly IRepository<UserProfileStatus, Guid> _userProfileStatusRepository;
        private readonly IRepository<IdentityUser, Guid> _userRepository;
        public IntranetPortalDataSeedContributor(
            IRepository<Department, Guid> department,
            IRepository<Designation, Guid> designation,
            IRepository<DocumentStatus, Guid> documentStatusRepository,
            IRepository<UserProfile, Guid> userProfileRepository,
            IRepository<Document, Guid> documentRepository,
            IRepository<UserProfileStatus, Guid> userProfileStatusRepository,
            IRepository<DocumentAcknowledgementRequestStatuses, Guid> documentAcknowledgementRequestStatus,
            IRepository<IdentityUser, Guid> userRepository)
        {
            _documentStatusRepository = documentStatusRepository;
            _documentRepository = documentRepository;
            _documentAcknowledgementRequestStatus = documentAcknowledgementRequestStatus;
            _department = department;
            _designation = designation;
            _userProfileRepository = userProfileRepository;
            _userProfileStatusRepository = userProfileStatusRepository;
            _userRepository = userRepository;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            var identityUser1 = (await _userRepository.GetQueryableAsync()).Where(x =>x.Name == "admin").First();
            var identityUser2 = (await _userRepository.GetQueryableAsync()).Where(x => x.UserName == "Saugat").First();
            var identityUser3 = (await _userRepository.GetQueryableAsync()).Where(x => x.UserName == "Max").First();
            var identityUser4 = (await _userRepository.GetQueryableAsync()).Where(x => x.UserName == "Prabin").First();
            var identityUser5 = (await _userRepository.GetQueryableAsync()).Where(x => x.UserName == "Sujan").First();
            var identityUser6 = (await _userRepository.GetQueryableAsync()).Where(x => x.UserName == "Joey").First();
            var identityUser7 = (await _userRepository.GetQueryableAsync()).Where(x => x.UserName == "Chandler").First();
            var identityUser8 = (await _userRepository.GetQueryableAsync()).Where(x => x.UserName == "Phil").First();
            //DocumentStatus
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
            //Document
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
            var documents = await _documentRepository.GetListAsync();

            //DocumentAcknowledgementRequest
 /*           if(await _documentAcknowledgementRequestStatus.GetCountAsync() <= 0)
            {
                await _documentAcknowledgementRequestStatus.InsertAsync(
                    new A.DocumentAcknowledgementRequestStatuses
                    {
                        SystemName = "New",
                        DisplayName = "New"
                    },autoSave: true
                    );
                await _documentAcknowledgementRequestStatus.InsertAsync(
                    new AppEntities.Documents.DocumentAcknowledgementRequestStatus
                    {
                        SystemName = "Revoked",
                        DisplayName = "Revoked"
                    },autoSave : true
                    );
                await _documentAcknowledgementRequestStatus.InsertAsync(
                    new AppEntities.Documents.DocumentAcknowledgementRequestStatus
                    {
                        SystemName = "Acknowledged",
                        DisplayName = "Acknowledged"
                    },autoSave : true
                    );
            }
            var documentAcknowledgementRequests = await _documentAcknowledgementRequestStatus.GetListAsync();*/

            //Department
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
            var departments = await _department.GetListAsync();

            //Designation
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
            var designations = await _designation.GetListAsync();

            //UserStatus
            if (await _userProfileStatusRepository.GetCountAsync() <= 0)
            {
                var UserStatus1 = new UserProfileStatus
                {
                    SystemName = "Active",
                    DisplayName = "Active"
                };

                var UserStatus2 = new UserProfileStatus
                {
                    SystemName = "Inactive",
                    DisplayName = "Inactive"
                };

                await _userProfileStatusRepository.InsertManyAsync(new[] { UserStatus1, UserStatus2 }, autoSave: true);
            }
            var userStatuses = await _userProfileStatusRepository.GetListAsync();

            //UserProfile
            if (await _userProfileRepository.GetCountAsync() <= 0)
            {
                var UserProfile1 = new UserProfile
                {
                    AbpUserId = identityUser1.Id,
                    DesignationId = designations[0].Id,
                    DepartmentId = departments[1].Id,
                    DateOfBirth = DateTime.Now.AddDays(-100000),
                    ProfilePictureUrl = "http://ADMIN.jpg",
                    HiredDate = DateTime.Now.AddDays(-90),
                    ReferenceId = "2020",
                    MiddleName = "Raj",
                    UserProfileStatusId = userStatuses[0].Id
                };
                var UserProfile2 = new UserProfile
                {
                    AbpUserId = identityUser2.Id,
                    DesignationId = designations[1].Id,
                    DepartmentId = departments[0].Id,
                    DateOfBirth = DateTime.Now.AddDays(-200000),
                    ProfilePictureUrl = "http://saugat.jpg",
                    HiredDate = DateTime.Now.AddDays(-80),
                    ReferenceId = "2021",
                    MiddleName = "Kumari",
                    UserProfileStatusId = userStatuses[0].Id
                };
                var UserProfile3 = new UserProfile
                {
                    AbpUserId = identityUser3.Id,
                    DesignationId = designations[0].Id,
                    DepartmentId = departments[1].Id,
                    DateOfBirth = DateTime.Now.AddDays(-500000),
                    ProfilePictureUrl = "http://max.jpg",
                    HiredDate = DateTime.Now.AddDays(-70),
                    ReferenceId = "2022",
                    MiddleName = "Sharma",
                    UserProfileStatusId = userStatuses[1].Id
                };
                var UserProfile4 = new UserProfile
                {
                    AbpUserId = identityUser4.Id,
                    DesignationId = designations[1].Id,
                    DepartmentId = departments[1].Id,
                    DateOfBirth = DateTime.Now.AddDays(-100000),
                    ProfilePictureUrl = "http://Prabin.jpg",
                    HiredDate = DateTime.Now.AddDays(-10),
                    ReferenceId = "2023",
                    MiddleName = "Falula",
                    UserProfileStatusId = userStatuses[1].Id
                };
                var UserProfile5 = new UserProfile
                {
                    AbpUserId = identityUser5.Id,
                    DesignationId = designations[1].Id,
                    DepartmentId = departments[0].Id,
                    DateOfBirth = DateTime.Now.AddDays(-300000),
                    ProfilePictureUrl = "http://sujan.jpg",
                    HiredDate = DateTime.Now.AddDays(-30),
                    ReferenceId = "2024",
                    MiddleName = "Sha",
                    UserProfileStatusId = userStatuses[0].Id
                };
                var UserProfile6 = new UserProfile
                {
                    AbpUserId = identityUser6.Id,
                    DesignationId = designations[0].Id,
                    DepartmentId = departments[0].Id,
                    DateOfBirth = DateTime.Now.AddDays(-500000),
                    ProfilePictureUrl = "http://Joey.jpg",
                    HiredDate = DateTime.Now.AddDays(-50),
                    ReferenceId = "2025",
                    MiddleName = "Mureal",
                    UserProfileStatusId = userStatuses[1].Id
                };
                var UserProfile7 = new UserProfile
                {
                    AbpUserId = identityUser7.Id,
                    DesignationId = designations[0].Id,
                    DepartmentId = departments[1].Id,
                    DateOfBirth = DateTime.Now.AddDays(-700000),
                    ProfilePictureUrl = "http://Chandler.jpg",
                    HiredDate = DateTime.Now.AddDays(-20),
                    ReferenceId = "2026",
                    MiddleName = "Bob",
                    UserProfileStatusId = userStatuses[0].Id
                };
                var UserProfile8 = new UserProfile
                {
                    AbpUserId = identityUser8.Id,
                    DesignationId = designations[0].Id,
                    DepartmentId = departments[1].Id,
                    DateOfBirth = DateTime.Now.AddDays(-500000),
                    ProfilePictureUrl = "http://phil.jpg",
                    HiredDate = DateTime.Now.AddDays(-25),
                    ReferenceId = "2027",
                    MiddleName = "Super",
                    UserProfileStatusId = userStatuses[1].Id
                };

                await _userProfileRepository.InsertManyAsync(new[] { UserProfile1, UserProfile2,UserProfile3, UserProfile4,
                    UserProfile5,UserProfile6,UserProfile7,UserProfile8 }, autoSave: true);

            }
        }
    }
}   

