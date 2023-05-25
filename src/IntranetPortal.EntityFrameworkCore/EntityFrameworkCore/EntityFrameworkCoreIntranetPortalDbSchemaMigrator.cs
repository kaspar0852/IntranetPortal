using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using IntranetPortal.Data;
using Volo.Abp.DependencyInjection;

namespace IntranetPortal.EntityFrameworkCore;

public class EntityFrameworkCoreIntranetPortalDbSchemaMigrator
    : IIntranetPortalDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreIntranetPortalDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the IntranetPortalDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<IntranetPortalDbContext>()
            .Database
            .MigrateAsync();
    }
}
