using IntranetPortal.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace IntranetPortal.Permissions;

public class IntranetPortalPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var intranetProtalGroup = context.AddGroup(IntranetPortalPermissions.GroupName);
        //Define your own permissions here. Example:
        //myGroup.AddPermission(IntranetPortalPermissions.MyPermission1, L("Permission:MyPermission1"));
        intranetProtalGroup.AddPermission(IntranetPortalPermissions.Create, L("Permission:Create"));
        intranetProtalGroup.AddPermission(IntranetPortalPermissions.Delete, L("Permission:Delete"));
        intranetProtalGroup.AddPermission(IntranetPortalPermissions.Update, L("Permission:Update"));
        intranetProtalGroup.AddPermission(IntranetPortalPermissions.LogoUpload, L("Permission:LogoUpload"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<IntranetPortalResource>(name);
    }
}
