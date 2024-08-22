using Dalamud.Interface;
using Dalamud.Interface.Utility;
using ImGuiNET;
using System.Numerics;

namespace PlayerNicknames.PlayerNicknamesPlugin.Windowing.Components.Labels;

internal static class RenameLabel
{
    public static bool Draw(string label, bool activeSave, ref string value, Vector2 size, string tooltipLabel = "",float labelWidth = 140)
    {
        ImGuiStylePtr style = ImGui.GetStyle();

        float actualWidth = labelWidth * ImGuiHelpers.GlobalScale;
        float height = size.Y;

        TextAligner.Align(TextAlignment.Left);
        BasicLabel.Draw(label, new Vector2(actualWidth, size.Y), tooltipLabel);
        TextAligner.PopAlignment();

        ImGui.SameLine();

        bool shouldActivate = false;

        ImGui.BeginDisabled(activeSave);
        ImGui.PushFont(UiBuilder.IconFont);

        shouldActivate |= ImGui.Button($"{FontAwesomeIcon.Save.ToIconString()}##saveButton_{WindowHandler.InternalCounter}", new Vector2(height, height));

        ImGui.PopFont();

        if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
        {
            ImGui.SetTooltip("Save");
        }

        ImGui.EndDisabled();

        ImGui.SameLine();

        bool keyComboNotPressed = !ImGui.IsKeyDown(ImGuiKey.LeftCtrl) || !ImGui.IsKeyDown(ImGuiKey.LeftShift);

        ImGui.BeginDisabled(keyComboNotPressed);

        ImGui.PushFont(UiBuilder.IconFont);

        if (ImGui.Button($"{FontAwesomeIcon.Eraser.ToIconString()}##clearButton_{WindowHandler.InternalCounter}", new Vector2(height, height)))
        {
            value = string.Empty;
            shouldActivate |= true;
        }
        ImGui.PopFont();
        ImGui.EndDisabled();

        if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
        {
            if (keyComboNotPressed)
            {
                ImGui.SetTooltip("Hold \"Left Ctrl\" + \"Left Shift\" to delete an entry.");
            }
            else
            {
                ImGui.SetTooltip("Clear");
            }
        }

        ImGui.SameLine();

        shouldActivate |= ImGui.InputTextMultiline($"##RenameBar_{WindowHandler.InternalCounter}", ref value, PluginConstants.FFXIV_MAX_PLAYERNAME_SIZE, size - new Vector2(actualWidth + style.ItemSpacing.X * 3 + height * 2, 0), ImGuiInputTextFlags.CtrlEnterForNewLine | ImGuiInputTextFlags.EnterReturnsTrue);

        return shouldActivate;
    }
}
