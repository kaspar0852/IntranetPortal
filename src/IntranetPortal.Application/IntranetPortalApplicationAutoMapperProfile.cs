
using AutoMapper;
using IntranetPortal.AppEntities;
using IntranetPortal.AppEntities.Documents;
using IntranetPortal.Departments.Dtos;
using IntranetPortal.Designations.Dtos;
using IntranetPortal.Documents.Dtos;
using IntranetPortal.InternalApplicationDto;

namespace IntranetPortal;

public class IntranetPortalApplicationAutoMapperProfile : Profile
{
    public IntranetPortalApplicationAutoMapperProfile()
    {
        CreateMap<InternalApplications.InternalApplication, GetInternalApplicationDto>().ReverseMap();
        CreateMap<Document, DocumentDto>().ReverseMap();
        CreateMap<Document,UpdateDocumentDto>().ReverseMap();
        CreateMap<DocumentStatus, GetDocumentStatusDto>().ReverseMap();
        CreateMap<DocumentAcknowledgementRequestStatuses, GetDocumentAcknowledgementRequestStatusDto>().ReverseMap();
        CreateMap<Department, GetDepartmentDto>().ReverseMap();
        CreateMap<Designation,GetDesignationDto>().ReverseMap();
    }
}
