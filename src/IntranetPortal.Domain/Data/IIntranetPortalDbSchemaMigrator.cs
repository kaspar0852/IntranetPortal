using System.Threading.Tasks;

namespace IntranetPortal.Data;

public interface IIntranetPortalDbSchemaMigrator
{
    Task MigrateAsync();
}
