using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using PlayerNicknames.PlayerNicknamesPlugin.Windowing.Components.Header;
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
    protected abstract bool HasTopBar { get; }

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

    readonly Vector2 windowPadding = new(8, 8);
    readonly Vector2 framePadding = new(4, 3);
    readonly Vector2 itemInnerSpacing = new(4, 4);
    readonly Vector2 itemSpacing = new(4, 4);

    public sealed override void PreDraw()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, windowPadding * ImGuiHelpers.GlobalScale);
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, framePadding * ImGuiHelpers.GlobalScale);
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, itemSpacing * ImGuiHelpers.GlobalScale);
        ImGui.PushStyleVar(ImGuiStyleVar.ItemInnerSpacing, itemInnerSpacing * ImGuiHelpers.GlobalScale);

        OnEarlyDraw();
    }

    public sealed override void PostDraw()
    {
        OnLateDraw();
        ImGui.PopStyleVar(4);
    }

    public sealed override void Draw()
    {
        if (HasTopBar) HeaderBar.Draw(in WindowHandler, in Configuration, this);
        OnDraw();
    }

    public void Dispose() => OnDispose();
    public void NotifyDirty() => OnDirty();

    protected virtual void OnEarlyDraw() { }
    protected virtual void OnDraw() { }
    protected virtual void OnLateDraw() { }
    protected virtual void OnDispose() { }
    protected virtual void OnDirty() { }
}
