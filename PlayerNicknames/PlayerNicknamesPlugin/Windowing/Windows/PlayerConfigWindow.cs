using ImGuiNET;
using PlayerNicknames.PlayerNicknamesPlugin.Core;
using PlayerNicknames.PlayerNicknamesPlugin.Windowing.Base;
using PlayerRenamer;
using System.Numerics;

namespace PlayerNicknames.PlayerNicknamesPlugin.Windowing.Windows;

internal class PlayerConfigWindow : PlayerWindow
{
    protected override Vector2 MinSize { get; } = new Vector2(400, 200);
    protected override Vector2 MaxSize { get; } = new Vector2(400, 1200);
    protected override Vector2 DefaultSize { get; } = new Vector2(400, 500);
    protected override bool HasTopBar { get; } = true;

    public PlayerConfigWindow(in WindowHandler windowHandler, in DalamudServices dalamudServices, in Configuration configuration) : base(windowHandler, dalamudServices, configuration, "Player Settings", ImGuiWindowFlags.None) { }

    protected override void OnDraw()
    {
        if (ImGui.CollapsingHeader("General Settings"))
        {
            if (ImGui.Checkbox("Automatically download profile pictures.", ref Configuration.downloadProfilePictures)) Configuration.Save();
        }

        if (ImGui.CollapsingHeader("UI Settings"))
        {
            if (ImGui.Checkbox("Show Ko-fi Button.", ref Configuration.showKofiButton)) Configuration.Save();
            if (ImGui.Checkbox("Quick Buttons toggle instead of open.", ref Configuration.quickButtonsToggle)) Configuration.Save();
        }

        if (ImGui.CollapsingHeader("Native Settings"))
        {
            if (ImGui.Checkbox("Allow Context Menus.", ref Configuration.useContextMenus)) Configuration.Save();
        }

        if (ImGui.CollapsingHeader("DEBUG"))
        {
            bool keyComboPressed = ImGui.IsKeyDown(ImGuiKey.LeftCtrl) && ImGui.IsKeyDown(ImGuiKey.LeftShift);

            ImGui.BeginDisabled(!keyComboPressed && !Configuration.debugModeActive);
            if (ImGui.Checkbox("Enable Debug Mode.", ref Configuration.debugModeActive)) Configuration.Save();
            if (ImGui.Checkbox("Open Debug Window On Start.", ref Configuration.openDebugWindowOnStart)) Configuration.Save();
            ImGui.EndDisabled();
        }
    }
}
