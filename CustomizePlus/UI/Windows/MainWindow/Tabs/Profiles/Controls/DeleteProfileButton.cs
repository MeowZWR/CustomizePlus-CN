using CustomizePlus.Configuration.Data;
using CustomizePlus.Profiles;
using CustomizePlus.Profiles.Data;

namespace CustomizePlus.UI.Windows.MainWindow.Tabs.Profiles.Controls;

public sealed class DeleteProfileButton(
    ProfileFileSystem fileSystem,
    ProfileManager profileManager,
    PluginConfiguration config)
    : BaseIconButton<AwesomeIcon>
{
    /// <inheritdoc/>
    public override AwesomeIcon Icon
        => LunaStyle.DeleteIcon;

    /// <inheritdoc/>
    public override bool HasTooltip
        => true;

    /// <inheritdoc/>
    public override void DrawTooltip()
    {
        var anySelected = fileSystem.Selection.DataNodes.Count > 0;
        var modifier = Enabled;

        Im.Text(anySelected
            ? "从您的驱动器中完全删除当前选中的配置文件\n此操作无法撤销。"u8
            : "未选中配置文件。"u8);
        if (!modifier)
            Im.Text($"\n按住 {config.UISettings.DeleteModifier} 单击以删除配置文件。");
    }

    /// <inheritdoc/>
    public override bool Enabled
        => config.UISettings.DeleteModifier.IsActive() && fileSystem.Selection.DataNodes.Count > 0;

    /// <inheritdoc/>
    public override void OnClick()
    {
        var profiles = fileSystem.Selection.DataNodes.Select(n => n.Value).OfType<Profile>().ToList();
        fileSystem.Selection.UnselectAll();
        foreach (var profile in profiles)
            profileManager.Delete(profile);
    }
}
