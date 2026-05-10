using CustomizePlus.Configuration.Data;

namespace CustomizePlus.UI.Windows.MainWindow.Tabs;

public sealed class IncognitoButton(PluginConfiguration config) : BaseIconButton<AwesomeIcon>, IUiService
{
    public override AwesomeIcon Icon
        => config.UISettings.IncognitoMode
            ? LunaStyle.IncognitoOn
            : LunaStyle.IncognitoOff;

    public override bool HasTooltip
        => true;

    public override void DrawTooltip()
    {
        Im.Text(config.UISettings.IncognitoMode ? "关闭匿名模式。"u8 : "启用匿名模式。"u8);
    }

    public override void OnClick()
    {
        config.UISettings.IncognitoMode = !config.UISettings.IncognitoMode;
    }
}
