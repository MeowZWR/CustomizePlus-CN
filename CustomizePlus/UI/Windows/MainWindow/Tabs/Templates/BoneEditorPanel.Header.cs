using CustomizePlus.Armatures.Data;
using CustomizePlus.Configuration.Data;
using CustomizePlus.Core.Data;
using CustomizePlus.Core.Helpers;
using CustomizePlus.Game.Services;
using CustomizePlus.GameData.Extensions;
using CustomizePlus.Templates;
using CustomizePlus.Templates.Data;
using CustomizePlus.UI.Windows.Controls;
using Dalamud.Interface;
using Dalamud.Interface.Utility;

namespace CustomizePlus.UI.Windows.MainWindow.Tabs.Templates;

public partial class BoneEditorPanel
{
    private bool DrawEditorHeader()
    {
        string characterText = null!;

        if (_configuration.UISettings.IncognitoMode)
            characterText = "预览角色：匿名模式激活";
        else
            characterText = _editorManager.Character.IsValid ? $"预览角色：{(_editorManager.Character.Type == Penumbra.GameData.Enums.IdentifierType.Owned ?
            _editorManager.Character.ToNameWithoutOwnerName() : _editorManager.Character.ToString())}" : "未选择有效角色";

        UiHelpers.DrawIcon(FontAwesomeIcon.User);
        Im.Line.Same();
        Im.Text(characterText);

        Im.Separator();

        var isShouldDraw = Im.Tree.Header("更改预览角色"u8);

        if (isShouldDraw)
        {
            var width = new Vector2(Im.ContentRegion.Available.X - Im.Font.CalculateSize("限制为我的角色"u8).X - 68, 0);

            using (var disabled = Im.Disabled(!IsEditorActive || IsEditorPaused))
            {
                if (!_configuration.UISettings.IncognitoMode)
                {
                    _actorAssignmentUi.DrawWorldCombo(width.X / 2);
                    Im.Line.Same();
                    _actorAssignmentUi.DrawPlayerInput(width.X / 2);

                    var buttonWidth = new Vector2(165 * ImGuiHelpers.GlobalScale - Im.Style.ItemSpacing.X / 2, 0);

                    if (UiHelpers.DrawDisabledButton("应用于玩家角色", buttonWidth, string.Empty, !_actorAssignmentUi.CanSetPlayer))
                        _editorManager.ChangeEditorCharacter(_actorAssignmentUi.PlayerIdentifier);

                    Im.Line.Same();

                    if (UiHelpers.DrawDisabledButton("应用于雇员", buttonWidth, string.Empty, !_actorAssignmentUi.CanSetRetainer))
                        _editorManager.ChangeEditorCharacter(_actorAssignmentUi.RetainerIdentifier);

                    Im.Line.Same();

                    if (UiHelpers.DrawDisabledButton("应用于服装模特", buttonWidth, string.Empty, !_actorAssignmentUi.CanSetMannequin))
                        _editorManager.ChangeEditorCharacter(_actorAssignmentUi.MannequinIdentifier);

                    var currentPlayer = _gameObjectService.GetCurrentPlayerActorIdentifier().CreatePermanent();
                    if (UiHelpers.DrawDisabledButton("应用于当前角色", buttonWidth, string.Empty, !currentPlayer.IsValid))
                        _editorManager.ChangeEditorCharacter(currentPlayer);

                    Im.Separator();

                    _actorAssignmentUi.DrawObjectKindCombo(width.X / 2);
                    Im.Line.Same();
                    _actorAssignmentUi.DrawNpcInput(width.X / 2);

                    if (UiHelpers.DrawDisabledButton("应用于选定的NPC", buttonWidth, string.Empty, !_actorAssignmentUi.CanSetNpc))
                        _editorManager.ChangeEditorCharacter(_actorAssignmentUi.NpcIdentifier);
                }
                else
                    Im.Text("匿名模式已激活"u8);
            }
        }

        Im.Separator();

        using (var table = Im.Table.Begin("BoneEditorMenu"u8, 2))
        {
            if (!table)
                return false;

            table.SetupColumn("属性"u8, TableColumnFlags.WidthFixed);
            table.SetupColumn("空间"u8, TableColumnFlags.WidthStretch);

            Im.Table.NextRow();
            Im.Table.NextColumn();

            var modeChanged = false;
            if (Im.RadioButton("位置"u8, _editingAttribute == BoneAttribute.Position))
            {
                _editingAttribute = BoneAttribute.Position;
                modeChanged = true;
            }
            CtrlHelper.AddHoverText($"可能会产生意想不到的影响。编辑后风险自负！");

            Im.Line.Same();
            if (Im.RadioButton("旋转"u8, _editingAttribute == BoneAttribute.Rotation))
            {
                _editingAttribute = BoneAttribute.Rotation;
                modeChanged = true;
            }
            CtrlHelper.AddHoverText($"可能会产生意想不到的影响。编辑后风险自负！");

            Im.Line.Same();
            if (Im.RadioButton("缩放"u8, _editingAttribute == BoneAttribute.Scale))
            {
                _editingAttribute = BoneAttribute.Scale;
                modeChanged = true;
            }

            Im.Line.Same();
            Im.Item.SetNextWidth(200 * ImGuiHelpers.GlobalScale);
            Im.Input.Text("##BoneSearch"u8, ref _boneSearch, "搜索骨骼..."u8, maxLength: 64);

            Im.Line.Same();
            if (DrawIconButton("UndoBone", FontAwesomeIcon.Undo, "撤消", !_editorManager.CanUndo))
                _editorManager.Undo();

            Im.Line.Same();
            if (DrawIconButton("RedoBone", FontAwesomeIcon.Redo, "重做", !_editorManager.CanRedo))
                _editorManager.Redo();

            if (modeChanged)
            {
                _configuration.EditorConfiguration.EditorMode = _editingAttribute;
                _configuration.Save();
            }

            using (var disabled = Im.Disabled(!_isUnlocked))
            {
                Im.Line.Same();
                if (CtrlHelper.Checkbox("显示活动骨骼", ref _isShowLiveBones))
                {
                    _configuration.EditorConfiguration.ShowLiveBones = _isShowLiveBones;
                    _configuration.Save();
                }
                CtrlHelper.AddHoverText($"如果选中，则显示在游戏数据中找到的可编辑的所有骨骼，\n否则仅显示已编辑过的骨骼。");

                Im.Line.Same();
                using (var disabledMirrorMode = Im.Disabled(!_isShowLiveBones))
                {
                    if (CtrlHelper.Checkbox("镜像模式", ref _isMirrorModeEnabled))
                    {
                        _configuration.EditorConfiguration.BoneMirroringEnabled = _isMirrorModeEnabled;
                        _configuration.Save();
                    }
                    CtrlHelper.AddHoverText($"具有对应关系的骨骼将同时被修改。");
                }
            }

            Im.Table.NextColumn();

            if (Im.Slider("##Precision"u8, ref _precision, $"{_precision} 位", 0, 6))
            {
                _configuration.EditorConfiguration.EditorValuesPrecision = _precision;
                _configuration.Save();
            }
            CtrlHelper.AddHoverText("编辑时显示的小数点后的位数");
        }

        return true;
    }
}
