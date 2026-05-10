using CustomizePlus.Configuration.Data;

namespace CustomizePlus.UI.Windows;

//Versioning concept (X.Y.Z.W):
//X - major version, changes only during major rewrites of the plugin
//Y - major feature version, changes when new major features are introduced (also can be changed with major game patches)
//Z - minor feature version, changes when new minor features are introduced
//W - bugfix version, changes when the update only contains bugfixes.

public class CPlusChangeLog
{
    public const int LastChangelogVersion = 0;
    private readonly PluginConfiguration _configuration;
    public readonly Changelog Changelog;

    public CPlusChangeLog(PluginConfiguration configuration)
    {
        _configuration = configuration;
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
        Add2_0_7_23(Changelog);
        Add2_0_7_27(Changelog);
        Add2_0_8_0(Changelog);
        Add2_0_8_2(Changelog);
        Add2_0_8_4(Changelog);
        Add2_0_9_0(Changelog);
        Add2_1_0_0(Changelog);
        Add2_1_1_0(Changelog);
        Add2_2_0_0(Changelog);
    }

    private (int, ChangeLogDisplayType) ConfigData()
        => (_configuration.ChangelogSettings.LastSeenVersion, _configuration.ChangelogSettings.ChangeLogDisplayType);

    private void Save(int version, ChangeLogDisplayType type)
    {
        _configuration.ChangelogSettings.LastSeenVersion = version;
        _configuration.ChangelogSettings.ChangeLogDisplayType = type;
        _configuration.Save();
    }


    private static void Add2_2_0_0(Changelog log)
        => log.NextVersion("版本 2.2.0.0"u8)
        .RegisterImportant("支持 7.5 与 Dalamud API 15。（由 Abelfreyja 提供）"u8)
        .RegisterHighlight("插件已迁移至 Luna 框架。（由 Risa 实现，初稿 Abelfreyja，初稿审阅 Exter-N）"u8)
        .RegisterEntry("因此 Penumbra 与 Glamourer 近期的大量界面体验改进也可在 Customize+ 中使用。"u8, 1)
        .RegisterImportant("尽管已进行广泛测试，仍可能存在问题；如遇异常请反馈。"u8, 1)
        .RegisterEntry("骨骼编辑器列为各轴增加了颜色标识。（由 Abelfreyja 提供）"u8)
        .RegisterEntry("修复了「未保存的更改」窗口在不同分辨率与 UI 缩放下布局错乱的问题。（由 NalaPraline 提供）（2.1.1.2）"u8)
        .RegisterImportant("若曾使用本插件的非官方构建，可能出现数据丢失、崩溃等问题；这不属于官方插件缺陷，请勿就此反馈。"u8)
        .RegisterImportant("使用同步类插件与非官方构建用户同步可能导致崩溃。"u8, 1)
        .RegisterImportant("更多说明请见支持 Discord 中的公告（可通过设置页中的按钮加入）。"u8, 1);

    private static void Add2_1_1_0(Changelog log)
        => log.NextVersion("版本 2.1.1.0"u8)
        .RegisterEntry("新增将整个角色配置导出为单一模板的按钮。（由 MBadea21 提供）"u8);

    private static void Add2_1_0_0(Changelog log)
        => log.NextVersion("版本 2.1.0.0"u8)
        .RegisterImportant("支持 7.4 和 Dalamud API 14.（由 Risa 提供）"u8);

    private static void Add2_0_9_0(Changelog log)
        => log.NextVersion("版本 2.0.9.0"u8)
        .RegisterEntry("新增：现在可以在骨骼启用传播功能时，对子骨骼应用独立的缩放比例。（由 Midona 提供）"u8)
        .RegisterEntry("当在“缩放”选项上启用变换传播时，您现在将看到一个额外的「子骨骼」条目，用于更精细地控制骨骼缩放；对调整尾巴形状等特别有用。"u8, 1);
    
    private static void Add2_0_8_4(Changelog log)
        => log.NextVersion("版本 2.0.8.4"u8)
        .RegisterEntry("Customize+ 现在可以操纵配饰。（由 Caraxi 提供）"u8)
        .RegisterEntry("可操纵的范围取决于所选配饰。"u8, 1)
        .RegisterEntry("当“显示实时骨骼”关闭且启用了对子节点应用变换选项时，数值为 0 的骨骼不再会从编辑器中移除。（由 Caraxi 和 Risa 提供）（2.0.8.3）"u8);

    private static void Add2_0_8_2(Changelog log)
        => log.NextVersion("版本 2.0.8.2"u8)
        .RegisterEntry("提升了 Penumbra PCP 集成的稳定性。（由 abelfreyja 提供）"u8)
        .RegisterEntry("Customize+ 现在如果无法连接 Penumbra，会在菜单栏显示警告。（由 Risa 提供）"u8, 1)
        .RegisterEntry("修复了根位置重置在不应生效时被应用的问题。（由 abelfreyja 提供）"u8)
        .RegisterEntry("修复了配置文件文件夹会被重置的问题。（由 Risa 提供）"u8);

    private static void Add2_0_8_0(Changelog log)
        => log.NextVersion("版本 2.0.8.0"u8)
        .RegisterHighlight("新增对 Penumbra PCP 文件的支持。（由 abelfreyja 提供）"u8)
        .RegisterEntry("该功能默认启用，可在设置 -> 集成菜单中关闭。"u8, 1)
        .RegisterHighlight("新增骨骼编辑传播功能。（由 d87 提供）"u8)
        .RegisterEntry("该功能在某些骨骼或某些骨骼编辑组合下可能无法正常工作。"u8, 1)
        .RegisterEntry("骨骼编辑时新增搜索过滤和撤销/重做功能。（由 abelfreyja 提供）"u8)
        .RegisterEntry("新增将骨骼分组复制到剪贴板及导入的功能。（由 abelfreyja 提供）"u8)
        .RegisterEntry("右键点击分组名称可访问此功能。"u8, 1)
        .RegisterEntry("新增收藏骨骼功能。（由 abelfreyja 和 Risa 提供）"u8)
        .RegisterEntry("IPC 版本更新至 6.3。"u8)
        .RegisterEntry("新增 Profile.SetPriorityByUniqueId IPC 端点。（由 CordeliaMist 提供）"u8, 1)
        .RegisterEntry("骨骼传播设置现在会在适用时返回。（由 abelfreyja 提供）"u8, 1);

    private static void Add2_0_7_27(Changelog log)
        => log.NextVersion("版本 2.0.7.27"u8)
        .RegisterEntry("新增可在不移除模板的情况下切换配置文件中的模板启用状态。（由 Caraxi 提供）"u8)
        .RegisterEntry("IPC 版本更新至 6.2。"u8)
        .RegisterEntry("新增 Profile.GetTemplates、Profile.EnableTemplateByUniqueId、Profile.DisableTemplateByUniqueId IPC 端点。（由 Caraxi 提供）"u8, 1)
        .RegisterEntry("修复当存在空名称模板/配置文件时，尝试打开模板/配置文件标签页会崩溃的问题。（2.0.7.25）"u8);

    private static void Add2_0_7_23(Changelog log)
        => log.NextVersion("版本 2.0.7.23"u8)
        .RegisterImportant("支持游戏版本 7.3 和 Dalamud API 13。"u8)
        .RegisterEntry("IPC 版本更新至 6.1。(2.0.7.20)"u8)
        .RegisterEntry("添加了 Profile.AddPlayerCharacter 和 Profile.RemovePlayerCharacter IPC 端点。（由 Caraxi 提供）"u8, 1)
        .RegisterEntry("“模板”和“配置文件”标签页中的左侧选择器现在可以调整大小。"u8)
        .RegisterEntry("修复了登录/登出时的崩溃问题。"u8)
        .RegisterEntry("这通常发生在设置中启用了“在角色选择界面应用配置文件”和/或“自动将当前角色设为编辑器预览角色”选项时。"u8, 1)
        .RegisterEntry("修复了在配置文件之间切换时，根变换有时不会重置直到角色移动的问题。"u8)
        .RegisterEntry("修复了配置文件会尝试应用到当前未在屏幕上绘制的对象的问题。"u8)
        .RegisterEntry("对用户界面代码进行了轻微重构。"u8);

    private static void Add2_0_7_16(Changelog log)
        => log.NextVersion("版本 2.0.7.16"u8)
        .RegisterImportant("支持游戏版本 7.2 和 Dalamud API 12。"u8);

    private static void Add2_0_7_15(Changelog log)
        => log.NextVersion("版本 2.0.7.15"u8)
            .RegisterEntry("优化了 Profile.GetByUniqueId IPC 方法返回的 JSON 数据负载。（由 Mare Synchronos 请求）"u8)
            .RegisterEntry("不再返回默认值，显著减少了返回数据的大小。"u8, 1)
            .RegisterEntry("修复剪贴板复制内容缺少版本数据的问题"u8)
            .RegisterEntry("您无需进行任何操作。此修复仅解决剪贴板复制数据与磁盘数据之间的格式不一致问题。"u8, 1)
            .RegisterEntry("修复了（部分？）\"ImGui 断言失败\"错误（2.0.7.14）"u8)
            .RegisterEntry("改进了支持日志的内容（2.0.7.13）"u8)
            .RegisterEntry("修复了在切换骨骼数量相同的发型时，骨架变更未被检测到的问题（2.0.7.11）"u8)
            .RegisterEntry("修复了 GPose 中角色闪烁的问题（GPose 中不再应用根骨骼位置编辑）（2.0.7.10）"u8)

            .RegisterEntry("源代码维护 - 更新外部库。"u8);

    private static void Add2_0_7_9(Changelog log)
        => log.NextVersion("版本 2.0.7.9"u8)
            .RegisterEntry("在设置标签页中添加了捐赠按钮。"u8)
            .RegisterEntry("对当前配置文件所用模板的作出保存更改时，现在会通过发送 OnProfileUpdate IPC 事件通知其他插件配置文件已被更改。（2.0.7.8）"u8)
            .RegisterEntry("编辑根骨骼位置现在无需角色移动即可生效。（2.0.7.6）"u8)
            .RegisterEntry("修复了“应用于您登录的任何角色”配置文件选项被 Profile.GetActiveProfileIdOnCharacter IPC 函数忽略的问题，该问题导致其他插件无法检测此选项启用时的活动配置文件。（2.0.7.8）"u8)
            .RegisterEntry("源代码维护 - 更新外部库。"u8);

    private static void Add2_0_7_2(Changelog log)
        => log.NextVersion("版本 2.0.7.2"u8)
        .RegisterHighlight("支持 7.1 和 Dalamud API 11。"u8)
        .RegisterHighlight("修复了一个问题，该问题导致无法检测到拥有的角色（例如宝石兽和亲信NPC）。(2.0.7.1)"u8)

        .RegisterEntry("源代码维护 - 更新外部库。"u8);

    private static void Add2_0_7_0(Changelog log)
        => log.NextVersion("版本 2.0.7.0"u8)
            .RegisterImportant("本次更新中 Customize+ 的某些部分进行了重大重写。如果您遇到任何问题，请报告。"u8)

            .RegisterHighlight("角色配置已被重写。"u8)
            .RegisterImportant("Customize+ 将尽力自动迁移您的配置到新系统，但在某些少见情况下，您可能需要重新在某些配置里添加角色。"u8, 1)
            .RegisterEntry("角色选择用户界面已重新设计。"u8, 1)
            .RegisterEntry("现在可以将多个角色分配给单个配置。"u8, 2)
            .RegisterEntry("控制台命令的工作方式没有改变。这意味着命令将以与以前相同的方式影响配置，即使配置影响多个角色。"u8, 3)
            .RegisterEntry("\"限制为从属我的角色\"选项已被移除，因为它现在已过时。"u8, 2)
            .RegisterEntry("现在可以选择应用于您登录的任何角色的配置。"u8, 2)
            .RegisterEntry("玩家拥有的 NPC（宠物、坐骑）现在应该可以通过 Mare Synchronos 正确同步。"u8, 1)
            .RegisterEntry("非英语角色名称现在也许可以正常工作。请注意，这是一个附带效果，CN/KR 客户端仍然未正式支持。"u8, 1)

            .RegisterHighlight("添加了配置优先级系统。"u8)
            .RegisterEntry("当多个活动配置影响同一角色时，将使用配置优先级来确定应用于该角色的配置。"u8, 1)

            .RegisterEntry("添加了额外选项以配置 Customize+ 窗口的行为。"u8)
            .RegisterEntry("添加了配置选项，以决定在隐藏游戏用户界面时 Customize+ 窗口是否被隐藏。"u8, 1)
            .RegisterEntry("添加了配置选项，以决定在进入 GPose 时 Customize+ 窗口是否被隐藏。"u8, 1)
            .RegisterEntry("添加了配置选项，以决定在启动游戏时 Customize+ 主窗口是否自动打开。"u8, 1)

            .RegisterImportant("为自定义骨骼添加了警告。如果您安装了自定义骨骼 - 请认真仔细地阅读。这些骨骼旁边有一个扳手图标。"u8)
            .RegisterEntry("添加了在测试 Customize+ 的构建时出现的几条警告。"u8)

            .RegisterHighlight("修复了 Customize+ 未能检测到角色骨骼变化的问题。这主要发生在通过 Glamourer 和其他插件/工具更改角色外观时。"u8)

            .RegisterEntry("放弃支持从 Customize+ 1.0 升级。剪贴板复制不受此更改影响。"u8)

            .RegisterEntry("IPC 说明，仅供开发者使用。"u8)
            .RegisterImportant("IPC 版本现在是 6.0。"u8, 1)
            .RegisterEntry("Profile.GetList 已更新，以包含配置优先级以及带有元数据的角色列表。请参考 Customize+ IPC 源代码文件以获取更多信息。"u8, 1)
            .RegisterEntry("Profile.OnUpdate 事件现在会为启用了 \"应用于所有玩家和雇员\" 和 \"应用于您登录的任何角色\" 选项的配置触发。"u8, 1)
            .RegisterEntry("Profile.SetTemporaryProfileOnCharacter 所需的配置 json 格式已更新。"u8, 1)
            .RegisterEntry("CharacterName 字段已删除。"u8, 2)
            .RegisterEntry("添加了一些为未来功能保留的字段。"u8, 2)
            .RegisterEntry("临时配置现在应该正确应用于拥有的角色，如宠物。"u8, 1)

            .RegisterEntry("源代码维护 - 外部库更新。"u8);

    private static void Add2_0_6_3(Changelog log)
        => log.NextVersion("版本 2.0.6.3"u8)
            .RegisterEntry("添加了新的 IPC 方法：GameState.GetCutsceneParentIndex, GameState.SetCutsceneParentIndex。"u8)
            .RegisterImportant("这些方法是 Ktisis 开发者请求的。建议其他开发者除非绝对确定自己在做什么，否则不要使用它们。"u8, 1)
            .RegisterEntry("改进了支持日志。 (2.0.6.2)"u8)
            .RegisterEntry("调整了日志记录，使其在 \"Debug+\" 模式下减少冗余。"u8)
            .RegisterEntry("使在角色选择界面的处理更可靠。 (2.0.6.1, 2.0.6.3)"u8)
            .RegisterEntry("修复了在集体动作中角色处理不正确的问题。"u8)
            .RegisterEntry("源代码维护 - 更新外部库。"u8);

    private static void Add2_0_6_0(Changelog log)
	    => log.NextVersion("版本 2.0.6.0"u8)
	        .RegisterHighlight("IPC 已重新启用。"u8)
	        .RegisterImportant("如果你是普通用户，你需要等待其他插件实施必要的更改。请咨询这些插件的开发者获取更多信息。"u8, 1)
	        .RegisterImportant("重大变更：IPC 版本已升级至 5.0。"u8, 1)
	        .RegisterImportant("重大变更：所有功能现在都使用对象表索引进行操作。这是为了与其他主要插件的处理方式保持一致，并尽量减少再度受到 Dalamud 错误影响的可能性。"u8, 1)
	        .RegisterHighlight("「金曦之遗辉」面部骨骼已分类。由 Kaze 贡献。(2.0.5.1)"u8)
	        .RegisterEntry("将所有IVCS骨骼重命名为“IVCS 兼容”，以反映现在可以使用其他IVCS兼容的骨架来支持 IVCS 模组。"u8)
	        .RegisterEntry("修复了根骨无法处理负值的问题。"u8)
	        .RegisterEntry("修复了打开冒险者名片窗口时引发的问题。"u8);

	private static void Add2_0_5_0(Changelog log)
	    => log.NextVersion("版本 2.0.5.0"u8)
	        .RegisterHighlight("Customize+ 已更新以支持「金曦之遗辉」。"u8)
	        .RegisterImportant("如果你编辑了任何面部骨骼，你可能需要调整这些编辑。"u8, 1)
	        .RegisterImportant("已知问题："u8, 1)
	        .RegisterImportant("在登录大厅角色选择界面不会应用配置文件。"u8, 2)
	        .RegisterImportant("所有新的「金曦之遗辉」骨骼都被归入“未知”类别。"u8, 2)
	        .RegisterImportant("IPC需要额外的工作，目前已被禁用。如果你仍然调用它，可能会出现问题。"u8, 2)
	        .RegisterEntry("在设置标签页中添加了“复制支持信息到剪贴板”按钮。"u8)
	        .RegisterEntry("将“默认配置文件”重命名为“应用于所有玩家和随从”，以帮助用户更好地理解此功能。(2.0.4.5)"u8)
	        .RegisterEntry("当“应用于所有玩家和随从”启用时，改进了用户界面的行为。(2.0.4.5)"u8);

	private static void Add2_0_4_4(Changelog log)
	    => log.NextVersion("版本 2.0.4.4"u8)
	        .RegisterHighlight("在配置文件编辑器的模板选择器中添加了编辑按钮，允许快速开始编辑关联模板。"u8)
	        .RegisterEntry("修复了“仅限我的从属角色”设置未正常工作的问题。(2.0.4.2)"u8)
	        .RegisterEntry("添加了额外的日志记录。(2.0.4.2)"u8);

	private static void Add2_0_4_1(Changelog log)
	    => log.NextVersion("版本 2.0.4.1"u8)
	        .RegisterEntry("添加了对新世界的支持。"u8)
	        .RegisterEntry("源代码维护 - 更新外部库。"u8);

    private static void Add2_0_4_0(Changelog log)
        => log.NextVersion("版本 2.0.4.0"u8)
            .RegisterImportant("已移除 版本 3 IPC，任何仍依赖于它的插件在更新之前将停止工作。"u8)
            .RegisterEntry("Mare Synchronos 和 Dynamic Bridge 不受影响。"u8, 1)
            .RegisterEntry("新增选项以配置是否在登录期间的角色选择界面上应用配置文件。"u8)
            .RegisterEntry("减少信息级插件日志的冗长程度。"u8)
            .RegisterEntry("源代码维护 - 更新外部库。"u8);

	private static void Add2_0_3_0(Changelog log)
	    => log.NextVersion("版本 2.0.3.0"u8)
	        .RegisterEntry("添加了配置选项，用于确定配置文件是否影响游戏用户界面的各个部分："u8)
	        .RegisterEntry("角色窗口"u8, 1)
	        .RegisterEntry("试穿、染色预览、投影窗口"u8, 1)
	        .RegisterEntry("冒险者铭牌 (肖像)"u8, 1)
	        .RegisterEntry("调查窗口"u8, 1)
	        .RegisterEntry("添加了配置选项，用于确定模板编辑器预览角色是否在登录时自动设置为当前角色。默认情况下禁用。"u8)
	        .RegisterEntry("启用的配置文件不再能被设置为默认配置文件。"u8)
	        .RegisterEntry("修复了当前玩家角色的配置文件应用于其他角色的特殊角色 (肖像等) 的问题。"u8)
	        .RegisterEntry("修复了关闭具有活动临时配置文件的角色检查窗口时临时配置文件被移除的问题。"u8)
	        .RegisterEntry("修复了在做完Penumbra重绘后马上启用另一个配置文件时配置文件未应用的问题。"u8)
	        .RegisterEntry("修复了切换到不同配置文件时不反映在特殊角色 (肖像等) 上的问题。"u8)
	        .RegisterEntry("修复了遗留 IPC 的 `RevertCharacter` 方法泄漏异常的问题。 (2.0.2.4)"u8)
	        .RegisterEntry("源代码维护 - 外部库更新、重构、清理。"u8);

    private static void Add2_0_2_2(Changelog log)
        => log.NextVersion("版本 2.0.2.2"u8)
            .RegisterHighlight("新增用于插件间交互的全新 IPC（版本 4）。（2.0.2.0）"u8)
            .RegisterEntry("使用方法请参阅 GitHub 仓库自述文件。"u8, 1)
            .RegisterImportant("旧版 IPC（版本 3）仍可用，但将在「金曦之遗辉」上线前的某个版本中移除；建议插件开发者尽快迁移。"u8, 1)
            .RegisterEntry("已更新至 .NET 8。（2.0.2.0）"u8)
            .RegisterEntry("更新了外部库。（2.0.2.1）"u8)
            .RegisterEntry("对用户输入增加了额外清理。（2.0.2.0）"u8)
            .RegisterEntry("若默认配置文件处于启用状态，则无法再更改当前选中的默认配置。（2.0.2.1）"u8)
            .RegisterEntry("编辑器激活时无法再启用/禁用配置文件。（2.0.2.1）"u8)
            .RegisterEntry("修复了主窗口警告消息优先级不正确的问题。（2.0.2.1）"u8)
            .RegisterEntry("修复了「仅限我的从属」未正确忽略除召唤、宠物、坐骑外对象的问题。（2.0.2.1）"u8)
            .RegisterEntry("修正了多处文本。（2.0.2.1）"u8);

    private static void Add2_0_1_0(Changelog log)
        => log.NextVersion("版本 2.0.1.0"u8)
            .RegisterHighlight("新增对旧版剪贴板复制的支持。"u8)
            .RegisterEntry("新增设置项：可关闭聊天指令的确认提示。"u8)
            .RegisterEntry("在 GPose 中不再禁用模板与配置文件编辑。"u8)
            .RegisterImportant("Customize+ 与 Ktisis、Brio、Anamnesis 等造型工具并非完全兼容；部分功能可能影响 Customize+ 或使其无法正常工作。"u8, 1)
            .RegisterHighlight("修复了「任务完成」过场动画期间的崩溃。"u8)
            .RegisterEntry("修复了当某个配置文件损坏时设置迁移会整体失败的问题。"u8)
            .RegisterEntry("改进了错误处理。"u8)
            .RegisterHighlight("插件遇到严重错误时，Customize+ 窗口将显示警告信息。"u8, 1);

    private static void Add2_0_0_0(Changelog log)
        => log.NextVersion("版本 2.0.0.0"u8)
            .RegisterHighlight("对整个插件进行了大规模重写。"u8)
            .RegisterEntry("首次加载时会自动将旧版本的设置与配置文件转换为新格式。"u8, 1)
            .RegisterImportant("旧版本配置已备份以防万一；若迁移出现问题请尽快反馈。"u8, 2)
            .RegisterImportant("暂不支持来自旧版本的剪贴板复制内容。"u8, 2)
            .RegisterImportant("旧版本配置文件仅在首次加载时导入。"u8, 2)

            .RegisterHighlight("主要变更："u8)

            .RegisterEntry("插件代码几乎全部从零重写。"u8, 1)

            .RegisterEntry("用户界面已迁至 Glamourer 与 Penumbra 所用的框架，熟悉这两款插件的用户会感到似曾相识。"u8, 1)
            .RegisterEntry("因分辨率与字体大小导致的界面问题*大多*应已解决。"u8, 2)
            .RegisterImportant("在部分分辨率与字体设置下，仍有个别位置文字显示不全，后续会修复。"u8, 3)

            .RegisterEntry("新增模板系统。"u8, 1)
            .RegisterEntry("骨骼编辑现均保存在可被多个配置文件共享的模板中；单个配置文件可引用任意数量的模板。"u8, 2)

            .RegisterImportant("聊天指令已变更；可用指令请使用「/customize help」查看。"u8, 1)

            .RegisterEntry("配置文件可无任何限制地应用于召唤兽、坐骑与宠物。"u8, 1)
            .RegisterImportant("坐骑根缩放暂不可用。"u8, 2)

            .RegisterEntry("修复了「仅限持有」在多种情况下不正确的问题，并重命名为「仅限我的从属」。"u8, 1)

            .RegisterEntry("修复了因最初的 Mare Synchronos 集成实现方式导致配置文件「串」到其他角色上的问题。"u8, 1)

            .RegisterEntry("与过场动画的兼容性有所提升，但尚未充分测试。"u8, 1)

            .RegisterEntry("插件配置现会定期备份，备份位于 %appdata%\\XIVLauncher\\backups\\CustomizePlus 文件夹。"u8, 1);
}
