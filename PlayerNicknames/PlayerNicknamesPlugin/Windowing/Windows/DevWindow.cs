using ImGuiNET;
using PlayerNicknames.PlayerNicknamesPlugin.Core;
using PlayerNicknames.PlayerNicknamesPlugin.NicknamableUsers.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Windowing.Base;
using PlayerRenamer;
using System.Numerics;

namespace PlayerNicknames.PlayerNicknamesPlugin.Windowing.Windows;

internal class DevWindow : PlayerWindow
{
    protected override Vector2 MinSize { get; } = new Vector2(350, 136);
    protected override Vector2 MaxSize { get; } = new Vector2(2000, 2000);
    protected override Vector2 DefaultSize { get; } = new Vector2(800, 400);
    protected override bool HasTopBar { get; } = true;

    readonly IUserList UserList;

    public DevWindow(WindowHandler windowHandler, DalamudServices dalamudServices, Configuration configuration, IUserList userList) : base(windowHandler, dalamudServices, configuration, "Developer Tools", ImGuiWindowFlags.None, true)
    {
        UserList = userList;

        if (configuration.debugModeActive && configuration.openDebugWindowOnStart)
        {
            Open();
        }
    }

    protected override void OnDraw()
    {
        DrawUserList();
    }

    void DrawUserList()
    {
        foreach (INamableUser? user in UserList.NamableUsers)
        {
            if (user == null) continue;
            NewDrawUser(user);
        }
    }

    void NewDrawUser(INamableUser user)
    {
        if (!ImGui.BeginTable($"##usersTable{WindowHandler.InternalCounter}", 6, ImGuiTableFlags.RowBg | ImGuiTableFlags.Borders | ImGuiTableFlags.Resizable | ImGuiTableFlags.SizingMask))
            return;

        ImGui.TableNextRow();

        ImGui.TableSetColumnIndex(0);
        ImGui.TextUnformatted($"{user.Name}");

        ImGui.TableSetColumnIndex(1);
        ImGui.TextUnformatted(user.DatabaseEntry.HomeworldName);

        ImGui.TableSetColumnIndex(2);
        ImGui.TextUnformatted(user.IsActive ? "O" : "X");

        ImGui.TableSetColumnIndex(3);
        ImGui.TextUnformatted(user.DatabaseEntry.ActiveEntry.GetName() ?? "...");


        ImGui.TableSetColumnIndex(4);
        if (ImGui.Button($"+##{WindowHandler.InternalCounter}"))
        {
            user.DatabaseEntry.ActiveEntry.SetName("TEST NAME!");
        }

        ImGui.TableSetColumnIndex(5);
        if (ImGui.Button($"X##{WindowHandler.InternalCounter}"))
        {
            user.DatabaseEntry.ActiveEntry.SetName(null);
        }


        ImGui.EndTable();
    }
}
