using Dalamud.Interface.Utility;
using Dalamud.Interface;
using ImGuiNET;
using System.Numerics;
using CustomizePlus.Core.Services;
using CustomizePlus.Game.Services;
using CustomizePlus.Configuration.Data;
using CustomizePlus.UI.Windows.MainWindow.Tabs.Templates;
using CustomizePlus.Core.Helpers;
using CustomizePlus.Api;
using CustomizePlus.Core.Data;
using CustomizePlus.Core.Services.Dalamud;

namespace CustomizePlus.UI.Windows.Controls;

public class PluginStateBlock
{
    private readonly BoneEditorPanel _boneEditorPanel;
    private readonly PluginConfiguration _configuration;
    private readonly GameStateService _gameStateService;
    private readonly HookingService _hookingService;
    private readonly CustomizePlusIpc _ipcService;
    private readonly DalamudBranchService _dalamudBranchService;

    public PluginStateBlock(
        BoneEditorPanel boneEditorPanel,
        PluginConfiguration configuration,
        GameStateService gameStateService,
        HookingService hookingService,
        CustomizePlusIpc ipcService,
        DalamudBranchService dalamudBranchService)
    {
        _boneEditorPanel = boneEditorPanel;
        _configuration = configuration;
        _gameStateService = gameStateService;
        _hookingService = hookingService;
        _ipcService = ipcService;
        _dalamudBranchService = dalamudBranchService;
    }

    public void Draw(float yPos)
    {
        var severity = PluginStateSeverity.Normal;
        string? message = null;
        string? hoverInfo = null;

        if(_hookingService.RenderHookFailed || _hookingService.MovementHookFailed)
        {
            severity = PluginStateSeverity.Error;
            message = "检测到游戏钩子失效。Customize+ 已禁用。";
        }
        else if (!_configuration.PluginEnabled)
        {
            severity = PluginStateSeverity.Warning;
            message = "插件已禁用，骨骼编辑模板不可用。";
        }
        else if (_boneEditorPanel.IsEditorActive)
        {
            if (!_boneEditorPanel.IsCharacterFound)
            {
                severity = PluginStateSeverity.Error;
                message = $"未找到指定的预览角色。";
            }
            else
            {
                if (_boneEditorPanel.HasChanges)
                    severity = PluginStateSeverity.Warning;

                message = $"编辑器已激活。{(_boneEditorPanel.HasChanges ? "您有未保存的修改。点击“结束骨骼编辑”来进入保存/还原对话框。" : "")}";
            }
        }
        else if (_gameStateService.GameInPosingMode())
        {
            severity = PluginStateSeverity.Warning;
            message = "已进入集体动作。与姿势工具的兼容性有限。";
        }
        else if (_ipcService.IPCFailed) //this is a low priority error
        {
            severity = PluginStateSeverity.Error;
            message = "在IPC中检测到故障。与其他插件的集成将不起作用。";
        }
        else if (!_dalamudBranchService.AllowPluginToRun)
        {
            severity = PluginStateSeverity.Error;
            message = "您正在运行不受支持的 Dalamud 版本，悬停查看更多信息。";
            hoverInfo = "普通用户不应在 Dalamud 的开发或测试版本上运行 Customize+。\n此版本不受支持，因此 Customize+ 已自行禁用。";
        }
        else if(VersionHelper.IsTesting)
        {
            severity = PluginStateSeverity.Warning;
            message = "您正在运行 Customize+ 的测试版本，悬停查看更多信息。";
            hoverInfo = "这是 Customize+ 的测试版本。某些功能（例如与其他插件的集成），可能无法正常工作。";
        }

        if (message != null)
        {
            ImGui.SetCursorPos(new Vector2(ImGui.GetWindowContentRegionMax().X - ImGui.CalcTextSize(message).X - 30, yPos - ImGuiHelpers.GlobalScale));

            var icon = FontAwesomeIcon.InfoCircle;
            var color = Constants.Colors.Normal;
            switch (severity)
            {
                case PluginStateSeverity.Warning:
                    icon = FontAwesomeIcon.ExclamationTriangle;
                    color = Constants.Colors.Warning;
                    break;
                case PluginStateSeverity.Error:
                    icon = FontAwesomeIcon.ExclamationTriangle;
                    color = Constants.Colors.Error;
                    break;
            }

            ImGui.PushStyleColor(ImGuiCol.Text, color);
            CtrlHelper.LabelWithIcon(icon, message, false);
            ImGui.PopStyleColor();
            if (hoverInfo != null)
                CtrlHelper.AddHoverText(hoverInfo);
        }
    }

    private enum PluginStateSeverity
    {
        Normal,
        Warning,
        Error
    }
}
