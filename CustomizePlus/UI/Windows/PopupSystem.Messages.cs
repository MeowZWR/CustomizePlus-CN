using System.Numerics;

namespace CustomizePlus.UI.Windows;

public partial class PopupSystem
{
    public static class Messages
    {
        public const string ActionError = "action_error";

        public const string FantasiaPlusDetected = "fantasia_detected_warn";

        public const string IPCProfileRemembered = "ipc_profile_remembered";
        public const string IPCGetProfileByIdRemembered = "ipc_get_profile_by_id_remembered";
        public const string IPCSetProfileToChrDone = "ipc_set_profile_to_character_done";
        public const string IPCRevertDone = "ipc_revert_done";
        public const string IPCCopiedToClipboard = "ipc_copied_to clipboard";
        public const string IPCEnableProfileByIdDone = "ipc_enable_profile_by_id_done";
        public const string IPCDisableProfileByIdDone = "ipc_disable_profile_by_id_done";

        public const string TemplateEditorActiveWarning = "template_editor_active_warn";
        public const string ClipboardDataUnsupported = "clipboard_data_unsupported_version";

        public const string ClipboardDataNotLongTerm = "clipboard_data_not_longterm";
    }

    private void RegisterMessages()
    {
        RegisterPopup(Messages.ActionError, "执行所选操作时出错。\n详细信息已打印到卫月日志（聊天命令为/xllog）。");

        RegisterPopup(Messages.FantasiaPlusDetected, "Customize+检测到您安装了Fantasia+。\n请删除或关闭它后重新启动游戏，以继续使用Customize+.");

        RegisterPopup(Messages.IPCProfileRemembered, "当前配置文件已复制到内存中。");
        RegisterPopup(Messages.IPCGetProfileByIdRemembered, "GetProfileByUniqueId 结果已复制到内存中");
        RegisterPopup(Messages.IPCSetProfileToChrDone, "SetProfileToCharacter 已使用内存中的数据调用， 配置文件ID已打印到日志");
        RegisterPopup(Messages.IPCRevertDone, "DeleteTemporaryProfileByUniqueId 已调用回滚");
        RegisterPopup(Messages.IPCCopiedToClipboard, "复制到剪贴板");
        RegisterPopup(Messages.IPCEnableProfileByIdDone, "按ID启用配置文件已被调用");
        RegisterPopup(Messages.IPCDisableProfileByIdDone, "按ID禁用配置文件已被调用");

        RegisterPopup(Messages.TemplateEditorActiveWarning, "执行此操作之前，您需要结束骨骼编辑");
        RegisterPopup(Messages.ClipboardDataUnsupported, "无法在此版本的Customize+中使用此剪贴板数据。");

        RegisterPopup(Messages.ClipboardDataNotLongTerm, "警告：剪贴板数据不是用来长期存储模板的。\n不保证复制的数据在不同的Customize+版本之间的兼容性。", true, new Vector2(5, 10));
    }
}
