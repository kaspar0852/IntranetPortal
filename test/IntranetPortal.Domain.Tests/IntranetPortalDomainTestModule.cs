using IntranetPortal.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace IntranetPortal;

[DependsOn(
    typeof(IntranetPortalEntityFrameworkCoreTestModule)
    )]
public class IntranetPortalDomainTestModule : AbpModule
{

}
