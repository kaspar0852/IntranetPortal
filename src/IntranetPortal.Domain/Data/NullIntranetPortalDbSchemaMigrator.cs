using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace IntranetPortal.Data;

/* This is used if database provider does't define
 * IIntranetPortalDbSchemaMigrator implementation.
 */
public class NullIntranetPortalDbSchemaMigrator : IIntranetPortalDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
