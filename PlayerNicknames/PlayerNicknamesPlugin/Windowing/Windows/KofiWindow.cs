using Dalamud.Interface.Utility;
using Dalamud.Utility;
using ImGuiNET;
using PlayerNicknames.PlayerNicknamesPlugin.Core;
using PlayerNicknames.PlayerNicknamesPlugin.Windowing;
using PlayerNicknames.PlayerNicknamesPlugin.Windowing.Base;
using PlayerNicknames.PlayerNicknamesPlugin.Windowing.Components.Labels;
using PlayerRenamer;
using System.Numerics;

namespace PlayerNicknames.PlayerNicknamesPlugin.Windowing.Windows;

internal class KofiWindow : PlayerWindow
{
    protected override Vector2 MinSize { get; } = new Vector2(350, 136);
    protected override Vector2 MaxSize { get; } = new Vector2(350, 136);
    protected override Vector2 DefaultSize { get; } = new Vector2(350, 136);
    protected override bool HasTopBar { get; } = false;

    float BarSize = 30 * ImGuiHelpers.GlobalScale;

    public KofiWindow(in WindowHandler windowHandler, in DalamudServices dalamudServices, in Configuration configuration) : base(windowHandler, dalamudServices, configuration, "Kofi-Window", ImGuiWindowFlags.None)
    {
        
    }

    protected override void OnDraw()
    {
        BasicLabel.Draw("This is about real life money.", new Vector2(ImGui.GetContentRegionAvail().X, BarSize));
        BasicLabel.Draw("It will be used to my cat toys!", new Vector2(ImGui.GetContentRegionAvail().X, BarSize));

        float width = 100 * ImGuiHelpers.GlobalScale;

        ImGui.SetCursorPos(ImGui.GetCursorPos() + new Vector2(ImGui.GetContentRegionAvail().X * 0.5f - width * 0.5f, 0));

        if (ImGui.Button("Take Me" + "##Kofi_{WindowHandler.InternalCounter}", new Vector2(width, BarSize)))
        {
            Util.OpenLink("https://ko-fi.com/glyceri");
        }
    }
}
