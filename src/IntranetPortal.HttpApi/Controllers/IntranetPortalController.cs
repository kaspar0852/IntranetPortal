using IntranetPortal.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace IntranetPortal.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class IntranetPortalController : AbpControllerBase
{
    protected IntranetPortalController()
    {
        LocalizationResource = typeof(IntranetPortalResource);
    }
}
