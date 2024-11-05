using Dalamud.Interface.Components;
using Dalamud.Interface;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Interface.Utility;
using OtterGui;
using OtterGui.Raii;
using CustomizePlus.Core.Data;
using CustomizePlus.Armatures.Data;
using CustomizePlus.Configuration.Data;
using CustomizePlus.Core.Helpers;
using CustomizePlus.Templates;
using CustomizePlus.Game.Services;
using CustomizePlus.Templates.Data;
using CustomizePlus.UI.Windows.Controls;
using FFXIVClientStructs.FFXIV.Client.Graphics.Render;
using Penumbra.GameData.Actors;
using CustomizePlus.GameData.Extensions;

namespace CustomizePlus.UI.Windows.MainWindow.Tabs.Templates;

public class BoneEditorPanel
{
    private readonly TemplateFileSystemSelector _templateFileSystemSelector;
    private readonly TemplateEditorManager _editorManager;
    private readonly PluginConfiguration _configuration;
    private readonly GameObjectService _gameObjectService;
    private readonly ActorAssignmentUi _actorAssignmentUi;

    private BoneAttribute _editingAttribute;
    private int _precision;

    private bool _isShowLiveBones;
    private bool _isMirrorModeEnabled;

    private Dictionary<BoneData.BoneFamily, bool> _groupExpandedState = new();

    private bool _openSavePopup;

    private bool _isUnlocked = false;

    public bool HasChanges => _editorManager.HasChanges;
    public bool IsEditorActive => _editorManager.IsEditorActive;
    public bool IsEditorPaused => _editorManager.IsEditorPaused;
    public bool IsCharacterFound => _editorManager.IsCharacterFound;

    public BoneEditorPanel(
        TemplateFileSystemSelector templateFileSystemSelector,
        TemplateEditorManager editorManager,
        PluginConfiguration configuration,
        GameObjectService gameObjectService,
        ActorAssignmentUi actorAssignmentUi)
    {
        _templateFileSystemSelector = templateFileSystemSelector;
        _editorManager = editorManager;
        _configuration = configuration;
        _gameObjectService = gameObjectService;
        _actorAssignmentUi = actorAssignmentUi;

        _isShowLiveBones = configuration.EditorConfiguration.ShowLiveBones;
        _isMirrorModeEnabled = configuration.EditorConfiguration.BoneMirroringEnabled;
        _precision = configuration.EditorConfiguration.EditorValuesPrecision;
        _editingAttribute = configuration.EditorConfiguration.EditorMode;
    }

    public bool EnableEditor(Template template)
    {
        if (_editorManager.EnableEditor(template))
        {
            //_editorManager.SetLimitLookupToOwned(_configuration.EditorConfiguration.LimitLookupToOwnedObjects);

            return true;
        }

        return false;
    }

    public bool DisableEditor()
    {
        if (!_editorManager.HasChanges)
            return _editorManager.DisableEditor();

        if (_editorManager.HasChanges && !IsEditorActive)
            throw new Exception("骨骼编辑面板处于无效状态：已更改，但编辑器未处于活动状态。");

        _openSavePopup = true;

        return false;
    }

    public void Draw()
    {
        _isUnlocked = IsCharacterFound && IsEditorActive && !IsEditorPaused;

        DrawEditorConfirmationPopup();

        ImGui.Separator();

        using (var style = ImRaii.PushStyle(ImGuiStyleVar.ButtonTextAlign, new Vector2(0, 0.5f)))
        {
            string characterText = null!;

            if (_templateFileSystemSelector.IncognitoMode)
                characterText = "预览角色：匿名模式激活";
            else
                characterText = _editorManager.Character.IsValid ? $"预览角色：{(_editorManager.Character.Type == Penumbra.GameData.Enums.IdentifierType.Owned ?
                _editorManager.Character.ToNameWithoutOwnerName() : _editorManager.Character.ToString())}" : "未选择有效角色";

            ImGuiUtil.PrintIcon(FontAwesomeIcon.User);
            ImGui.SameLine();
            ImGui.Text(characterText);

            ImGui.Separator();

            var isShouldDraw = ImGui.CollapsingHeader("更改预览角色");

            if (isShouldDraw)
            {
                var width = new Vector2(ImGui.GetContentRegionAvail().X - ImGui.CalcTextSize("限制为我的角色").X - 68, 0);

                using (var disabled = ImRaii.Disabled(!IsEditorActive || IsEditorPaused))
                {
                    if (!_templateFileSystemSelector.IncognitoMode)
                    {
                        _actorAssignmentUi.DrawWorldCombo(width.X / 2);
                        ImGui.SameLine();
                        _actorAssignmentUi.DrawPlayerInput(width.X / 2);

                        var buttonWidth = new Vector2(165 * ImGuiHelpers.GlobalScale - ImGui.GetStyle().ItemSpacing.X / 2, 0);

                        if (ImGuiUtil.DrawDisabledButton("应用于玩家角色", buttonWidth, string.Empty, !_actorAssignmentUi.CanSetPlayer))
                            _editorManager.ChangeEditorCharacter(_actorAssignmentUi.PlayerIdentifier);

                        ImGui.SameLine();

                        if (ImGuiUtil.DrawDisabledButton("应用于雇员", buttonWidth, string.Empty, !_actorAssignmentUi.CanSetRetainer))
                            _editorManager.ChangeEditorCharacter(_actorAssignmentUi.RetainerIdentifier);

                        ImGui.SameLine();

                        if (ImGuiUtil.DrawDisabledButton("应用于服装模特", buttonWidth, string.Empty, !_actorAssignmentUi.CanSetMannequin))
                            _editorManager.ChangeEditorCharacter(_actorAssignmentUi.MannequinIdentifier);

                        var currentPlayer = _gameObjectService.GetCurrentPlayerActorIdentifier();
                        if (ImGuiUtil.DrawDisabledButton("应用于当前角色", buttonWidth, string.Empty, !currentPlayer.IsValid))
                            _editorManager.ChangeEditorCharacter(currentPlayer);

                        ImGui.Separator();

                        _actorAssignmentUi.DrawObjectKindCombo(width.X / 2);
                        ImGui.SameLine();
                        _actorAssignmentUi.DrawNpcInput(width.X / 2);

                        if (ImGuiUtil.DrawDisabledButton("应用于选定的NPC", buttonWidth, string.Empty, !_actorAssignmentUi.CanSetNpc))
                            _editorManager.ChangeEditorCharacter(_actorAssignmentUi.NpcIdentifier);
                    }
                    else
                        ImGui.TextUnformatted("匿名模式已激活");
                }
            }

            ImGui.Separator();

            using (var table = ImRaii.Table("BoneEditorMenu", 2))
            {
                ImGui.TableSetupColumn("属性", ImGuiTableColumnFlags.WidthFixed);
                ImGui.TableSetupColumn("空间", ImGuiTableColumnFlags.WidthStretch);

                ImGui.TableNextRow();
                ImGui.TableNextColumn();

                var modeChanged = false;
                if (ImGui.RadioButton("位置", _editingAttribute == BoneAttribute.Position))
                {
                    _editingAttribute = BoneAttribute.Position;
                    modeChanged = true;
                }
                CtrlHelper.AddHoverText($"可能会产生意想不到的影响。编辑后风险自负！");

                ImGui.SameLine();
                if (ImGui.RadioButton("旋转", _editingAttribute == BoneAttribute.Rotation))
                {
                    _editingAttribute = BoneAttribute.Rotation;
                    modeChanged = true;
                }
                CtrlHelper.AddHoverText($"可能会产生意想不到的影响。编辑后风险自负！");

                ImGui.SameLine();
                if (ImGui.RadioButton("缩放", _editingAttribute == BoneAttribute.Scale))
                {
                    _editingAttribute = BoneAttribute.Scale;
                    modeChanged = true;
                }

                if (modeChanged)
                {
                    _configuration.EditorConfiguration.EditorMode = _editingAttribute;
                    _configuration.Save();
                }

                using (var disabled = ImRaii.Disabled(!_isUnlocked))
                {
                    ImGui.SameLine();
                    if (CtrlHelper.Checkbox("显示活动骨骼", ref _isShowLiveBones))
                    {
                        _configuration.EditorConfiguration.ShowLiveBones = _isShowLiveBones;
                        _configuration.Save();
                    }
                    CtrlHelper.AddHoverText($"如果选中，则显示在游戏数据中找到的可编辑的所有骨骼，\n否则仅显示已编辑过的骨骼。");

                    ImGui.SameLine();
                    ImGui.BeginDisabled(!_isShowLiveBones);
                    if (CtrlHelper.Checkbox("镜像模式", ref _isMirrorModeEnabled))
                    {
                        _configuration.EditorConfiguration.BoneMirroringEnabled = _isMirrorModeEnabled;
                        _configuration.Save();
                    }
                    CtrlHelper.AddHoverText($"具有对应关系的骨骼将同时被修改。");
                    ImGui.EndDisabled();
                }

                ImGui.TableNextColumn();

                if (ImGui.SliderInt("##Precision", ref _precision, 0, 6, $"{_precision} 位{(_precision == 1 ? "" : " ")}"))
                {
                    _configuration.EditorConfiguration.EditorValuesPrecision = _precision;
                    _configuration.Save();
                }
                CtrlHelper.AddHoverText("编辑时显示的小数点后的位数");
            }

            ImGui.Separator();

            using (var table = ImRaii.Table("BoneEditorContents", 6, ImGuiTableFlags.BordersOuterH | ImGuiTableFlags.BordersV | ImGuiTableFlags.ScrollY))
            {
                if (!table)
                    return;

                var col1Label = _editingAttribute == BoneAttribute.Rotation ? "翻滚旋转" : "X轴";
                var col2Label = _editingAttribute == BoneAttribute.Rotation ? "上下旋转" : "Y轴";
                var col3Label = _editingAttribute == BoneAttribute.Rotation ? "左右旋转" : "Z轴";
                var col4Label = _editingAttribute == BoneAttribute.Scale ? "全部" : "N/A";

                ImGui.TableSetupColumn("骨骼", ImGuiTableColumnFlags.NoReorder | ImGuiTableColumnFlags.WidthFixed, 3 * CtrlHelper.IconButtonWidth);

                ImGui.TableSetupColumn($"{col1Label}", ImGuiTableColumnFlags.NoReorder | ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn($"{col2Label}", ImGuiTableColumnFlags.NoReorder | ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn($"{col3Label}", ImGuiTableColumnFlags.NoReorder | ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn($"{col4Label}", ImGuiTableColumnFlags.NoReorder | ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetColumnEnabled(4, _editingAttribute == BoneAttribute.Scale);

                ImGui.TableSetupColumn("名称", ImGuiTableColumnFlags.NoReorder | ImGuiTableColumnFlags.WidthStretch);

                ImGui.TableHeadersRow();

                IEnumerable<EditRowParams> relevantModelBones = null!;
                if (_editorManager.IsEditorActive && _editorManager.EditorProfile != null && _editorManager.EditorProfile.Armatures.Count > 0)
                    relevantModelBones = _isShowLiveBones && _editorManager.EditorProfile.Armatures.Count > 0
                        ? _editorManager.EditorProfile.Armatures[0].GetAllBones().DistinctBy(x => x.BoneName).Select(x => new EditRowParams(x))
                        : _editorManager.EditorProfile.Armatures[0].BoneTemplateBinding.Where(x => x.Value.Bones.ContainsKey(x.Key))
                            .Select(x => new EditRowParams(x.Key, x.Value.Bones[x.Key])); //todo: this is awful
                else
                    relevantModelBones = _templateFileSystemSelector.Selected!.Bones.Select(x => new EditRowParams(x.Key, x.Value));

                var groupedBones = relevantModelBones.GroupBy(x => BoneData.GetBoneFamily(x.BoneCodeName));

                foreach (var boneGroup in groupedBones.OrderBy(x => (int)x.Key))
                {
                    //Hide root bone if it's not enabled in settings or if we are in rotation mode
                    if (boneGroup.Key == BoneData.BoneFamily.根骨骼 &&
                        (!_configuration.EditorConfiguration.RootPositionEditingEnabled ||
                            _editingAttribute == BoneAttribute.Rotation))
                        continue;

                    //create a dropdown entry for the family if one doesn't already exist
                    //mind that it'll only be rendered if bones exist to fill it
                    if (!_groupExpandedState.TryGetValue(boneGroup.Key, out var expanded))
                    {
                        _groupExpandedState[boneGroup.Key] = false;
                        expanded = false;
                    }

                    if (expanded)
                    {
                        //paint the row in header colors if it's expanded
                        ImGui.TableNextRow(ImGuiTableRowFlags.Headers);
                    }
                    else
                    {
                        ImGui.TableNextRow();
                    }

                    using var id = ImRaii.PushId(boneGroup.Key.ToString());
                    ImGui.TableNextColumn();

                    CtrlHelper.ArrowToggle($"##{boneGroup.Key}", ref expanded);
                    ImGui.SameLine();
                    CtrlHelper.StaticLabel(boneGroup.Key.ToString());
                    if (BoneData.DisplayableFamilies.TryGetValue(boneGroup.Key, out var tip) && tip != null)
                        CtrlHelper.AddHoverText(tip);

                    if (expanded)
                    {
                        ImGui.TableNextRow();
                        foreach (var erp in boneGroup.OrderBy(x => BoneData.GetBoneRanking(x.BoneCodeName)))
                        {
                            CompleteBoneEditor(boneGroup.Key, erp);
                        }
                    }

                    _groupExpandedState[boneGroup.Key] = expanded;
                }
            }
        }
    }

    private void DrawEditorConfirmationPopup()
    {
        if (_openSavePopup)
        {
            ImGui.OpenPopup("SavePopup");
            _openSavePopup = false;
        }

        var viewportSize = ImGui.GetWindowViewport().Size;
        ImGui.SetNextWindowSize(new Vector2(viewportSize.X / 4, viewportSize.Y / 12));
        ImGui.SetNextWindowPos(viewportSize / 2, ImGuiCond.Always, new Vector2(0.5f));
        using var popup = ImRaii.Popup("SavePopup", ImGuiWindowFlags.Modal);
        if (!popup)
            return;

        ImGui.SetCursorPos(new Vector2(ImGui.GetWindowWidth() / 4 - 40, ImGui.GetWindowHeight() / 4));
        ImGuiUtil.TextWrapped("您在当前模板有未保存的修改，请选择需要的操作。");

        var buttonWidth = new Vector2(150 * ImGuiHelpers.GlobalScale, 0);
        var yPos = ImGui.GetWindowHeight() - 2 * ImGui.GetFrameHeight();
        var xPos = (ImGui.GetWindowWidth() - ImGui.GetStyle().ItemSpacing.X) / 4 - buttonWidth.X;
        ImGui.SetCursorPos(new Vector2(xPos, yPos));
        if (ImGui.Button("保存", buttonWidth))
        {
            _editorManager.SaveChanges();
            _editorManager.DisableEditor();

            ImGui.CloseCurrentPopup();
        }

        ImGui.SameLine();
        if (ImGui.Button("另存为副本", buttonWidth))
        {
            _editorManager.SaveChanges(true);
            _editorManager.DisableEditor();

            ImGui.CloseCurrentPopup();
        }

        ImGui.SameLine();
        if (ImGui.Button("不保存", buttonWidth))
        {
            _editorManager.DisableEditor();

            ImGui.CloseCurrentPopup();
        }

        ImGui.SameLine();
        if (ImGui.Button("继续编辑", buttonWidth))
        {
            ImGui.CloseCurrentPopup();
        }
    }

    #region ImGui helper functions

    private bool ResetBoneButton(EditRowParams bone)
    {
        var output = ImGuiComponents.IconButton(bone.BoneCodeName, FontAwesomeIcon.Recycle);
        CtrlHelper.AddHoverText(
            $"重设“{BoneData.GetBoneDisplayName(bone.BoneCodeName)}”为默认的{_editingAttribute}值");

        if (output)
        {
            _editorManager.ResetBoneAttributeChanges(bone.BoneCodeName, _editingAttribute);
            if (_isMirrorModeEnabled && bone.Basis?.TwinBone != null) //todo: put it inside manager
                _editorManager.ResetBoneAttributeChanges(bone.Basis.TwinBone.BoneName, _editingAttribute);
        }

        return output;
    }

    private bool RevertBoneButton(EditRowParams bone)
    {
        var output = ImGuiComponents.IconButton(bone.BoneCodeName, FontAwesomeIcon.ArrowCircleLeft);
        CtrlHelper.AddHoverText(
            $"恢复“{BoneData.GetBoneDisplayName(bone.BoneCodeName)}”到上一次保存的{_editingAttribute}值");

        if (output)
        {
            _editorManager.RevertBoneAttributeChanges(bone.BoneCodeName, _editingAttribute);
            if (_isMirrorModeEnabled && bone.Basis?.TwinBone != null) //todo: put it inside manager
                _editorManager.RevertBoneAttributeChanges(bone.Basis.TwinBone.BoneName, _editingAttribute);
        }

        return output;
    }

    private bool FullBoneSlider(string label, ref Vector3 value)
    {
        var velocity = _editingAttribute == BoneAttribute.Rotation ? 0.1f : 0.001f;
        var minValue = _editingAttribute == BoneAttribute.Rotation ? -360.0f : -10.0f;
        var maxValue = _editingAttribute == BoneAttribute.Rotation ? 360.0f : 10.0f;

        var temp = _editingAttribute switch
        {
            BoneAttribute.Position => 0.0f,
            BoneAttribute.Rotation => 0.0f,
            _ => value.X == value.Y && value.Y == value.Z ? value.X : 1.0f
        };


        ImGui.PushItemWidth(ImGui.GetColumnWidth());
        if (ImGui.DragFloat(label, ref temp, velocity, minValue, maxValue, $"%.{_precision}f"))
        {
            value = new Vector3(temp, temp, temp);
            return true;

        }

        return false;
    }

    private bool SingleValueSlider(string label, ref float value)
    {
        var velocity = _editingAttribute == BoneAttribute.Rotation ? 0.1f : 0.001f;
        var minValue = _editingAttribute == BoneAttribute.Rotation ? -360.0f : -10.0f;
        var maxValue = _editingAttribute == BoneAttribute.Rotation ? 360.0f : 10.0f;

        ImGui.PushItemWidth(ImGui.GetColumnWidth());
        var temp = value;
        if (ImGui.DragFloat(label, ref temp, velocity, minValue, maxValue, $"%.{_precision}f"))
        {
            value = temp;
            return true;
        }

        return false;
    }

    private void CompleteBoneEditor(BoneData.BoneFamily boneFamily, EditRowParams bone)
    {
        var codename = bone.BoneCodeName;
        var displayName = bone.BoneDisplayName;
        var transform = new BoneTransform(bone.Transform);

        var flagUpdate = false;

        var newVector = _editingAttribute switch
        {
            BoneAttribute.Position => transform.Translation,
            BoneAttribute.Rotation => transform.Rotation,
            _ => transform.Scaling
        };

        using var id = ImRaii.PushId(codename);
        ImGui.TableNextColumn();
        using (var disabled = ImRaii.Disabled(!_isUnlocked))
        {
            //----------------------------------
            ImGui.Dummy(new Vector2(CtrlHelper.IconButtonWidth * 0.75f, 0));
            ImGui.SameLine();
            ResetBoneButton(bone);
            ImGui.SameLine();
            RevertBoneButton(bone);

            //----------------------------------
            ImGui.TableNextColumn();
            flagUpdate |= SingleValueSlider($"##{displayName}-X", ref newVector.X);

            //----------------------------------
            ImGui.TableNextColumn();
            flagUpdate |= SingleValueSlider($"##{displayName}-Y", ref newVector.Y);

            //-----------------------------------
            ImGui.TableNextColumn();
            flagUpdate |= SingleValueSlider($"##{displayName}-Z", ref newVector.Z);

            //----------------------------------
            if (_editingAttribute != BoneAttribute.Scale)
                ImGui.BeginDisabled();

            ImGui.TableNextColumn();
            var tempVec = new Vector3(newVector.X, newVector.Y, newVector.Z);
            flagUpdate |= FullBoneSlider($"##{displayName}-All", ref newVector);

            if (_editingAttribute != BoneAttribute.Scale)
                ImGui.EndDisabled();
        }

        //----------------------------------
        ImGui.TableNextColumn();

        if((BoneData.IsIVCSCompatibleBone(codename) || boneFamily == BoneData.BoneFamily.未知 || boneFamily == BoneData.BoneFamily.摇晃)
            && !codename.StartsWith("j_f_"))
        {
            ImGui.PushStyleColor(ImGuiCol.Text, Constants.Colors.Warning);
            ImGuiUtil.PrintIcon(FontAwesomeIcon.Wrench);
            ImGui.PopStyleColor();
            CtrlHelper.AddHoverText("这是一个来自骨骼模组中的骨骼。" +
                "\r\n重要提示：Customize+ 团队不提供与这些骨骼相关的问题支持。" +
                "\r\n这些骨骼需要专门为其设计的特殊服装和身体模组。" +
                "\r\n即使它们是为这些骨骼设计的，并不是所有服装修改都支持每个骨骼。" +
                "\r\n如果遇到问题，请尝试使用摆姿势的工具执行相同操作。");
            ImGui.SameLine();
        }
        CtrlHelper.StaticLabel(displayName, CtrlHelper.TextAlignment.Left, BoneData.IsIVCSCompatibleBone(codename) ? $"(IVCS 兼容) {codename}" : codename);

        if (flagUpdate)
        {
            transform.UpdateAttribute(_editingAttribute, newVector);

            _editorManager.ModifyBoneTransform(codename, transform);
            if (_isMirrorModeEnabled && bone.Basis?.TwinBone != null) //todo: put it inside manager
                _editorManager.ModifyBoneTransform(bone.Basis.TwinBone.BoneName,
                    BoneData.IsIVCSCompatibleBone(codename) ? transform.GetSpecialReflection() : transform.GetStandardReflection());
        }

        ImGui.TableNextRow();
    }

    #endregion
}

/// <summary>
/// Simple structure for representing arguments to the editor table.
/// Can be constructed with or without access to a live armature.
/// </summary>
internal struct EditRowParams
{
    public string BoneCodeName;
    public string BoneDisplayName => BoneData.GetBoneDisplayName(BoneCodeName);
    public BoneTransform Transform;
    public ModelBone? Basis = null;

    public EditRowParams(ModelBone mb)
    {
        BoneCodeName = mb.BoneName;
        Transform = mb.CustomizedTransform ?? new BoneTransform();
        Basis = mb;
    }

    public EditRowParams(string codename, BoneTransform tr)
    {
        BoneCodeName = codename;
        Transform = tr;
        Basis = null;
    }
}