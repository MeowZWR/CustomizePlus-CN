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
using Penumbra.GameData.Actors;
using Penumbra.String;
using static FFXIVClientStructs.FFXIV.Client.LayoutEngine.ILayoutInstance;
using CustomizePlus.GameData.Extensions;
using CustomizePlus.Core.Extensions;
using Dalamud.Interface.Components;

namespace CustomizePlus.UI.Windows.MainWindow.Tabs.Profiles;

public class ProfilePanel
{
    private readonly ProfileFileSystemSelector _selector;
    private readonly ProfileManager _manager;
    private readonly PluginConfiguration _configuration;
    private readonly TemplateCombo _templateCombo;
    private readonly TemplateEditorManager _templateEditorManager;
    private readonly ActorAssignmentUi _actorAssignmentUi;
    private readonly ActorManager _actorManager;
    private readonly TemplateEditorEvent _templateEditorEvent;

    private string? _newName;
    private int? _newPriority;
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
        ActorAssignmentUi actorAssignmentUi,
        ActorManager actorManager,
        TemplateEditorEvent templateEditorEvent)
    {
        _selector = selector;
        _manager = manager;
        _configuration = configuration;
        _templateCombo = templateCombo;
        _templateEditorManager = templateEditorManager;
        _actorAssignmentUi = actorAssignmentUi;
        _actorManager = actorManager;
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

        ImGui.Separator();

        using (var disabled = ImRaii.Disabled(_selector.Selected?.IsWriteProtected ?? true))
        {
            DrawBasicSettings();

            ImGui.Separator();

            var isShouldDraw = ImGui.CollapsingHeader("添加角色");

            if (isShouldDraw)
                DrawAddCharactersArea();

            ImGui.Separator();

            DrawCharacterListArea();

            ImGui.Separator();

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
                    "是否需要完全应用此配置文件中的模板。");
            }
        }
    }

    private void DrawBasicSettings()
    {
        using (var style = ImRaii.PushStyle(ImGuiStyleVar.ButtonTextAlign, new Vector2(0, 0.5f)))
        {
            using (var table = ImRaii.Table("BasicSettings", 2))
            {
                ImGui.TableSetupColumn("BasicCol1", ImGuiTableColumnFlags.WidthFixed, ImGui.CalcTextSize("配置名称").X);
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

                ImGuiUtil.DrawFrameColumn("优先级");
                ImGui.TableNextColumn();

                var priority = _newPriority ?? _selector.Selected!.Priority;

                ImGui.SetNextItemWidth(50);
                if (ImGui.InputInt("##Priority", ref priority, 0, 0))
                {
                    _newPriority = priority;
                    _changedProfile = _selector.Selected;
                }

                if (ImGui.IsItemDeactivatedAfterEdit() && _changedProfile != null)
                {
                    _manager.SetPriority(_changedProfile, priority);
                    _newPriority = null;
                    _changedProfile = null;
                }

                ImGuiComponents.HelpMarker("数值较高的配置文件优先于数值较低的配置文件。\n" +
                    "也就是说，如果两个或多个配置文件影响同一个角色，优先级较高的配置文件将应用于该角色。");
            }
        }
    }

    private void DrawAddCharactersArea()
    {
        using (var style = ImRaii.PushStyle(ImGuiStyleVar.ButtonTextAlign, new Vector2(0, 0.5f)))
        {
            var width = new Vector2(ImGui.GetContentRegionAvail().X - ImGui.CalcTextSize("仅限我的角色").X - 68, 0);

            ImGui.SetNextItemWidth(width.X);

            bool appliesToMultiple = _manager.DefaultProfile == _selector.Selected || _manager.DefaultLocalPlayerProfile == _selector.Selected;
            using (ImRaii.Disabled(appliesToMultiple))
            {
                _actorAssignmentUi.DrawWorldCombo(width.X / 2);
                ImGui.SameLine();
                _actorAssignmentUi.DrawPlayerInput(width.X / 2);

                var buttonWidth = new Vector2(165 * ImGuiHelpers.GlobalScale - ImGui.GetStyle().ItemSpacing.X / 2, 0);

                if (ImGuiUtil.DrawDisabledButton("应用于玩家角色", buttonWidth, string.Empty, !_actorAssignmentUi.CanSetPlayer))
                    _manager.AddCharacter(_selector.Selected!, _actorAssignmentUi.PlayerIdentifier);

                ImGui.SameLine();

                if (ImGuiUtil.DrawDisabledButton("应用于雇员", buttonWidth, string.Empty, !_actorAssignmentUi.CanSetRetainer))
                    _manager.AddCharacter(_selector.Selected!, _actorAssignmentUi.RetainerIdentifier);

                ImGui.SameLine();

                if (ImGuiUtil.DrawDisabledButton("应用于服装模特", buttonWidth, string.Empty, !_actorAssignmentUi.CanSetMannequin))
                    _manager.AddCharacter(_selector.Selected!, _actorAssignmentUi.MannequinIdentifier);

                var currentPlayer = _actorManager.GetCurrentPlayer();
                if (ImGuiUtil.DrawDisabledButton("应用于当前角色", buttonWidth, string.Empty, !currentPlayer.IsValid))
                    _manager.AddCharacter(_selector.Selected!, currentPlayer);

                ImGui.Separator();

                _actorAssignmentUi.DrawObjectKindCombo(width.X / 2);
                ImGui.SameLine();
                _actorAssignmentUi.DrawNpcInput(width.X / 2);

                if (ImGuiUtil.DrawDisabledButton("应用于选定的NPC", buttonWidth, string.Empty, !_actorAssignmentUi.CanSetNpc))
                    _manager.AddCharacter(_selector.Selected!, _actorAssignmentUi.NpcIdentifier);
            }
        }
    }

    private void DrawCharacterListArea()
    {
        var isDefaultLP = _manager.DefaultLocalPlayerProfile == _selector.Selected;
        var isDefaultLPOrCurrentProfilesEnabled = (_manager.DefaultLocalPlayerProfile?.Enabled ?? false) || (_selector.Selected?.Enabled ?? false);
        using (ImRaii.Disabled(isDefaultLPOrCurrentProfilesEnabled))
        {
            if (ImGui.Checkbox("##DefaultLocalPlayerProfile", ref isDefaultLP))
                _manager.SetDefaultLocalPlayerProfile(isDefaultLP ? _selector.Selected! : null);
            ImGuiUtil.LabeledHelpMarker("应用于您登录的任何角色",
                "是否应将此配置文件中的模板应用于您当前登录的任何角色。\r\n对该角色而言，此选项优先于下一个选项。\r\n此设置不能应用于多个配置文件。");
        }
        if (isDefaultLPOrCurrentProfilesEnabled)
        {
            ImGui.SameLine();
            ImGui.PushStyleColor(ImGuiCol.Text, Constants.Colors.Warning);
            ImGuiUtil.PrintIcon(FontAwesomeIcon.ExclamationTriangle);
            ImGui.PopStyleColor();
            ImGuiUtil.HoverTooltip("只有在当前选定的配置文件和此复选框选中的配置文件都被禁用时，才能更改。");
        }

        ImGui.SameLine();
        using(ImRaii.Disabled(true))
            ImGui.Button("##splitter", new Vector2(1, ImGui.GetFrameHeight()));
        ImGui.SameLine();

        var isDefault = _manager.DefaultProfile == _selector.Selected;
        var isDefaultOrCurrentProfilesEnabled = (_manager.DefaultProfile?.Enabled ?? false) || (_selector.Selected?.Enabled ?? false);
        using (ImRaii.Disabled(isDefaultOrCurrentProfilesEnabled))
        {
            if (ImGui.Checkbox("##DefaultProfile", ref isDefault))
                _manager.SetDefaultProfile(isDefault ? _selector.Selected! : null);
            ImGuiUtil.LabeledHelpMarker("应用于所有玩家和雇员",
                "是否应将此配置文件中的模板应用于所有没有特定配置文件的玩家和雇员。\r\n此设置不能应用于多个配置文件。");
        }
        if (isDefaultOrCurrentProfilesEnabled)
        {
            ImGui.SameLine();
            ImGui.PushStyleColor(ImGuiCol.Text, Constants.Colors.Warning);
            ImGuiUtil.PrintIcon(FontAwesomeIcon.ExclamationTriangle);
            ImGui.PopStyleColor();
            ImGuiUtil.HoverTooltip("只有在当前选定的配置文件和此复选框选中的配置文件都被禁用时，才能更改。");
        }
        bool appliesToMultiple = _manager.DefaultProfile == _selector.Selected || _manager.DefaultLocalPlayerProfile == _selector.Selected;

        ImGui.Separator();

        using var dis = ImRaii.Disabled(appliesToMultiple);
        using var table = ImRaii.Table("CharacterTable", 2, ImGuiTableFlags.RowBg | ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY, new Vector2(ImGui.GetContentRegionAvail().X, 200));
        if (!table)
            return;

        ImGui.TableSetupColumn("##charaDel", ImGuiTableColumnFlags.WidthFixed, ImGui.GetFrameHeight());
        ImGui.TableSetupColumn("角色", ImGuiTableColumnFlags.WidthFixed, 320 * ImGuiHelpers.GlobalScale);
        ImGui.TableHeadersRow();

        if (appliesToMultiple)
        {
            ImGui.TableNextColumn();
            ImGui.TableNextColumn();
            ImGui.AlignTextToFramePadding();
            ImGui.TextUnformatted("应用于多个目标");
            return;
        }

        //warn: .ToList() might be performance critical at some point
        //the copying via ToList is done because manipulations with .Templates list result in "Collection was modified" exception here
        var charas = _selector.Selected!.Characters.WithIndex().ToList();

        if (charas.Count == 0)
        {
            ImGui.TableNextColumn();
            ImGui.TableNextColumn();
            ImGui.AlignTextToFramePadding();
            ImGui.TextUnformatted("此配置文件没有关联角色");
        }

        foreach (var (character, idx) in charas)
        {
            using var id = ImRaii.PushId(idx);
            ImGui.TableNextColumn();
            var keyValid = _configuration.UISettings.DeleteTemplateModifier.IsActive();
            var tt = keyValid
                ? "从配置文件中移除此角色。"
                : $"从配置文件中移除此角色。\n按住 {_configuration.UISettings.DeleteTemplateModifier} 以移除。";

            if (ImGuiUtil.DrawDisabledButton(FontAwesomeIcon.Trash.ToIconString(), new Vector2(ImGui.GetFrameHeight()), tt, !keyValid, true))
                _endAction = () => _manager.DeleteCharacter(_selector.Selected!, character);
            ImGui.TableNextColumn();
            ImGui.AlignTextToFramePadding();
            ImGui.TextUnformatted(!_selector.IncognitoMode ? $"{character.ToNameWithoutOwnerName()}{character.TypeToString()}" : "匿名模式");

            var profiles = _manager.GetEnabledProfilesByActor(character).ToList();
            if (profiles.Count > 1)
            {
                //todo: make helper
                ImGui.SameLine();
                if (profiles.Any(x => x.IsTemporary))
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, Constants.Colors.Error);
                    ImGuiUtil.PrintIcon(FontAwesomeIcon.Lock);
                }
                else if (profiles[0] != _selector.Selected!)
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, Constants.Colors.Warning);
                    ImGuiUtil.PrintIcon(FontAwesomeIcon.ExclamationTriangle);
                }
                else
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, Constants.Colors.Info);
                    ImGuiUtil.PrintIcon(FontAwesomeIcon.Star);
                }

                ImGui.PopStyleColor();

                if (profiles.Any(x => x.IsTemporary))
                    ImGuiUtil.HoverTooltip("此角色正在受到外部插件设置的临时配置文件影响。该配置文件将不会被应用！");
                else
                    ImGuiUtil.HoverTooltip(profiles[0] != _selector.Selected! ? "多个配置文件尝试影响此角色。该配置文件将不会被应用！" :
                        "多个配置文件尝试影响此角色。该配置文件正在被应用。");
            }
        }

        _endAction?.Invoke();
        _endAction = null;
    }

    private void DrawTemplateArea()
    {
        using var table = ImRaii.Table("TemplateTable", 4, ImGuiTableFlags.RowBg | ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
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

    private void UpdateIdentifiers()
    {

    }
}
