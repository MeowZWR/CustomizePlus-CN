using Dalamud.Interface;
using Dalamud.Interface.Utility;
using ImGuiNET;
using OtterGui;
using OtterGui.Raii;
using System;
using System.Linq;
using System.Numerics;
using CustomizePlus.Profiles;
using CustomizePlus.Configuration.Data;
using CustomizePlus.Profiles.Data;
using CustomizePlus.UI.Windows.Controls;
using CustomizePlus.Templates;
using CustomizePlus.Core.Data;
using CustomizePlus.Templates.Events;

namespace CustomizePlus.UI.Windows.MainWindow.Tabs.Profiles;

public class ProfilePanel
{
    private readonly ProfileFileSystemSelector _selector;
    private readonly ProfileManager _manager;
    private readonly PluginConfiguration _configuration;
    private readonly TemplateCombo _templateCombo;
    private readonly TemplateEditorManager _templateEditorManager;
    private readonly TemplateEditorEvent _templateEditorEvent;

    private string? _newName;
    private string? _newCharacterName;
    private Profile? _changedProfile;

    private Action? _endAction;

    private int _dragIndex = -1;

    private string SelectionName
        => _selector.Selected == null ? "未选中" : _selector.IncognitoMode ? _selector.Selected.Incognito : _selector.Selected.Name.Text;

    public ProfilePanel(
        ProfileFileSystemSelector selector,
        ProfileManager manager,
        PluginConfiguration configuration,
        TemplateCombo templateCombo,
        TemplateEditorManager templateEditorManager,
        TemplateEditorEvent templateEditorEvent)
    {
        _selector = selector;
        _manager = manager;
        _configuration = configuration;
        _templateCombo = templateCombo;
        _templateEditorManager = templateEditorManager;
        _templateEditorEvent = templateEditorEvent;
    }

    public void Draw()
    {
        using var group = ImRaii.Group();
        if (_selector.SelectedPaths.Count > 1)
        {
            DrawMultiSelection();
        }
        else
        {
            DrawHeader();
            DrawPanel();
        }
    }

    private HeaderDrawer.Button LockButton()
        => _selector.Selected == null
            ? HeaderDrawer.Button.Invisible
            : _selector.Selected.IsWriteProtected
                ? new HeaderDrawer.Button
                {
                    Description = "使此配置可编辑。",
                    Icon = FontAwesomeIcon.Lock,
                    OnClick = () => _manager.SetWriteProtection(_selector.Selected!, false)
                }
                : new HeaderDrawer.Button
                {
                    Description = "锁定此配置。",
                    Icon = FontAwesomeIcon.LockOpen,
                    OnClick = () => _manager.SetWriteProtection(_selector.Selected!, true)
                };

    private void DrawHeader()
        => HeaderDrawer.Draw(SelectionName, 0, ImGui.GetColorU32(ImGuiCol.FrameBg),
            0, LockButton(),
            HeaderDrawer.Button.IncognitoButton(_selector.IncognitoMode, v => _selector.IncognitoMode = v));

    private void DrawMultiSelection()
    {
        if (_selector.SelectedPaths.Count == 0)
            return;

        var sizeType = ImGui.GetFrameHeight();
        var availableSizePercent = (ImGui.GetContentRegionAvail().X - sizeType - 4 * ImGui.GetStyle().CellPadding.X) / 100;
        var sizeMods = availableSizePercent * 35;
        var sizeFolders = availableSizePercent * 65;

        ImGui.NewLine();
        ImGui.TextUnformatted("当前选中的配置");
        ImGui.Separator();
        using var table = ImRaii.Table("profile", 3, ImGuiTableFlags.RowBg);
        ImGui.TableSetupColumn("btn", ImGuiTableColumnFlags.WidthFixed, sizeType);
        ImGui.TableSetupColumn("name", ImGuiTableColumnFlags.WidthFixed, sizeMods);
        ImGui.TableSetupColumn("path", ImGuiTableColumnFlags.WidthFixed, sizeFolders);

        var i = 0;
        foreach (var (fullName, path) in _selector.SelectedPaths.Select(p => (p.FullName(), p))
                     .OrderBy(p => p.Item1, StringComparer.OrdinalIgnoreCase))
        {
            using var id = ImRaii.PushId(i++);
            ImGui.TableNextColumn();
            var icon = (path is ProfileFileSystem.Leaf ? FontAwesomeIcon.FileCircleMinus : FontAwesomeIcon.FolderMinus).ToIconString();
            if (ImGuiUtil.DrawDisabledButton(icon, new Vector2(sizeType), "从选择中移除。", false, true))
                _selector.RemovePathFromMultiSelection(path);

            ImGui.TableNextColumn();
            ImGui.AlignTextToFramePadding();
            ImGui.TextUnformatted(path is ProfileFileSystem.Leaf l ? _selector.IncognitoMode ? l.Value.Incognito : l.Value.Name.Text : string.Empty);

            ImGui.TableNextColumn();
            ImGui.AlignTextToFramePadding();
            ImGui.TextUnformatted(_selector.IncognitoMode ? "匿名模式已激活" : fullName);
        }
    }

    private void DrawPanel()
    {
        using var child = ImRaii.Child("##Panel", -Vector2.One, true);
        if (!child || _selector.Selected == null)
            return;

        DrawEnabledSetting();
        using (var disabled = ImRaii.Disabled(_selector.Selected?.IsWriteProtected ?? true))
        {
            DrawBasicSettings();
            DrawTemplateArea();
        }
    }

    private void DrawEnabledSetting()
    {
        var spacing = ImGui.GetStyle().ItemInnerSpacing with { X = ImGui.GetStyle().ItemSpacing.X, Y = ImGui.GetStyle().ItemSpacing.Y };

        using (var style = ImRaii.PushStyle(ImGuiStyleVar.ItemSpacing, spacing))
        {
            var enabled = _selector.Selected?.Enabled ?? false;
            using (ImRaii.Disabled(_templateEditorManager.IsEditorActive || _templateEditorManager.IsEditorPaused))
            {
                if (ImGui.Checkbox("##Enabled", ref enabled))
                    _manager.SetEnabled(_selector.Selected!, enabled);
                ImGuiUtil.LabeledHelpMarker("启用",
                    "是否应用此角色配置中的模板。一个角色同时只能启用一个配置文件。");
            }

            ImGui.SameLine();
            var isDefault = _manager.DefaultProfile == _selector.Selected;
            var isDefaultOrCurrentProfilesEnabled = _manager.DefaultProfile?.Enabled ?? false || enabled;
            using (ImRaii.Disabled(isDefaultOrCurrentProfilesEnabled))
            {
                if (ImGui.Checkbox("##DefaultProfile", ref isDefault))
                    _manager.SetDefaultProfile(isDefault ? _selector.Selected! : null);
                ImGuiUtil.LabeledHelpMarker("默认配置（仅玩家和雇员）",
                    "是否将此配置应用于没有指定角色配置的所有玩家和雇员。同时只有一个角色配置可以设置为默认配置。");
            }
            if(isDefaultOrCurrentProfilesEnabled)
            {
                ImGui.SameLine();
                ImGui.PushStyleColor(ImGuiCol.Text, Constants.Colors.Warning);
                ImGuiUtil.PrintIcon(FontAwesomeIcon.ExclamationTriangle);
                ImGui.PopStyleColor();
                ImGuiUtil.HoverTooltip("只能在当前选定且禁用默认配置文件时更改。");
            }
        }
    }

    private void DrawBasicSettings()
    {
        using (var style = ImRaii.PushStyle(ImGuiStyleVar.ButtonTextAlign, new Vector2(0, 0.5f)))
        {
            using (var table = ImRaii.Table("BasicSettings", 2))
            {
                ImGui.TableSetupColumn("BasicCol1", ImGuiTableColumnFlags.WidthFixed, ImGui.CalcTextSize("lorem ipsum dolor").X);
                ImGui.TableSetupColumn("BasicCol2", ImGuiTableColumnFlags.WidthStretch);

                ImGuiUtil.DrawFrameColumn("配置名称");
                ImGui.TableNextColumn();
                var width = new Vector2(ImGui.GetContentRegionAvail().X, 0);
                var name = _newName ?? _selector.Selected!.Name;
                ImGui.SetNextItemWidth(width.X);

                if (!_selector.IncognitoMode)
                {
                    if (ImGui.InputText("##ProfileName", ref name, 128))
                    {
                        _newName = name;
                        _changedProfile = _selector.Selected;
                    }

                    if (ImGui.IsItemDeactivatedAfterEdit() && _changedProfile != null)
                    {
                        _manager.Rename(_changedProfile, name);
                        _newName = null;
                        _changedProfile = null;
                    }
                }
                else
                    ImGui.TextUnformatted(_selector.Selected!.Incognito);

                ImGui.TableNextRow();

                ImGuiUtil.DrawFrameColumn("角色名称");
                ImGui.TableNextColumn();
                width = new Vector2(ImGui.GetContentRegionAvail().X - ImGui.CalcTextSize("限制为从属于我的角色").X - 68, 0);
                name = _newCharacterName ?? _selector.Selected!.CharacterName;
                ImGui.SetNextItemWidth(width.X);

                if(_manager.DefaultProfile != _selector.Selected)
                {
                    if (!_selector.IncognitoMode)
                    {
                        if (ImGui.InputText("##CharacterName", ref name, 128))
                        {
                            _newCharacterName = name;
                            _changedProfile = _selector.Selected;
                        }

                        if (ImGui.IsItemDeactivatedAfterEdit() && _changedProfile != null)
                        {
                            _manager.ChangeCharacterName(_changedProfile, name);
                            _newCharacterName = null;
                            _changedProfile = null;
                        }
                    }
                    else
                        ImGui.TextUnformatted("匿名模式已激活");

                    ImGui.SameLine();
                    var enabled = _selector.Selected?.LimitLookupToOwnedObjects ?? false;
                    if (ImGui.Checkbox("##LimitLookupToOwnedObjects", ref enabled))
                        _manager.SetLimitLookupToOwned(_selector.Selected!, enabled);
                    ImGuiUtil.LabeledHelpMarker("限制为从属于我的角色",
                        "启用时，将角色搜索范围限制为仅您自己的召唤兽、坐骑和宠物。\n在可能会有另一个同名角色被另一个玩家拥有时使用此选项。\n*对于战斗陆行鸟请使用\"Chocobo\"作为角色名称。\n**如果您正在修改坐骑的根骨骼缩放，并希望保留您自己的缩放，请确保您自己的缩放是默认值以外的任何值。");
                }
                else
                    ImGui.TextUnformatted("All players and retainers");
            }
        }
    }

    private void DrawTemplateArea()
    {
        using var table = ImRaii.Table("SetTable", 4, ImGuiTableFlags.RowBg | ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
        if (!table)
            return;

        ImGui.TableSetupColumn("##del", ImGuiTableColumnFlags.WidthFixed, ImGui.GetFrameHeight());
        ImGui.TableSetupColumn("##Index", ImGuiTableColumnFlags.WidthFixed, 30 * ImGuiHelpers.GlobalScale);

        ImGui.TableSetupColumn("模板", ImGuiTableColumnFlags.WidthFixed, 220 * ImGuiHelpers.GlobalScale);

        ImGui.TableSetupColumn("##editbtn", ImGuiTableColumnFlags.WidthFixed, 120 * ImGuiHelpers.GlobalScale);

        ImGui.TableHeadersRow();

        //warn: .ToList() might be performance critical at some point
        //the copying via ToList is done because manipulations with .Templates list result in "Collection was modified" exception here
        foreach (var (template, idx) in _selector.Selected!.Templates.WithIndex().ToList())
        {
            using var id = ImRaii.PushId(idx);
            ImGui.TableNextColumn();
            var keyValid = _configuration.UISettings.DeleteTemplateModifier.IsActive();
            var tt = keyValid
                ? "从配置文件中删除此模板。"
                : $"从配置文件中删除此模板。\n同时按住{_configuration.UISettings.DeleteTemplateModifier}来删除。";

            if (ImGuiUtil.DrawDisabledButton(FontAwesomeIcon.Trash.ToIconString(), new Vector2(ImGui.GetFrameHeight()), tt, !keyValid, true))
                _endAction = () => _manager.DeleteTemplate(_selector.Selected!, idx);
            ImGui.TableNextColumn();
            ImGui.Selectable($"#{idx + 1:D2}");
            DrawDragDrop(_selector.Selected!, idx);
            ImGui.TableNextColumn();
            _templateCombo.Draw(_selector.Selected!, template, idx);
            DrawDragDrop(_selector.Selected!, idx);
            ImGui.TableNextColumn();

            var disabledCondition = _templateEditorManager.IsEditorActive || template.IsWriteProtected;

            if (ImGuiUtil.DrawDisabledButton(FontAwesomeIcon.Edit.ToIconString(), new Vector2(ImGui.GetFrameHeight()), "在模板编辑器中打开此模板", disabledCondition, true))
                _templateEditorEvent.Invoke(TemplateEditorEvent.Type.EditorEnableRequested, template);

            if (disabledCondition)
            {
                //todo: make helper
                ImGui.SameLine();
                ImGui.PushStyleColor(ImGuiCol.Text, Constants.Colors.Warning);
                ImGuiUtil.PrintIcon(FontAwesomeIcon.ExclamationTriangle);
                ImGui.PopStyleColor();
                ImGuiUtil.HoverTooltip("无法编辑此模板，因为它已被锁定，或您正在编辑其他模板。");
            }
        }

        ImGui.TableNextColumn();
        ImGui.TableNextColumn();
        ImGui.AlignTextToFramePadding();
        ImGui.TextUnformatted("新增");
        ImGui.TableNextColumn();
        _templateCombo.Draw(_selector.Selected!, null, -1);
        ImGui.TableNextRow();

        _endAction?.Invoke();
        _endAction = null;
    }

    private void DrawDragDrop(Profile profile, int index)
    {
        const string dragDropLabel = "TemplateDragDrop";
        using (var target = ImRaii.DragDropTarget())
        {
            if (target.Success && ImGuiUtil.IsDropping(dragDropLabel))
            {
                if (_dragIndex >= 0)
                {
                    var idx = _dragIndex;
                    _endAction = () => _manager.MoveTemplate(profile, idx, index);
                }

                _dragIndex = -1;
            }
        }

        using (var source = ImRaii.DragDropSource())
        {
            if (source)
            {
                ImGui.TextUnformatted($"移动模板 #{index + 1:D2}...");
                if (ImGui.SetDragDropPayload(dragDropLabel, nint.Zero, 0))
                {
                    _dragIndex = index;
                }
            }
        }
    }
}
