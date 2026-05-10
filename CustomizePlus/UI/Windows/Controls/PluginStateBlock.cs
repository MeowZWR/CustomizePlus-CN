using CustomizePlus.Api;
using CustomizePlus.Configuration.Data;
using CustomizePlus.Core.Data;
using CustomizePlus.Core.Helpers;
using CustomizePlus.Core.Services;
using CustomizePlus.Game.Services;
using CustomizePlus.UI.Windows.MainWindow.Tabs.Templates;
using Dalamud.Interface;

namespace CustomizePlus.UI.Windows.Controls;

public class PluginStateBlock
{
    private readonly BoneEditorPanel _boneEditorPanel;
    private readonly PluginConfiguration _configuration;
    private readonly GameStateService _gameStateService;
    private readonly HookingService _hookingService;
    private readonly CustomizePlusIpc _ipcService;
    private readonly PcpService _pcpService;

    public PluginStateBlock(
        BoneEditorPanel boneEditorPanel,
        PluginConfiguration configuration,
        GameStateService gameStateService,
        HookingService hookingService,
        CustomizePlusIpc ipcService,
        PcpService pcpService)
    {
        _boneEditorPanel = boneEditorPanel;
        _configuration = configuration;
        _gameStateService = gameStateService;
        _hookingService = hookingService;
        _ipcService = ipcService;
        _pcpService = pcpService;
    }

    public void Draw(float yPos, float messageLeftEdge)
    {
        var severity = PluginStateSeverity.Normal;
        string? message = null;
        string? hoverInfo = null;

        if (_hookingService.RenderHookFailed || _hookingService.MovementHookFailed)
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

                message = $"编辑器已激活。{(_boneEditorPanel.HasChanges ? "您有未保存的修改。点击“结束骨骼编辑”来进入保存/还原对话框。" : string.Empty)}";
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
        else if (_pcpService.IsEnabled && !_pcpService.IsPenumbraAvailable)
        {
            severity = PluginStateSeverity.Error;
            message = "Unable to connect to Penumbra. PCP integration will not function.";
        }
        else if (VersionHelper.IsTesting)
        {
            severity = PluginStateSeverity.Warning;
            message = "您正在运行 Customize+ 的测试版本，悬停查看更多信息。";
            hoverInfo = "这是 Customize+ 的测试版本。某些功能（例如与其他插件的集成），可能无法正常工作。";
        }

        if (message != null)
        {
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

            DrawMessage(icon, message, hoverInfo, color, yPos, messageLeftEdge);
        }
    }

    private static void DrawMessage(FontAwesomeIcon icon, string message, string? hoverInfo, Vector4 color, float yPos, float messageLeftEdge)
    {
        var itemSpacing = Im.Style.ItemSpacing.X;
        var rightEdge = Im.Window.MaximumContentRegion.X - itemSpacing;
        var availableWidth = MathF.Max(0, rightEdge - messageLeftEdge);
        var iconWidth = icon.CalculateSize().X;

        if (availableWidth < iconWidth)
            return;

        var textWidth = MathF.Max(0, availableWidth - iconWidth - itemSpacing);
        var visibleMessage = TruncateTextToWidth(message, textWidth);
        var visibleMessageWidth = Im.Font.CalculateSize(visibleMessage, false).X;
        var hasVisibleMessage = visibleMessage.Length > 0;
        var messageWidth = hasVisibleMessage
            ? iconWidth + itemSpacing + visibleMessageWidth
            : iconWidth;
        var messageWasTrimmed = visibleMessage.Length != message.Length;
        var x = MathF.Max(messageLeftEdge, rightEdge - messageWidth);

        Im.Cursor.Position = new Vector2(x, yPos + Im.Style.FramePadding.Y);

        using var textColor = ImGuiColor.Text.Push(color);
        using (var group = Im.Group())
        {
            icon.Draw();
            if (hasVisibleMessage)
            {
                Im.Line.Same();

                Im.Cursor.Position = new Vector2(x + 25, yPos + Im.Style.FramePadding.Y);
                Im.Text(visibleMessage);
            }
        }

        if (hoverInfo != null || messageWasTrimmed)
            CtrlHelper.AddHoverText(hoverInfo == null ? message : $"{message}\n\n{hoverInfo}");
    }

    private static string TruncateTextToWidth(string text, float maxWidth)
    {
        if (maxWidth <= 0)
            return string.Empty;

        if (Im.Font.CalculateSize(text, false).X <= maxWidth)
            return text;

        var ellipsis = "...";
        var ellipsisWidth = Im.Font.CalculateSize(ellipsis, false).X;
        if (ellipsisWidth >= maxWidth)
            return string.Empty;

        var low = 0;
        var high = text.Length;
        while (low < high)
        {
            var mid = (low + high + 1) / 2;
            if (Im.Font.CalculateSize(text[..mid], false).X + ellipsisWidth <= maxWidth)
                low = mid;
            else
                high = mid - 1;
        }

        return string.Concat(text.AsSpan(0, low), ellipsis.AsSpan());
    }

    private enum PluginStateSeverity
    {
        Normal,
        Warning,
        Error
    }
}
