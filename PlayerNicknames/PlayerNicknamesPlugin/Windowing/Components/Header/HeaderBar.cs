using Dalamud.Interface;
using Dalamud.Interface.Utility;
using ImGuiNET;
using PlayerNicknames.PlayerNicknamesPlugin.Windowing.Windows;
using PlayerNicknames.PlayerNicknamesPlugin.Windowing;
using PlayerNicknames.PlayerNicknamesPlugin.Windowing.Base;
using PlayerNicknames.PlayerNicknamesPlugin.Windowing.Components;
using PlayerNicknames.PlayerNicknamesPlugin.Windowing.Windows;
using PlayerRenamer;
using System.Numerics;

namespace PlayerNicknames.PlayerNicknamesPlugin.Windowing.Components.Header;

internal static class HeaderBar
{
    const float HEADER_BAR_HEIGHT = 35;
    public static float HeaderBarWidth = 0;

    public static void Draw(in WindowHandler windowHandler, in Configuration configuration, in PlayerWindow petWindow)
    {
        Vector2 contentSize = ImGui.GetContentRegionAvail();
        contentSize.Y = HEADER_BAR_HEIGHT * ImGuiHelpers.GlobalScale;

        if (Listbox.Begin($"##headerbar_{WindowHandler.InternalCounter}", contentSize))
        {
            HeaderBarWidth = 0;

            WindowStruct<DevWindow> petDevWindow = new WindowStruct<DevWindow>(in windowHandler, in configuration, FontAwesomeIcon.Biohazard, "Pet Dev", configuration.debugModeActive);
            WindowStruct<KofiWindow> kofiWindow = new WindowStruct<KofiWindow>(in windowHandler, in configuration, FontAwesomeIcon.Coffee, "Kofi", configuration.showKofiButton && petWindow is not KofiWindow);
            WindowStruct<RenameWindow> petRenameWindow = new WindowStruct<RenameWindow>(in windowHandler, in configuration, FontAwesomeIcon.PenSquare, "Nickname Window", petWindow is not RenameWindow || configuration.quickButtonsToggle);

            float availableWidth = ImGui.GetContentRegionAvail().X;
            availableWidth -= HeaderBarWidth;

            ImGui.SetCursorPos(ImGui.GetCursorPos() + new Vector2(availableWidth, 0));

            petDevWindow.Draw();
            kofiWindow.Draw();
            petRenameWindow.Draw();

            Listbox.End();
        }
    }
}

ref struct WindowStruct<T> where T : PlayerWindow
{
    readonly WindowHandler WindowHandler;
    readonly Configuration Configuration;
    readonly FontAwesomeIcon Icon;
    readonly string Tooltip;
    readonly bool Active;

    public WindowStruct(in WindowHandler handler, in Configuration configuration, FontAwesomeIcon icon, string tooltip, bool active = true)
    {
        Active = active;

        if (Active) HeaderBar.HeaderBarWidth += WindowButton.Width;

        WindowHandler = handler;
        Configuration = configuration;
        Icon = icon;
        Tooltip = tooltip;
    }

    public void Draw() 
    {
        if (!Active) return;

        WindowButton.Draw<T>(in WindowHandler, in Configuration, Icon, Tooltip);
        ImGui.SameLine(0, 0);
    }
}
