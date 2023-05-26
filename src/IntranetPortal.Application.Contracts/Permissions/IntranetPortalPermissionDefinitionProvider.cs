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

        //Internal Application 
        intranetProtalGroup.AddPermission(IntranetPortalPermissions.InternalApplicationAdmin.Create, L("Permission:InternalApplicationAdmin.Create"));
        intranetProtalGroup.AddPermission(IntranetPortalPermissions.InternalApplicationAdmin.Delete, L("Permission:InternalApplicationAdmin.Delete"));
        intranetProtalGroup.AddPermission(IntranetPortalPermissions.InternalApplicationAdmin.Update, L("Permission:InternalApplicationAdmin.Update"));
        intranetProtalGroup.AddPermission(IntranetPortalPermissions.InternalApplicationAdmin.LogoUpload, L("Permission:InternalApplicationAdmin.LogoUpload"));

        //Document Application 
        /* intranetProtalGroup.AddPermission(IntranetPortalPermissions.DocumentAdmin.ActivateDocument, L("Permission:DocumentAdmin.ActivateDocument"));
         intranetProtalGroup.AddPermission(IntranetPortalPermissions.DocumentAdmin.DeactivateDocument, L("Permission:DocumentAdmin.DeactivateDocument"));*/
        var documentAdminPermission = intranetProtalGroup.AddPermission(IntranetPortalPermissions.DocumentAdmin.Default, L("Permission:DocumentAdmin"));
        documentAdminPermission.AddChild(IntranetPortalPermissions.DocumentAdmin.Create, L("Permission:DocumentAdmin.CreateDocument"));
        documentAdminPermission.AddChild(IntranetPortalPermissions.DocumentAdmin.Update, L("Permission:DocumentAdmin.UpdateDocument"));
        documentAdminPermission.AddChild(IntranetPortalPermissions.DocumentAdmin.Delete, L("Permission:DocumentAdmin.DeleteDocument"));
        documentAdminPermission.AddChild(IntranetPortalPermissions.DocumentAdmin.ActivateDocument, L("Permission:DocumentAdmin.ActivateDocument"));
        documentAdminPermission.AddChild(IntranetPortalPermissions.DocumentAdmin.DeactivateDocument, L("Permission:DocumentAdmin.DeactivateDocument"));
        documentAdminPermission.AddChild(IntranetPortalPermissions.DocumentAdmin.Upload, L("Permission:DocumentAdmin.UploadDocument"));

    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<IntranetPortalResource>(name);
    }
}
