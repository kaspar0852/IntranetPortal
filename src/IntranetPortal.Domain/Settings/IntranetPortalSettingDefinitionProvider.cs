using Volo.Abp.Settings;

namespace IntranetPortal.Settings;

public class IntranetPortalSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(IntranetPortalSettings.MySetting1));
    }
}
