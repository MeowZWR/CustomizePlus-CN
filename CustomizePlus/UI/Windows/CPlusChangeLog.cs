using CustomizePlus.Configuration.Data;
using OtterGui.Widgets;

namespace CustomizePlus.UI.Windows;

public class CPlusChangeLog
{
    public const int LastChangelogVersion = 0;
    private readonly PluginConfiguration _config;
    public readonly Changelog Changelog;

    public CPlusChangeLog(PluginConfiguration config)
    {
        _config = config;
        Changelog = new Changelog("Customize+ 更新历史", ConfigData, Save);

        Add2_0_0_0(Changelog);
        Add2_0_1_0(Changelog);
        Add2_0_2_2(Changelog);
        Add2_0_3_0(Changelog);
        Add2_0_4_0(Changelog);
        Add2_0_4_1(Changelog);
        Add2_0_4_4(Changelog);
        Add2_0_5_0(Changelog);
        Add2_0_6_0(Changelog);
        Add2_0_6_3(Changelog);
        Add2_0_7_0(Changelog);
        Add2_0_7_2(Changelog);
        Add2_0_7_9(Changelog);
        Add2_0_7_15(Changelog);
        Add2_0_7_16(Changelog);
    }

    private (int, ChangeLogDisplayType) ConfigData()
        => (_config.ChangelogSettings.LastSeenVersion, _config.ChangelogSettings.ChangeLogDisplayType);

    private void Save(int version, ChangeLogDisplayType type)
    {
        _config.ChangelogSettings.LastSeenVersion = version;
        _config.ChangelogSettings.ChangeLogDisplayType = type;
        _config.Save();
    }

    private static void Add2_0_7_16(Changelog log)
        => log.NextVersion("版本 2.0.7.16")
        .RegisterImportant("支持游戏版本 7.2 和 Dalamud API 12。");

    private static void Add2_0_7_15(Changelog log)
        => log.NextVersion("版本 2.0.7.15")
            .RegisterEntry("优化了 Profile.GetByUniqueId IPC 方法返回的 JSON 数据负载。（由 Mare Synchronos 请求）")
            .RegisterEntry("不再返回默认值，显著减少了返回数据的大小。", 1)
            .RegisterEntry("修复剪贴板复制内容缺少版本数据的问题")
            .RegisterEntry("您无需进行任何操作。此修复仅解决剪贴板复制数据与磁盘数据之间的格式不一致问题。", 1)
            .RegisterEntry("修复了（部分？）\"ImGui 断言失败\"错误（2.0.7.14）")
            .RegisterEntry("改进了支持日志的内容（2.0.7.13）")
            .RegisterEntry("修复了在切换骨骼数量相同的发型时，骨架变更未被检测到的问题（2.0.7.11）")
            .RegisterEntry("修复了 GPose 中角色闪烁的问题（GPose 中不再应用根骨骼位置编辑）（2.0.7.10）")

            .RegisterEntry("源代码维护 - 更新外部库。");

    private static void Add2_0_7_9(Changelog log)
        => log.NextVersion("版本 2.0.7.9")
            .RegisterEntry("在设置标签页中添加了捐赠按钮。")
            .RegisterEntry("对当前配置文件所用模板的作出保存更改时，现在会通过发送 OnProfileUpdate IPC 事件通知其他插件配置文件已被更改。（2.0.7.8）")
            .RegisterEntry("编辑根骨骼位置现在无需角色移动即可生效。（2.0.7.6）")
            .RegisterEntry("修复了“应用于您登录的任何角色”配置文件选项被 Profile.GetActiveProfileIdOnCharacter IPC 函数忽略的问题，该问题导致其他插件无法检测此选项启用时的活动配置文件。（2.0.7.8）")
            .RegisterEntry("源代码维护 - 更新外部库。");

    private static void Add2_0_7_2(Changelog log)
        => log.NextVersion("版本 2.0.7.2")
        .RegisterHighlight("支持 7.1 和 Dalamud API 11。")
        .RegisterHighlight("修复了一个问题，该问题导致无法检测到拥有的角色（例如宝石兽和亲信NPC）。(2.0.7.1)")

        .RegisterEntry("源代码维护 - 更新外部库。");

    private static void Add2_0_7_0(Changelog log)
        => log.NextVersion("版本 2.0.7.0")
            .RegisterImportant("本次更新中 Customize+ 的某些部分进行了重大重写。如果您遇到任何问题，请报告。")

            .RegisterHighlight("角色配置已被重写。")
            .RegisterImportant("Customize+ 将尽力自动迁移您的配置到新系统，但在某些少见情况下，您可能需要重新在某些配置里添加角色。", 1)
            .RegisterEntry("角色选择用户界面已重新设计。", 1)
            .RegisterEntry("现在可以将多个角色分配给单个配置。", 2)
            .RegisterEntry("控制台命令的工作方式没有改变。这意味着命令将以与以前相同的方式影响配置，即使配置影响多个角色。", 3)
            .RegisterEntry("\"限制为从属我的角色\"选项已被移除，因为它现在已过时。", 2)
            .RegisterEntry("现在可以选择应用于您登录的任何角色的配置。", 2)
            .RegisterEntry("玩家拥有的 NPC（宠物、坐骑）现在应该可以通过 Mare Synchronos 正确同步。", 1)
            .RegisterEntry("非英语角色名称现在也许可以正常工作。请注意，这是一个附带效果，CN/KR 客户端仍然未正式支持。", 1)

            .RegisterHighlight("添加了配置优先级系统。")
            .RegisterEntry("当多个活动配置影响同一角色时，将使用配置优先级来确定应用于该角色的配置。", 1)

            .RegisterEntry("添加了额外选项以配置 Customize+ 窗口的行为。")
            .RegisterEntry("添加了配置选项，以决定在隐藏游戏用户界面时 Customize+ 窗口是否被隐藏。", 1)
            .RegisterEntry("添加了配置选项，以决定在进入 GPose 时 Customize+ 窗口是否被隐藏。", 1)
            .RegisterEntry("添加了配置选项，以决定在启动游戏时 Customize+ 主窗口是否自动打开。", 1)

            .RegisterImportant("为自定义骨骼添加了警告。如果您安装了自定义骨骼 - 请认真仔细地阅读。这些骨骼旁边有一个扳手图标。")
            .RegisterEntry("添加了在测试 Customize+ 的构建时出现的几条警告。")

            .RegisterHighlight("修复了 Customize+ 未能检测到角色骨骼变化的问题。这主要发生在通过 Glamourer 和其他插件/工具更改角色外观时。")

            .RegisterEntry("放弃支持从 Customize+ 1.0 升级。剪贴板复制不受此更改影响。")

            .RegisterEntry("IPC 说明，仅供开发者使用。")
            .RegisterImportant("IPC 版本现在是 6.0。", 1)
            .RegisterEntry("Profile.GetList 已更新，以包含配置优先级以及带有元数据的角色列表。请参考 Customize+ IPC 源代码文件以获取更多信息。", 1)
            .RegisterEntry("Profile.OnUpdate 事件现在会为启用了 \"应用于所有玩家和雇员\" 和 \"应用于您登录的任何角色\" 选项的配置触发。", 1)
            .RegisterEntry("Profile.SetTemporaryProfileOnCharacter 所需的配置 json 格式已更新。", 1)
            .RegisterEntry("CharacterName 字段已删除。", 2)
            .RegisterEntry("添加了一些为未来功能保留的字段。", 2)
            .RegisterEntry("临时配置现在应该正确应用于拥有的角色，如宠物。", 1)

            .RegisterEntry("源代码维护 - 外部库更新。");

    private static void Add2_0_6_3(Changelog log)
        => log.NextVersion("版本 2.0.6.3")
            .RegisterEntry("添加了新的 IPC 方法：GameState.GetCutsceneParentIndex, GameState.SetCutsceneParentIndex。")
            .RegisterImportant("这些方法是 Ktisis 开发者请求的。建议其他开发者除非绝对确定自己在做什么，否则不要使用它们。", 1)
            .RegisterEntry("改进了支持日志。 (2.0.6.2)")
            .RegisterEntry("调整了日志记录，使其在 \"Debug+\" 模式下减少冗余。")
            .RegisterEntry("使在角色选择界面的处理更可靠。 (2.0.6.1, 2.0.6.3)")
            .RegisterEntry("修复了在集体动作中角色处理不正确的问题。")
            .RegisterEntry("源代码维护 - 更新外部库。");

    private static void Add2_0_6_0(Changelog log)
	    => log.NextVersion("版本 2.0.6.0")
	        .RegisterHighlight("IPC 已重新启用。")
	        .RegisterImportant("如果你是普通用户，你需要等待其他插件实施必要的更改。请咨询这些插件的开发者获取更多信息。", 1)
	        .RegisterImportant("重大变更：IPC 版本已升级至 5.0。", 1)
	        .RegisterImportant("重大变更：所有功能现在都使用对象表索引进行操作。这是为了与其他主要插件的处理方式保持一致，并尽量减少再度受到 Dalamud 错误影响的可能性。", 1)
	        .RegisterHighlight("「金曦之遗辉」面部骨骼已分类。由 Kaze 贡献。(2.0.5.1)")
	        .RegisterEntry("将所有IVCS骨骼重命名为“IVCS 兼容”，以反映现在可以使用其他IVCS兼容的骨架来支持 IVCS 模组。")
	        .RegisterEntry("修复了根骨无法处理负值的问题。")
	        .RegisterEntry("修复了打开冒险者名片窗口时引发的问题。");

	private static void Add2_0_5_0(Changelog log)
	    => log.NextVersion("版本 2.0.5.0")
	        .RegisterHighlight("Customize+ 已更新以支持「金曦之遗辉」。")
	        .RegisterImportant("如果你编辑了任何面部骨骼，你可能需要调整这些编辑。", 1)
	        .RegisterImportant("已知问题：", 1)
	        .RegisterImportant("在登录大厅角色选择界面不会应用配置文件。", 2)
	        .RegisterImportant("所有新的「金曦之遗辉」骨骼都被归入“未知”类别。", 2)
	        .RegisterImportant("IPC需要额外的工作，目前已被禁用。如果你仍然调用它，可能会出现问题。", 2)
	        .RegisterEntry("在设置标签页中添加了“复制支持信息到剪贴板”按钮。")
	        .RegisterEntry("将“默认配置文件”重命名为“应用于所有玩家和随从”，以帮助用户更好地理解此功能。(2.0.4.5)")
	        .RegisterEntry("当“应用于所有玩家和随从”启用时，改进了用户界面的行为。(2.0.4.5)");

	private static void Add2_0_4_4(Changelog log)
	    => log.NextVersion("版本 2.0.4.4")
	        .RegisterHighlight("在配置文件编辑器的模板选择器中添加了编辑按钮，允许快速开始编辑关联模板。")
	        .RegisterEntry("修复了“仅限我的从属角色”设置未正常工作的问题。(2.0.4.2)")
	        .RegisterEntry("添加了额外的日志记录。(2.0.4.2)");

	private static void Add2_0_4_1(Changelog log)
	    => log.NextVersion("版本 2.0.4.1")
	        .RegisterEntry("添加了对新世界的支持。")
	        .RegisterEntry("源代码维护 - 更新外部库。");

    private static void Add2_0_4_0(Changelog log)
        => log.NextVersion("版本 2.0.4.0")
            .RegisterImportant("已移除 版本 3 IPC，任何仍依赖于它的插件在更新之前将停止工作。")
            .RegisterEntry("Mare Synchronos 和 Dynamic Bridge 不受影响。", 1)
            .RegisterEntry("新增选项以配置是否在登录期间的角色选择界面上应用配置文件。")
            .RegisterEntry("减少信息级插件日志的冗长程度。")
            .RegisterEntry("源代码维护 - 更新外部库。");

	private static void Add2_0_3_0(Changelog log)
	    => log.NextVersion("版本 2.0.3.0")
	        .RegisterEntry("添加了配置选项，用于确定配置文件是否影响游戏用户界面的各个部分：")
	        .RegisterEntry("角色窗口", 1)
	        .RegisterEntry("试穿、染色预览、投影窗口", 1)
	        .RegisterEntry("冒险者铭牌 (肖像)", 1)
	        .RegisterEntry("调查窗口", 1)
	        .RegisterEntry("添加了配置选项，用于确定模板编辑器预览角色是否在登录时自动设置为当前角色。默认情况下禁用。")
	        .RegisterEntry("启用的配置文件不再能被设置为默认配置文件。")
	        .RegisterEntry("修复了当前玩家角色的配置文件应用于其他角色的特殊角色 (肖像等) 的问题。")
	        .RegisterEntry("修复了关闭具有活动临时配置文件的角色检查窗口时临时配置文件被移除的问题。")
	        .RegisterEntry("修复了在做完Penumbra重绘后马上启用另一个配置文件时配置文件未应用的问题。")
	        .RegisterEntry("修复了切换到不同配置文件时不反映在特殊角色 (肖像等) 上的问题。")
	        .RegisterEntry("修复了遗留 IPC 的 `RevertCharacter` 方法泄漏异常的问题。 (2.0.2.4)")
	        .RegisterEntry("源代码维护 - 外部库更新、重构、清理。");

    private static void Add2_0_2_2(Changelog log)
        => log.NextVersion("Version 2.0.2.2")
            .RegisterHighlight("Added brand new IPC (version 4) for cross-plugin interraction. (2.0.2.0)")
            .RegisterEntry("Please refer to repository readme on GitHub for information about using it.", 1)
            .RegisterImportant("Old IPC (version 3) is still available, but it will be removed sometime before Dawntrail release. Plugin developers are advised to migrate as soon as possible.", 1)
            .RegisterEntry("Updated to .NET 8. (2.0.2.0)")
            .RegisterEntry("Updated external libraries. (2.0.2.1)")
            .RegisterEntry("Added additional cleanup of user input. (2.0.2.0)")
            .RegisterEntry("Selected default profile can no longer be changed if profile set as default is enabled. (2.0.2.1)")
            .RegisterEntry("Profiles can no longer be enabled/disabled while editor is active. (2.0.2.1)")
            .RegisterEntry("Fixed incorrect warning message priorities in main window. (2.0.2.1)")
            .RegisterEntry("Fixed \"Limit to my creatures\" not ignoring objects other than summons, minions and mounts. (2.0.2.1)")
            .RegisterEntry("Fixed text in various places. (2.0.2.1)");

    private static void Add2_0_1_0(Changelog log)
        => log.NextVersion("Version 2.0.1.0")
            .RegisterHighlight("Added support for legacy clipboard copies.")
            .RegisterEntry("Added setting allowing disabling of confirmation messages for chat commands.")
            .RegisterEntry("Template and profile editing is no longer disabled during GPose.")
            .RegisterImportant("Customize+ is not 100% compatible with posing tools such as Ktisis, Brio and Anamnesis. Some features of those tools might alter Customize+ behavior or prevent it from working.", 1)
            .RegisterHighlight("Fixed crash during \"Duty Complete\" cutscenes.")
            .RegisterEntry("Fixed settings migration failing completely if one of the profiles is corrupted.")
            .RegisterEntry("Improved error handling.")
            .RegisterHighlight("Customize+ window will now display warning message if plugin encounters a critical error.", 1);
   
    private static void Add2_0_0_0(Changelog log)
        => log.NextVersion("Version 2.0.0.0")
            .RegisterHighlight("Major rework of the entire plugin.")
            .RegisterEntry("Settings and profiles from previous version will be automatically converted to new format on the first load.", 1)
            .RegisterImportant("Old version configuration is backed up in case something goes wrong, please report any issues with configuration migration as soon as possible.", 2)
            .RegisterImportant("Clipboard copies from previous versions are not currently supported.", 2)
            .RegisterImportant("Profiles from previous versions will only be loaded during first load.", 2)

            .RegisterHighlight("Major changes:")

            .RegisterEntry("Plugin has been almost completely rewritten from scratch.", 1)

            .RegisterEntry("User interface has been moved to the framework used by Glamourer and Penumbra, so the interface should feel familiar to the users of those plugins.", 1)
            .RegisterEntry("User interface issues related to different resolutions and font sizes should *mostly* not occur anymore.", 2)
            .RegisterImportant("There are several issues with text not fitting in some places depending on your screen resolution and font size. This will be fixed later.", 3)

            .RegisterEntry("Template system has been added", 1)
            .RegisterEntry("All bone edits are now stored in templates which can be used by multiple profiles and single profile can reference unlimited number of templates.", 2)

            .RegisterImportant("Chat commands have been changed, refer to \"/customize help\" for information about available commands.", 1)

            .RegisterEntry("Profiles can be applied to summons, mounts and pets without any limitations.", 1)
            .RegisterImportant("Root scaling of mounts is not available for now.", 2)

            .RegisterEntry("Fixed \"Only owned\" not working properly in various cases and renamed it to \"Limit to my creatures\".", 1)

            .RegisterEntry("Fixed profiles \"leaking\" to other characters due to the way original Mare Synchronos integration implementation was handled.", 1)

            .RegisterEntry("Compatibility with cutscenes is improved, but that was not extensively tested.", 1)

            .RegisterEntry("Plugin configuration is now being regularly backed up, the backup is located in %appdata%\\XIVLauncher\\backups\\CustomizePlus folder", 1);
}
