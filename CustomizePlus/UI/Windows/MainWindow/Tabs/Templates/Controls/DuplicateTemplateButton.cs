using CustomizePlus.Templates;
using CustomizePlus.Templates.Data;

namespace CustomizePlus.UI.Windows.MainWindow.Tabs.Templates.Controls;

public sealed class DuplicateTemplateButton(
    TemplateFileSystem fileSystem,
    TemplateManager templateManager,
    TemplateEditorManager editorManager,
    PopupSystem popupSystem) : BaseIconButton<AwesomeIcon>
{
    private readonly WeakReference<Template> _template = new(null!);

    public override AwesomeIcon Icon
        => LunaStyle.DuplicateIcon;

    public override bool HasTooltip
        => true;

    public override bool Enabled
        => fileSystem.Selection.Selection is not null;

    public override void DrawTooltip()
        => Im.Text(fileSystem.Selection.Selection is null ? "未选中模板。"u8 : "复制当前选中的模板到新模板。"u8);

    public override void OnClick()
    {
        if (editorManager.IsEditorActive)
        {
            popupSystem.ShowPopup(PopupSystem.Messages.TemplateEditorActiveWarning);
            return;
        }

        _template.SetTarget(fileSystem.Selection.Selection?.GetValue<Template>()!);
        Im.Popup.Open("##CloneTemplate"u8);
    }

    protected override void PostDraw()
    {
        if (!InputPopup.OpenName("##CloneTemplate"u8, out var newName))
            return;

        if (_template.TryGetTarget(out var template))
            templateManager.Clone(template, newName, true);

        _template.SetTarget(null!);
    }
}