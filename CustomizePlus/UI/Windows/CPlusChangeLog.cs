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
    }

    private (int, ChangeLogDisplayType) ConfigData()
        => (_config.ChangelogSettings.LastSeenVersion, _config.ChangelogSettings.ChangeLogDisplayType);

    private void Save(int version, ChangeLogDisplayType type)
    {
        _config.ChangelogSettings.LastSeenVersion = version;
        _config.ChangelogSettings.ChangeLogDisplayType = type;
        _config.Save();
    }

    private static void Add2_0_7_0(Changelog log)
        => log.NextVersion("Version 2.0.7.0")
        .RegisterImportant("Some parts of Customize+ have been considerably rewritten in this update. If you encounter any issues please report them.")

        .RegisterHighlight("Character management has been rewritten.")
        .RegisterImportant("Customize+ will do its best to automatically migrate your profiles to new system but in some rare cases it is possible that you will have to add characters again for some of your profiles.", 1)
        .RegisterEntry("Character selection user interface has been redesigned.", 1)
        .RegisterEntry("It is now possible to assign several characters to a single profile.", 2)
        .RegisterEntry("The way console commands work has not changed. This means that the commands will affect profiles the same way as before, even if profile affects multiple characters.", 3)
        .RegisterEntry("\"Limit to my creatures\" option has been removed as it is now obsolete.", 2)
        .RegisterEntry("It is now possible to choose profile which will be applied to any character you login with.", 2)
        .RegisterEntry("Player-owned NPCs (minions, mounts) should now correctly synchronize via Mare Synchronos.", 1)
        .RegisterEntry("It is possible that non-english character names are now working properly. Please note that this is a side effect and CN/KR clients are still not officially supported.", 1)

        .RegisterHighlight("Added profile priority system.")
        .RegisterEntry("When several active profiles affect the same character, profile priority will be used to determine which profile will be applied to said character.", 1)

        .RegisterEntry("Added additional options to configure how Customize+ window behaves.")
        .RegisterEntry("Added option to configure if Customize+ windows will be hidden when you hide game UI or not.", 1)
        .RegisterEntry("Added option to configure if Customize+ windows will be hidden when you enter GPose or not.", 1)
        .RegisterEntry("Added option to configure if Customize+ main window will be automatically opened when you launch the game or not.", 1)

        .RegisterImportant("Added warning for custom skeleton bones. If you have custom skeleton installed - read it. Seriously. It's a wrench icon near the name of those bones.")
        .RegisterEntry("Added several warnings when testing build of Customize+ is being used.")

        .RegisterHighlight("Fixed issue when Customize+ did not detect changes in character skeleton. This mostly happened when altering character appearance via Glamourer and other plugins/tools.")

        .RegisterEntry("Dropped support for upgrading from Customize+ 1.0. Clipboard copies are not affected by this change.")

        .RegisterEntry("IPC notes, developers only.")
        .RegisterImportant("IPC version is now 6.0.", 1)
        .RegisterEntry("Profile.GetList has been updated to include profile priority as well as list of characters with their metadata. Please refer to Customize+ IPC source code files for additional information.", 1)
        .RegisterEntry("Profile.OnUpdate event is now being triggered for profiles with \"Apply to all players and retainers\" and \"Apply to any character you are logged in with\" options enabled.", 1)
        .RegisterEntry("Format of the profile json expected by Profile.SetTemporaryProfileOnCharacter has been updated.", 1)
        .RegisterEntry("CharacterName field removed.", 2)
        .RegisterEntry("Added few fields reserved for the future functionality.", 2)
        .RegisterEntry("Temporary profiles should now apply correctly to owned characters like minions.", 1)

        .RegisterEntry("Source code maintenance - external libraries update.");

    private static void Add2_0_6_3(Changelog log)
        => log.NextVersion("版本 2.0.6.3")
            .RegisterEntry("添加了新的 IPC 方法：GameState.GetCutsceneParentIndex, GameState.SetCutsceneParentIndex。")
            .RegisterImportant("这些方法是 Ktisis 开发者请求的。建议其他开发者除非绝对确定自己在做什么，否则不要使用它们。", 1)
            .RegisterEntry("改进了支持日志。 (2.0.6.2)")
            .RegisterEntry("调整了日志记录，使其在 \"Debug+\" 模式下减少冗余。")
            .RegisterEntry("使在角色选择界面的处理更可靠。 (2.0.6.1, 2.0.6.3)")
            .RegisterEntry("修复了在集体动作中角色处理不正确的问题。")
            .RegisterEntry("源代码维护 - 外部库更新。");

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
