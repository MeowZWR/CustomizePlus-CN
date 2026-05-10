namespace CustomizePlus.UI.Windows;

public partial class PopupSystem
{
    public static class Messages
    {
        public const string ActionError = "action_error";
        public const string ActionDone = "action_done";

        public const string FantasiaPlusDetected = "fantasia_detected_warn";

        public const string IPCProfileRemembered = "ipc_profile_remembered";
        public const string IPCGetProfileByIdRemembered = "ipc_get_profile_by_id_remembered";
        public const string IPCSetProfileToChrDone = "ipc_set_profile_to_character_done";
        public const string IPCRevertDone = "ipc_revert_done";
        public const string IPCCopiedToClipboard = "ipc_copied_to clipboard";
        public const string IPCSuccessfullyExecuted = "ipc_successfully_executed";
        public const string IPCEnableProfileByIdDone = "ipc_enable_profile_by_id_done";
        public const string IPCDisableProfileByIdDone = "ipc_disable_profile_by_id_done";

        public const string TemplateEditorActiveWarning = "template_editor_active_warn";
        public const string ClipboardDataUnsupported = "clipboard_data_unsupported_version";

        public const string ClipboardDataNotLongTerm = "clipboard_data_not_longterm";

        public const string PluginDisabledNonReleaseDalamud = "non_release_dalamud";
    }

    private void RegisterMessages()
    {
        RegisterPopup(Messages.ActionError, "操作失败", "执行所选操作时出错。\n详细信息已打印到卫月日志（聊天命令为 /xllog）。");
        RegisterPopup(Messages.ActionDone, "操作完成", "操作已成功执行。");

        RegisterPopup(Messages.FantasiaPlusDetected, "检测到 Fantasia+", "Customize+ 检测到您已安装 Fantasia+。\n请删除或禁用该插件并重启游戏后再使用 Customize+。");

        RegisterPopup(Messages.IPCProfileRemembered, "配置文件已复制", "当前配置文件已复制到内存。");
        RegisterPopup(Messages.IPCGetProfileByIdRemembered, "配置文件已复制", "GetProfileByUniqueId 的结果已复制到内存。");
        RegisterPopup(Messages.IPCSetProfileToChrDone, "IPC 已执行", "已使用内存中的数据调用 SetProfileToCharacter；配置文件 ID 已写入日志。");
        RegisterPopup(Messages.IPCRevertDone, "IPC 已执行", "已调用 DeleteTemporaryProfileByUniqueId 进行回滚。");
        RegisterPopup(Messages.IPCCopiedToClipboard, "已复制", "已复制到剪贴板。");
        RegisterPopup(Messages.IPCSuccessfullyExecuted, "IPC 已执行", "已成功执行。");
        RegisterPopup(Messages.IPCEnableProfileByIdDone, "IPC 已执行", "已调用按 ID 启用配置文件。");
        RegisterPopup(Messages.IPCDisableProfileByIdDone, "IPC 已执行", "已调用按 ID 禁用配置文件。");

        RegisterPopup(Messages.TemplateEditorActiveWarning, "骨骼编辑进行中", "请先结束骨骼编辑后再执行此操作。");
        RegisterPopup(Messages.ClipboardDataUnsupported, "不支持的剪贴板数据", "您尝试使用的剪贴板数据无法在此版本的 Customize+ 中使用。");

        RegisterPopup(Messages.ClipboardDataNotLongTerm, "剪贴板警告", "剪贴板数据并非用于长期保存模板。\n无法保证不同 Customize+ 版本之间复制数据的兼容性。", true, new Vector2(5, 10));
    }
}
