using System;
using System.Collections.Generic;
using System.Text;
using IntranetPortal.Localization;
using Volo.Abp.Application.Services;

namespace IntranetPortal;

/* Inherit your application services from this class.
 */
public abstract class IntranetPortalAppService : ApplicationService
{
    protected IntranetPortalAppService()
    {
        LocalizationResource = typeof(IntranetPortalResource);
    }
}
