using Dalamud.Interface.Windowing;
using ImGuiNET;
using PlayerNicknames.PlayerNicknamesPlugin.Core;
using PlayerNicknames.PlayerNicknamesPlugin.Windowing.Interfaces;
using PlayerRenamer;
using System.Numerics;

namespace PlayerNicknames.PlayerNicknamesPlugin.Windowing.Base;

internal abstract class PlayerWindow : Window, IPWindow
{
    protected abstract Vector2 MinSize { get; }
    protected abstract Vector2 MaxSize { get; }
    protected abstract Vector2 DefaultSize { get; }

    protected readonly DalamudServices DalamudServices;
    protected readonly WindowHandler WindowHandler;
    protected readonly Configuration Configuration;

    protected PlayerWindow(WindowHandler windowHandler, DalamudServices dalamudServices, Configuration configuration, string name, ImGuiWindowFlags flags = ImGuiWindowFlags.None, bool forceMainWindow = false) : base(name, flags, forceMainWindow)
    {
        WindowHandler = windowHandler;
        Configuration = configuration;
        DalamudServices = dalamudServices;

        SizeCondition = ImGuiCond.FirstUseEver;
        Size = DefaultSize;

        SizeConstraints = new WindowSizeConstraints()
        {
            MinimumSize = MinSize,
            MaximumSize = MaxSize,
        };
    }

    public void Close() => IsOpen = false;
    public void Open() => IsOpen = true;
    public void Dispose() => OnDispose();
    public void NotifyDirty() => OnDirty();

    protected virtual void OnDispose() { }
    protected virtual void OnDirty() { }
}
