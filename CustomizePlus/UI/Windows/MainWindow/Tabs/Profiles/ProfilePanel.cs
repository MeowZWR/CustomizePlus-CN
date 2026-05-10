using CustomizePlus.Configuration.Data;
using CustomizePlus.Core.Data;
using CustomizePlus.Core.Helpers;
using CustomizePlus.GameData.Extensions;
using CustomizePlus.Profiles;
using CustomizePlus.Profiles.Data;
using CustomizePlus.Templates;
using CustomizePlus.Templates.Events;
using CustomizePlus.UI.Windows.Controls;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Penumbra.GameData.Actors;

namespace CustomizePlus.UI.Windows.MainWindow.Tabs.Profiles;

public class ProfilePanel : IPanel
{
    private readonly ProfileFileSystem _fileSystem;
    private readonly ProfileManager _manager;
    private readonly PluginConfiguration _configuration;
    private readonly TemplateCombo _templateCombo;
    private readonly TemplateEditorManager _templateEditorManager;
    private readonly ActorAssignmentUi _actorAssignmentUi;
    private readonly ActorManager _actorManager;
    private readonly TemplateEditorEvent _templateEditorEvent;
    private readonly PopupSystem _popupSystem;
    private readonly Logger _logger;
    private readonly MultiProfilePanel _multiProfilePanel;

    private string? _newName;
    private int? _newPriority;
    private Profile? _changedProfile;

    private Action? _endAction;

    private int _dragIndex = -1;

    public ReadOnlySpan<byte> Id
        => "ProfilePanel"u8;

    public bool Collapsed
        => false;

    public ProfilePanel(
        ProfileFileSystem fileSystem,
        ProfileManager manager,
        PluginConfiguration configuration,
        TemplateCombo templateCombo,
        TemplateEditorManager templateEditorManager,
        ActorAssignmentUi actorAssignmentUi,
        ActorManager actorManager,
        TemplateEditorEvent templateEditorEvent,
        PopupSystem popupSystem,
        Logger logger,
        MultiProfilePanel multiProfilePanel)
    {
        _fileSystem = fileSystem;
        _manager = manager;
        _configuration = configuration;
        _templateCombo = templateCombo;
        _templateEditorManager = templateEditorManager;
        _actorAssignmentUi = actorAssignmentUi;
        _actorManager = actorManager;
        _templateEditorEvent = templateEditorEvent;
        _popupSystem = popupSystem;
        _logger = logger;
        _multiProfilePanel = multiProfilePanel;
    }

    private Profile Selection
        => (Profile)_fileSystem.Selection.Selection!.Value;

    public void Draw()
    {
        if (_fileSystem.Selection.OrderedNodes.Count > 1)
        {
            _multiProfilePanel.Draw();
            return;
        }

        DrawPanel();
    }

    private void DrawPanel()
    {
        if (_fileSystem.Selection.Selection is null)
            return;

        DrawBasicSettings();

        Im.Separator();

        using (var disabled = Im.Disabled(Selection.IsWriteProtected))
        {
            var isShouldDraw = Im.Tree.Header("添加角色"u8);

            if (isShouldDraw)
                DrawAddCharactersArea();

            Im.Separator();

            DrawCharacterListArea();

            Im.Separator();

            DrawTemplateArea();
        }
    }

    private void DrawBasicSettings()
    {
        using (var table = Im.Table.Begin("BasicSettings"u8, 2))
        {
            if (!table)
                return;

            table.SetupColumn("Label"u8, TableColumnFlags.WidthFixed, 110 * ImGuiHelpers.GlobalScale);
            table.SetupColumn("Control"u8, TableColumnFlags.WidthStretch);

            table.NextRow();
            UiHelpers.DrawPropertyLabel("启用");
            table.NextColumn();
            DrawEnabledControl();

            table.NextRow();
            UiHelpers.DrawPropertyLabel("配置名称");
            table.NextColumn();
            DrawProfileNameControl();

            table.NextRow();
            UiHelpers.DrawPropertyLabel("优先级");
            table.NextColumn();
            DrawPriorityControl();
        }
    }

    private void DrawEnabledControl()
    {
        var enabled = Selection.Enabled;
        using (Im.Disabled(_templateEditorManager.IsEditorActive || _templateEditorManager.IsEditorPaused))
        {
            if (Im.Checkbox("##Enabled"u8, ref enabled))
                _manager.SetEnabled(Selection, enabled);
        }

        Im.Line.Same();
        LunaStyle.DrawAlignedHelpMarker("是否需要完全应用此配置文件中的模板。"u8);
    }

    private void DrawProfileNameControl()
    {
        using var disabled = Im.Disabled(Selection.IsWriteProtected);
        var name = _newName ?? Selection.Name;
        Im.Item.SetNextWidthFull();

        if (!_configuration.UISettings.IncognitoMode)
        {
            if (Im.Input.Text("##ProfileName"u8, ref name, maxLength: 128))
            {
                _newName = name;
                _changedProfile = Selection;
            }

            if (Im.Item.DeactivatedAfterEdit && _changedProfile != null)
            {
                _manager.Rename(_changedProfile, name);
                _newName = null;
                _changedProfile = null;
            }
        }
        else
        {
            Im.Cursor.FrameAlign();
            Im.Text(Selection.Incognito);
        }
    }

    private void DrawPriorityControl()
    {
        using var disabled = Im.Disabled(Selection.IsWriteProtected);
        var priority = _newPriority ?? Selection.Priority;

        Im.Item.SetNextWidth(90 * ImGuiHelpers.GlobalScale);
        if (Im.Input.Scalar("##Priority"u8, ref priority, "%d"u8, 0, 0))
        {
            _newPriority = priority;
            _changedProfile = Selection;
        }

        if (Im.Item.DeactivatedAfterEdit && _changedProfile != null)
        {
            _manager.SetPriority(_changedProfile, priority);
            _newPriority = null;
            _changedProfile = null;
        }

        Im.Line.Same();
        LunaStyle.DrawAlignedHelpMarker(
            "数值较高的配置文件优先于数值较低的配置文件。\n若多个配置文件影响同一角色，将应用优先级较高的配置文件。"u8);
    }

    private void ExportToClipboard()
    {
        try
        {
            Im.Clipboard.Set(Base64Helper.ExportProfileToBase64(Selection));
            _popupSystem.ShowPopup(PopupSystem.Messages.ClipboardDataNotLongTerm);
        }
        catch (Exception ex)
        {
            _logger.Error($"Could not copy data from profile {Selection.UniqueId} to clipboard: {ex}");
            _popupSystem.ShowPopup(PopupSystem.Messages.ActionError);
        }
    }

    private void DrawAddCharactersArea()
    {
        using (var style = Im.Style.Push(ImStyleDouble.ButtonTextAlign, new Vector2(0, 0.5f)))
        {
            var width = new Vector2(Im.ContentRegion.Available.X - Im.Font.CalculateSize("仅限我的从属"u8).X - 68, 0);

            Im.Item.SetNextWidth(width.X);

            bool appliesToMultiple = _manager.DefaultProfile == Selection || _manager.DefaultLocalPlayerProfile == Selection;
            using (Im.Disabled(appliesToMultiple))
            {
                _actorAssignmentUi.DrawWorldCombo(width.X / 2);
                Im.Line.Same();
                _actorAssignmentUi.DrawPlayerInput(width.X / 2);

                var buttonWidth = new Vector2(165 * ImGuiHelpers.GlobalScale - Im.Style.ItemSpacing.X / 2, 0);

                if (UiHelpers.DrawDisabledButton("应用于玩家角色", buttonWidth, string.Empty, !_actorAssignmentUi.CanSetPlayer))
                    _manager.AddCharacter(Selection, _actorAssignmentUi.PlayerIdentifier);

                Im.Line.Same();

                if (UiHelpers.DrawDisabledButton("应用于雇员", buttonWidth, string.Empty, !_actorAssignmentUi.CanSetRetainer))
                    _manager.AddCharacter(Selection, _actorAssignmentUi.RetainerIdentifier);

                Im.Line.Same();

                if (UiHelpers.DrawDisabledButton("应用于服装模特", buttonWidth, string.Empty, !_actorAssignmentUi.CanSetMannequin))
                    _manager.AddCharacter(Selection, _actorAssignmentUi.MannequinIdentifier);

                var currentPlayer = _actorManager.GetCurrentPlayer().CreatePermanent();
                if (UiHelpers.DrawDisabledButton("应用于当前角色", buttonWidth, string.Empty, !currentPlayer.IsValid))
                    _manager.AddCharacter(Selection, currentPlayer);

                Im.Separator();

                _actorAssignmentUi.DrawObjectKindCombo(width.X / 2);
                Im.Line.Same();
                _actorAssignmentUi.DrawNpcInput(width.X / 2);

                if (UiHelpers.DrawDisabledButton("应用于选定的 NPC", buttonWidth, string.Empty, !_actorAssignmentUi.CanSetNpc))
                    _manager.AddCharacter(Selection, _actorAssignmentUi.NpcIdentifier);
            }
        }
    }

    private void DrawCharacterListArea()
    {
        var isDefaultLP = _manager.DefaultLocalPlayerProfile == Selection;
        var isDefaultLPOrCurrentProfilesEnabled = (_manager.DefaultLocalPlayerProfile?.Enabled ?? false) || Selection.Enabled;
        using (Im.Disabled(isDefaultLPOrCurrentProfilesEnabled))
        {
            if (Im.Checkbox("##DefaultLocalPlayerProfile"u8, ref isDefaultLP))
                _manager.SetDefaultLocalPlayerProfile(isDefaultLP ? Selection : null);
            LunaStyle.DrawAlignedHelpMarkerLabel("应用于您登录的任何角色"u8,
                "是否将此配置文件中的模板应用于您当前登录的任意角色。\r\n对该角色而言，此选项优先于下一项。\r\n此设置不能同时应用于多个配置文件。"u8);
        }
        if (isDefaultLPOrCurrentProfilesEnabled)
        {
            Im.Line.Same();
            using var warning = ImGuiColor.Text.Push(Constants.Colors.Warning);
            UiHelpers.DrawIcon(FontAwesomeIcon.ExclamationTriangle);
            UiHelpers.DrawHoverTooltip("仅当当前选中的配置文件与此项所在的配置文件均已禁用时才能更改。");
        }

        Im.Line.Same();
        using (Im.Disabled(true))
            Im.Button("##splitter"u8, new Vector2(1, Im.Style.FrameHeight));
        Im.Line.Same();

        var isDefault = _manager.DefaultProfile == Selection;
        var isDefaultOrCurrentProfilesEnabled = (_manager.DefaultProfile?.Enabled ?? false) || Selection.Enabled;
        using (Im.Disabled(isDefaultOrCurrentProfilesEnabled))
        {
            if (Im.Checkbox("##DefaultProfile"u8, ref isDefault))
                _manager.SetDefaultProfile(isDefault ? Selection : null);
            LunaStyle.DrawAlignedHelpMarkerLabel("应用于所有玩家和雇员"u8,
                "是否将此配置文件中的模板应用于所有没有单独配置的玩家与雇员。\r\n此设置不能同时应用于多个配置文件。"u8);
        }
        if (isDefaultOrCurrentProfilesEnabled)
        {
            Im.Line.Same();
            using var warning = ImGuiColor.Text.Push(Constants.Colors.Warning);
            UiHelpers.DrawIcon(FontAwesomeIcon.ExclamationTriangle);
            UiHelpers.DrawHoverTooltip("仅当当前选中的配置文件与此项所在的配置文件均已禁用时才能更改。");
        }
        bool appliesToMultiple = _manager.DefaultProfile == Selection || _manager.DefaultLocalPlayerProfile == Selection;

        Im.Separator();

        using var dis = Im.Disabled(appliesToMultiple);
        using var table = Im.Table.Begin("CharacterTable"u8, 2, TableFlags.RowBackground | TableFlags.ScrollX | TableFlags.ScrollY, new Vector2(Im.ContentRegion.Available.X, 200));
        if (!table)
            return;

        table.SetupColumn("##charaDel"u8, TableColumnFlags.WidthFixed, Im.Style.FrameHeight);
        table.SetupColumn("角色"u8, TableColumnFlags.WidthFixed, 320 * ImGuiHelpers.GlobalScale);
        table.HeaderRow();

        if (appliesToMultiple)
        {
            table.NextColumn();
            table.NextColumn();
            Im.Cursor.FrameAlign();
            Im.Text("应用于多个目标"u8);
            return;
        }

        //warn: .ToList() might be performance critical at some point
        //the copying via ToList is done because manipulations with .Templates list result in "Collection was modified" exception here
        var charas = Selection.Characters.Select((character, idx) => (character, idx)).ToList();

        if (charas.Count == 0)
        {
            table.NextColumn();
            table.NextColumn();
            Im.Cursor.FrameAlign();
            Im.Text("此配置文件未关联任何角色"u8);
        }

        foreach (var (character, idx) in charas)
        {
            using var id = Im.Id.Push(idx);
            table.NextColumn();
            var keyValid = _configuration.UISettings.DeleteModifier.IsActive();
            var tt = keyValid
                ? "从配置文件中移除此角色。"
                : $"从配置文件中移除此角色。\n按住 {_configuration.UISettings.DeleteModifier} 以移除。";

            if (UiHelpers.DrawIconButton(FontAwesomeIcon.Trash, new Vector2(Im.Style.FrameHeight), tt, !keyValid))
                _endAction = () => _manager.DeleteCharacter(Selection, character);
            table.NextColumn();
            Im.Cursor.FrameAlign();
            Im.Text(!_configuration.UISettings.IncognitoMode ? $"{character.ToNameWithoutOwnerName()}{character.TypeToString()}" : "匿名模式");

            var profiles = _manager.GetEnabledProfilesByActor(character).ToList();
            if (profiles.Count > 1)
            {
                //todo: make helper
                Im.Line.Same();
                if (profiles.Any(x => x.IsTemporary))
                {
                    using var color = ImGuiColor.Text.Push(Constants.Colors.Error);
                    UiHelpers.DrawIcon(FontAwesomeIcon.Lock);
                }
                else if (profiles[0] != Selection)
                {
                    using var color = ImGuiColor.Text.Push(Constants.Colors.Warning);
                    UiHelpers.DrawIcon(FontAwesomeIcon.ExclamationTriangle);
                }
                else
                {
                    using var color = ImGuiColor.Text.Push(Constants.Colors.Info);
                    UiHelpers.DrawIcon(FontAwesomeIcon.Star);
                }

                if (profiles.Any(x => x.IsTemporary))
                    UiHelpers.DrawHoverTooltip("该角色正受外部插件设置的临时配置文件影响；此配置文件不会被应用！");
                else
                    UiHelpers.DrawHoverTooltip(profiles[0] != Selection ? "多个配置文件尝试影响此角色；此配置文件不会被应用！" :
                        "多个配置文件尝试影响此角色；当前正在应用此配置文件。");
            }
        }

        _endAction?.Invoke();
        _endAction = null;
    }

    private void DrawTemplateArea()
    {
        using var table = Im.Table.Begin("TemplateTable"u8, 5, TableFlags.RowBackground | TableFlags.ScrollX | TableFlags.ScrollY);
        if (!table)
            return;

        table.SetupColumn("##del"u8, TableColumnFlags.WidthFixed, Im.Style.FrameHeight);
        table.SetupColumn("##Index"u8, TableColumnFlags.WidthFixed, 30 * ImGuiHelpers.GlobalScale);
        table.SetupColumn("##Enabled"u8, TableColumnFlags.WidthFixed, 30 * ImGuiHelpers.GlobalScale);

        table.SetupColumn("模板"u8, TableColumnFlags.WidthFixed, 420 * ImGuiHelpers.GlobalScale);

        table.SetupColumn("##editbtn"u8, TableColumnFlags.WidthFixed, 120 * ImGuiHelpers.GlobalScale);

        table.HeaderRow();

        //warn: .ToList() might be performance critical at some point
        //the copying via ToList is done because manipulations with .Templates list result in "Collection was modified" exception here
        foreach (var (template, idx) in Selection.Templates.Select((template, idx) => (template, idx)).ToList())
        {
            using var id = Im.Id.Push(idx);
            table.NextColumn();
            var keyValid = _configuration.UISettings.DeleteModifier.IsActive();
            var tt = keyValid
                ? "从配置文件中删除此模板。"
                : $"从配置文件中删除此模板。\n按住 {_configuration.UISettings.DeleteModifier} 以删除。";

            if (UiHelpers.DrawIconButton(FontAwesomeIcon.Trash, new Vector2(Im.Style.FrameHeight), tt, !keyValid))
                _endAction = () => _manager.DeleteTemplate(Selection, idx);
            table.NextColumn();
            Im.Selectable($"#{idx + 1:D2}");
            DrawDragDrop(Selection, idx);

            table.NextColumn();
            var enabled = !Selection.DisabledTemplates.Contains(template.UniqueId);
            if (Im.Checkbox("##EnableCheckbox"u8, ref enabled))
                _manager.ToggleTemplate(Selection, idx);
            UiHelpers.DrawHoverTooltip("是否将此模板应用于该配置文件。");

            table.NextColumn();

            _templateCombo.Draw(Selection, template, idx);

            DrawDragDrop(Selection, idx);

            table.NextColumn();

            var disabledCondition = _templateEditorManager.IsEditorActive || template.IsWriteProtected;

            if (UiHelpers.DrawIconButton(FontAwesomeIcon.Edit, new Vector2(Im.Style.FrameHeight), "在模板编辑器中打开此模板。", disabledCondition))
                _templateEditorEvent.Invoke(new TemplateEditorEvent.Arguments(TemplateEditorEvent.Type.EditorEnableRequested, template));

            if (disabledCondition)
            {
                //todo: make helper
                Im.Line.Same();
                using var warning = ImGuiColor.Text.Push(Constants.Colors.Warning);
                UiHelpers.DrawIcon(FontAwesomeIcon.ExclamationTriangle);
                UiHelpers.DrawHoverTooltip("无法编辑此模板：模板已写保护，或您正在编辑其他模板。");
            }
        }

        table.NextColumn();
        table.NextColumn();
        table.NextColumn();
        Im.Cursor.FrameAlign();
        Im.Text("新增"u8);
        table.NextColumn();
        _templateCombo.Draw(Selection, null, -1);
        table.NextRow();

        _endAction?.Invoke();
        _endAction = null;
    }

    private void DrawDragDrop(Profile profile, int index)
    {
        const string dragDropLabel = "TemplateDragDrop";
        using (var target = Im.DragDrop.Target())
        {
            if (target.Success && UiHelpers.IsDragDropPayload(dragDropLabel))
            {
                if (_dragIndex >= 0)
                {
                    var idx = _dragIndex;
                    _endAction = () => _manager.MoveTemplate(profile, idx, index);
                }

                _dragIndex = -1;
            }
        }

        using (var source = Im.DragDrop.Source())
        {
            if (source)
            {
                Im.Text($"移动模板 #{index + 1:D2}...");
                if (source.SetPayload("TemplateDragDrop"u8))
                {
                    _dragIndex = index;
                }
            }
        }
    }
}
