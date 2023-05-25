
using AutoMapper;
using IntranetPortal.AppEntities.Documents;
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
    }
}
