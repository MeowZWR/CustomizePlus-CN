using System.Collections.Generic;

namespace CustomizePlus.UI;

public enum ColorId
{
    UsedTemplate,
    UnusedTemplate,
    EnabledProfile,
    DisabledProfile,
    LocalCharacterEnabledProfile,
    LocalCharacterDisabledProfile,
    FolderExpanded,
    FolderCollapsed,
    FolderLine,
    HeaderButtons,
}

public static class Colors
{
    public const uint SelectedRed = 0xFF2020D0;

    public static (uint DefaultColor, string Name, string Description) Data(this ColorId color)
        => color switch
        {
            // @formatter:off
            ColorId.UsedTemplate => (0xFFFFFFFF, "已使用模板", "至少有一个配置文件正在使用的模板。"),
            //ColorId.EnabledTemplate => (0xFFA0F0A0, "已启用的自动执行集", "当前启用的自动执行集。每个标识符一次只能启用一个集。"),
            ColorId.UnusedTemplate => (0xFF808080, "未使用模板", "当前未被任何配置文件使用的模板。"),
            ColorId.EnabledProfile => (0xFFFFFFFF, "已启用配置文件", "当前启用的配置文件。"),
            ColorId.DisabledProfile => (0xFF808080, "已禁用配置文件", "当前已禁用的配置文件"),
            ColorId.LocalCharacterEnabledProfile => (0xFF18C018, "当前角色配置文件（启用）", "当前启用且与您的角色关联的配置文件。"),
            ColorId.LocalCharacterDisabledProfile => (0xFF808080, "当前角色配置文件（禁用）", "当前禁用且与您的角色关联的配置文件。"),
            ColorId.FolderExpanded => (0xFFFFF0C0, "已展开折叠组", "当前已展开的文件夹。"),
            ColorId.FolderCollapsed => (0xFFFFF0C0, "已收起折叠组", "当前已折叠的文件夹。"),
            ColorId.FolderLine => (0xFFFFF0C0, "折叠组树形目录结构线", "标记哪些子项属于已展开折叠组的线条。"),
            ColorId.HeaderButtons => (0xFFFFF0C0, "标题按钮", "标题中的按钮的文本和边框颜色，如锁定切换。"),
            _ => (0x00000000, string.Empty, string.Empty),
            // @formatter:on
        };

    private static IReadOnlyDictionary<ColorId, uint> _colors = new Dictionary<ColorId, uint>();

    /// <summary> Obtain the configured value for a color. </summary>
    public static uint Value(this ColorId color)
        => _colors.TryGetValue(color, out var value) ? value : color.Data().DefaultColor;

    /// <summary> Set the configurable colors dictionary to a value. </summary>
    /*public static void SetColors(Configuration config)
        => _colors = config.Colors;*/
}
