using CustomizePlus.Anamnesis;
using CustomizePlus.Core.Data;
using CustomizePlus.Templates;
using Dalamud.Interface;
using Dalamud.Interface.ImGuiFileDialog;
using Dalamud.Interface.ImGuiNotification;

namespace CustomizePlus.UI.Windows.MainWindow.Tabs.Templates.Controls;

public sealed class AnamnesisImportButton(
    TemplateManager templateManager,
    TemplateEditorManager editorManager,
    PopupSystem popupSystem,
    MessageService messageService,
    PoseFileBoneLoader poseFileBoneLoader,
    FileDialogManager fileDialogManager) : BaseIconButton<AwesomeIcon>
{
    public override AwesomeIcon Icon
        => FontAwesomeIcon.PersonFalling;

    public override bool HasTooltip
        => true;

    public override bool Enabled
        => true;

    public override void DrawTooltip()
        => Im.Text("从 Anamnesis 姿势文件导入模板（仅缩放）"u8);

    public override void OnClick()
    {
        if (editorManager.IsEditorActive)
        {
            popupSystem.ShowPopup(PopupSystem.Messages.TemplateEditorActiveWarning);
            return;
        }

        fileDialogManager.OpenFileDialog("导入姿势文件", ".pose", (isSuccess, path) =>
        {
            if (isSuccess)
            {
                var selectedFilePath = path.FirstOrDefault();
                if (selectedFilePath is null)
                    return;

                var bones = poseFileBoneLoader.LoadBoneTransformsFromFile(selectedFilePath);

                if (bones != null)
                {
                    if (bones.Count == 0)
                    {
                        messageService.NotificationMessage("选中的 Anamnesis 姿势文件不包含任何缩放骨骼", NotificationType.Error);
                        return;
                    }

                    var template = templateManager.Create(Path.GetFileNameWithoutExtension(selectedFilePath), bones, false);
                    templateManager.SetSource(template, DataSource.PoseImport);
                }
                else
                {
                    messageService.NotificationMessage(
                        $"解析 Anamnesis 姿势文件时出错：'{path}'", NotificationType.Error);
                }
            }
            else
            {
                Logger.GlobalPluginLogger.Debug(isSuccess + " 未选择有效文件。" + path);
            }
        }, 1, null, true);
    }
}