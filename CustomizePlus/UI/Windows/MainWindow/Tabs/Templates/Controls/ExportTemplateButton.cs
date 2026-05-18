using CustomizePlus.Core.Helpers;
using CustomizePlus.Templates;
using CustomizePlus.Templates.Data;

namespace CustomizePlus.UI.Windows.MainWindow.Tabs.Templates.Controls;

public sealed class ExportTemplateButton(TemplateFileSystem fileSystem, PopupSystem popupSystem) : BaseIconButton<AwesomeIcon>
{
    public override bool IsVisible
        => fileSystem.Selection.Selection is not null;

    public override AwesomeIcon Icon
        => LunaStyle.ToClipboardIcon;

    public override bool HasTooltip
        => true;

    public override void DrawTooltip()
        => Im.Text("复制当前模板到剪贴板。"u8);

    public override void OnClick()
    {
        var template = (Template)fileSystem.Selection.Selection!.Value;

        try
        {
            var text = Base64Helper.ExportTemplateToBase64(template);
            Im.Clipboard.Set(text);
            popupSystem.ShowPopup(PopupSystem.Messages.ClipboardDataNotLongTerm);
        }
        catch (Exception ex)
        {
            CustomizePlus.Logger.Error($"无法从模板 {template.UniqueId} 复制数据到剪贴板：{ex}");
            popupSystem.ShowPopup(PopupSystem.Messages.ActionError);
        }
    }
}
