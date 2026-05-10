using CustomizePlus.Configuration.Data;

namespace CustomizePlus.UI.Windows.MainWindow.Tabs.Templates;

public sealed class TemplateFilter : TokenizedFilter<TemplateFilterTokenType, TemplateFileSystemCache.TemplateData, TemplateFilterToken>,
    IFileSystemFilter<TemplateFileSystemCache.TemplateData>, IUiService
{
    public TemplateFilter(PluginConfiguration configuration)
    {
        //todo
        /*if (config.RememberDesignFilter)
            Set(config.Filters.DesignFilter);
        FilterChanged += () => config.Filters.DesignFilter = Text;*/
    }

    protected override void DrawTooltip()
    {
        if (!Im.Item.Hovered())
            return;

        using var tt = Im.Tooltip.Begin();
        var highlightColor = ColorId.EnabledProfile.Value().ToVector();
        Im.Text("按空格分隔多个字符串，筛选完整路径或名称中包含这些字符串的模板。"u8);
        ImEx.TextMultiColored("输入 "u8).Then("n:[string]"u8, highlightColor).Then(" 可仅按模板名称筛选，忽略路径。"u8)
            .End();
        ImEx.TextMultiColored("输入 "u8).Then("f:[string]"u8, highlightColor).Then(
                " 可筛选名称或路径中包含该文字的模板。"u8)
            .End();
        Im.Line.New();
        ImEx.TextMultiColored("使用 "u8).Then("None"u8, highlightColor).Then(" 作为占位符，仅匹配空列表或空名称。"u8)
            .End();
        Im.Text("通常情况下，模板必须分别满足所有给定条件。"u8);
        ImEx.TextMultiColored("在搜索词前加 "u8).Then("'-'"u8, highlightColor)
            .Then("，仅查找不满足该条件的模板。"u8).End();
        ImEx.TextMultiColored("在搜索词前加 "u8).Then("'?'"u8, highlightColor)
            .Then("，查找至少满足任一“?”条件的模板。"u8).End();
        ImEx.TextMultiColored("用 "u8).Then("\"[string with space]\""u8, highlightColor)
            .Then(" 包裹含空格的内容，以匹配这组词的精确组合。"u8).End();
    }

    protected override bool Matches(in TemplateFilterToken token, in TemplateFileSystemCache.TemplateData cacheItem)
        => token.Type switch
        {
            TemplateFilterTokenType.Default => cacheItem.Node.FullPath.Contains(token.Needle, StringComparison.OrdinalIgnoreCase)
             || cacheItem.Node.Value.Name.Contains(token.Needle, StringComparison.OrdinalIgnoreCase),
            TemplateFilterTokenType.Name => cacheItem.Node.Value.Name.Contains(token.Needle, StringComparison.OrdinalIgnoreCase),
            TemplateFilterTokenType.FullContext => CheckFullContext(token.Needle, cacheItem),
            _ => true,
        };

    protected override bool MatchesNone(TemplateFilterTokenType type, bool negated, in TemplateFileSystemCache.TemplateData cacheItem)
        => true;

    private static bool CheckFullContext(string needle, in TemplateFileSystemCache.TemplateData cacheItem)
    {
        if (needle.Length is 0)
            return true;

        if (cacheItem.Node.FullPath.Contains(needle, StringComparison.OrdinalIgnoreCase))
            return true;

        var template = cacheItem.Node.Value;
        if (template.Name.Contains(needle, StringComparison.OrdinalIgnoreCase))
            return true;

        if (template.UniqueId.ToString().Contains(needle, StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    }

    public bool WouldBeVisible(in FileSystemFolderCache folder)
    {
        switch (State)
        {
            case FilterState.NoFilters: return true;
            case FilterState.NoMatches: return false;
        }

        foreach (var token in Forced)
        {
            if (token.Type switch
            {
                TemplateFilterTokenType.Name => !folder.Name.Contains(token.Needle, StringComparison.OrdinalIgnoreCase),
                TemplateFilterTokenType.Default => !folder.FullPath.Contains(token.Needle, StringComparison.OrdinalIgnoreCase),
                TemplateFilterTokenType.FullContext => !folder.FullPath.Contains(token.Needle, StringComparison.OrdinalIgnoreCase),
                _ => true,
            })
                return false;
        }

        foreach (var token in Negated)
        {
            if (token.Type switch
            {
                TemplateFilterTokenType.Name => folder.Name.Contains(token.Needle, StringComparison.OrdinalIgnoreCase),
                TemplateFilterTokenType.Default => folder.FullPath.Contains(token.Needle, StringComparison.OrdinalIgnoreCase),
                TemplateFilterTokenType.FullContext => folder.FullPath.Contains(token.Needle, StringComparison.OrdinalIgnoreCase),
                _ => false,
            })
                return false;
        }

        foreach (var token in General)
        {
            if (token.Type switch
            {
                TemplateFilterTokenType.Name => folder.Name.Contains(token.Needle, StringComparison.OrdinalIgnoreCase),
                TemplateFilterTokenType.Default => folder.FullPath.Contains(token.Needle, StringComparison.OrdinalIgnoreCase),
                TemplateFilterTokenType.FullContext => !folder.FullPath.Contains(token.Needle, StringComparison.OrdinalIgnoreCase),
                _ => false,
            })
                return true;
        }

        return General.Count is 0;
    }
}