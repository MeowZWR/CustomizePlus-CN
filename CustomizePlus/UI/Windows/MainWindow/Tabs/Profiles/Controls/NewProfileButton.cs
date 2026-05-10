using CustomizePlus.Profiles;

namespace CustomizePlus.UI.Windows.MainWindow.Tabs.Profiles.Controls;

public sealed class NewProfileButton(ProfileManager profileManager) : BaseIconButton<AwesomeIcon>
{
    public override AwesomeIcon Icon
        => LunaStyle.AddObjectIcon;

    public override bool HasTooltip
        => true;

    public override void DrawTooltip()
        => Im.Text("新建一个具有默认配置的配置文件。"u8);

    public override void OnClick()
    {
        Im.Popup.Open("##NewProfile"u8);
    }

    protected override void PostDraw()
    {
        if (!InputPopup.OpenName("##NewProfile"u8, out var newName))
            return;

        profileManager.Create(newName, true);
    }
}
