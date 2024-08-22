﻿using ImGuiNET;
using System.Numerics;

namespace PlayerNicknames.PlayerNicknamesPlugin.Windowing.Components;

internal static class Listbox
{
    public static bool Begin(string label, Vector2 size)
    {
        return ImGui.BeginListBox(label, size);
    }

    public static void End()
    {
        ImGui.EndListBox();
    }
}