using System;
using CustomizePlus.Core.Helpers;
using CustomizePlus.Game.Services;
using Dalamud.Plugin.Services;

namespace CustomizePlus.Core.Services;

public class TestingVersionNotifierService : IDisposable
{
    private readonly IClientState _clientState;
    private readonly ChatService _chatService;

    public TestingVersionNotifierService(IClientState clientState, ChatService chatService)
    {
        _clientState = clientState;
        _chatService = chatService;

        _clientState.Login += OnLogin;
    }

    public void Dispose()
    {
        _clientState.Login -= OnLogin;
    }

    private void OnLogin()
    {
        if (VersionHelper.IsTesting)
            _chatService.PrintInChat($"您正在运行 Customize+ 的测试版！某些功能（例如与其他插件的集成），可能无法正常工作。",
                ChatService.ChatMessageColor.Warning);
    }
}
