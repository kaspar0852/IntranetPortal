using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;

namespace IntranetPortal;

[Dependency(ReplaceServices = true)]
public class IntranetPortalBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "IntranetPortal";
}
