using Volo.Abp.Modularity;

namespace IntranetPortal;

[DependsOn(
    typeof(IntranetPortalApplicationModule),
    typeof(IntranetPortalDomainTestModule)
    )]
public class IntranetPortalApplicationTestModule : AbpModule
{

}
