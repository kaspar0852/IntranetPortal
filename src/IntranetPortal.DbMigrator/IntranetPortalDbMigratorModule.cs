using IntranetPortal.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace IntranetPortal.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(IntranetPortalEntityFrameworkCoreModule),
    typeof(IntranetPortalApplicationContractsModule)
    )]
public class IntranetPortalDbMigratorModule : AbpModule
{

}
