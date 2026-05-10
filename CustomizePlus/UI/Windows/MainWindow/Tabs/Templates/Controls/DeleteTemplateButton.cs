using CustomizePlus.Configuration.Data;
using CustomizePlus.Templates;
using CustomizePlus.Templates.Data;

namespace CustomizePlus.UI.Windows.MainWindow.Tabs.Templates.Controls;

public sealed class DeleteTemplateButton(
    TemplateFileSystem fileSystem,
    TemplateManager templateManager,
    TemplateEditorManager editorManager,
    PopupSystem popupSystem, PluginConfiguration config)
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
            ? "从您的驱动器中完全删除当前选中的模板\n此操作无法撤销。"u8
            : "未选中模板。"u8);
        if (!modifier)
            Im.Text($"\n按住 {config.UISettings.DeleteModifier} 单击以删除模板。");
    }

    /// <inheritdoc/>
    public override bool Enabled
        => config.UISettings.DeleteModifier.IsActive() && fileSystem.Selection.DataNodes.Count > 0;

    /// <inheritdoc/>
    public override void OnClick()
    {
        if (editorManager.IsEditorActive)
        {
            popupSystem.ShowPopup(PopupSystem.Messages.TemplateEditorActiveWarning);
            return;
        }

        var templates = fileSystem.Selection.DataNodes.Select(n => n.Value).OfType<Template>().ToList();
        fileSystem.Selection.UnselectAll();
        foreach (var template in templates)
            templateManager.Delete(template);
    }
}
