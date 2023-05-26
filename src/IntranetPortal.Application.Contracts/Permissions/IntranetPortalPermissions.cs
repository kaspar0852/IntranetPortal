namespace IntranetPortal.Permissions;

public static class IntranetPortalPermissions
{
    public const string GroupName = "IntranetPortal";

    //Add your own permission names. Example:
    //public const string MyPermission1 = GroupName + ".MyPermission1";

    //InternalApplication Permissions
    public class InternalApplicationAdmin
    {
        public const string Default = GroupName + ".InternalApplicationAdmin";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string LogoUpload = Default + ".LogoUpload";
    }
    //Document Application permissions
    public class DocumentAdmin
    {
        public const string Default = GroupName + ".DocumentAdmin";
        public const string Create = Default + ".CreateDocument";
        public const string Update = Default + ".UpdateDocument";
        public const string Delete = Default + ".DeleteDocument";
        public const string Upload = Default + ".UploadDocument";
        public const string ActivateDocument = Default + ".ActivateDocument";
        public const string DeactivateDocument = Default + ".DeactivateDocument";
    }



}
