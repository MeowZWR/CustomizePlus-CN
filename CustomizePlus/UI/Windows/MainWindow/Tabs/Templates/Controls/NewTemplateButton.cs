using CustomizePlus.Templates;

namespace CustomizePlus.UI.Windows.MainWindow.Tabs.Templates.Controls;


public sealed class NewTemplateButton(
    TemplateManager templateManager,
    TemplateEditorManager editorManager,
    PopupSystem popupSystem) : BaseIconButton<AwesomeIcon>
{
    public override AwesomeIcon Icon
        => LunaStyle.AddObjectIcon;

    public override bool HasTooltip
        => true;

    public override void DrawTooltip()
        => Im.Text("新建一个具有默认配置的模板。"u8);

    public override void OnClick()
    {
        if (editorManager.IsEditorActive)
        {
            popupSystem.ShowPopup(PopupSystem.Messages.TemplateEditorActiveWarning);
            return;
        }

        Im.Popup.Open("##NewTemplate"u8);
    }

    protected override void PostDraw()
    {
        if (!InputPopup.OpenName("##NewTemplate"u8, out var newName))
            return;

        templateManager.Create(newName, true);
    }
}
