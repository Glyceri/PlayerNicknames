using Dalamud.Interface.Utility;
using Dalamud.Interface;
using ImGuiNET;
using PlayerNicknames.PlayerNicknamesPlugin.Core;
using PlayerNicknames.PlayerNicknamesPlugin.Core.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.ImageDatabase.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.NicknamableUsers.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Windowing.Base;
using PlayerNicknames.PlayerNicknamesPlugin.Windowing.Components;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Utility;
using System;
using PlayerNicknames.PlayerNicknamesPlugin.Windowing.Components.Labels;
using PlayerNicknames.PlayerNicknamesPlugin.Windowing.Components.Image;
using PlayerNicknames.PlayerNicknamesPlugin.Windowing.Windows.PlayerList;

namespace PlayerNicknames.PlayerNicknamesPlugin.Windowing.Windows;

internal class PlayerListWindow : PlayerWindow
{
    readonly IUserList UserList;
    readonly INameDatabase Database;
    readonly IPlayerServices PlayerServices;
    readonly IImageDatabase ImageDatabase;

    protected override Vector2 MinSize { get; } = new Vector2(400, 250);
    protected override Vector2 MaxSize { get; } = new Vector2(1600, 1500);
    protected override Vector2 DefaultSize { get; } = new Vector2(800, 500);
    protected override bool HasTopBar { get; } = true;

    string SearchText = string.Empty;
    string activeSearchText = string.Empty;

    readonly List<PlayerListPlayer> activeUsers = new List<PlayerListPlayer>();

    float BarHeight => 30 * ImGuiHelpers.GlobalScaleSafe;

    public PlayerListWindow(in WindowHandler windowHandler, in DalamudServices dalamudServices, in IPlayerServices playerServices, in IUserList userList, in INameDatabase database, in IImageDatabase imageDatabase) : base(windowHandler, dalamudServices, playerServices.Configuration, "Player List", ImGuiWindowFlags.None)
    {
        UserList = userList;
        Database = database;
        PlayerServices = playerServices;
        ImageDatabase = imageDatabase;
    }

    public override void OnOpen()
    {
        ClearSearchBar();
        Setup();
    }

    protected override void OnDirty()
    {
        ClearSearchBar();
        Setup();
    }

    void Setup()
    {
        activeUsers.Clear();
        foreach (INameDatabaseEntry entry in Database.Entries)
        {
            if (!entry.IsActive) continue;
            if (entry.ActiveEntry.GetName() == null) continue;

            if (!(Valid(entry.Name) || Valid(entry.HomeworldName) || Valid(entry.ContentID.ToString()))) continue;

            activeUsers.Add(new PlayerListPlayer(entry));
        }
    }

    void ClearSearchBar()
    {
        SearchText = string.Empty;
        activeSearchText = string.Empty;
    }

    void DrawSearchbar()
    {
        if (Listbox.Begin($"##Listbox_{WindowHandler.InternalCounter}", new Vector2(ImGui.GetContentRegionAvail().X, 30 * ImGuiHelpers.GlobalScale)))
        {
            ImGuiStylePtr style = ImGui.GetStyle();
            float buttSize = ImGui.GetContentRegionAvail().Y;

            bool clicked = false;

            if (ImGui.InputTextMultiline($"##InputText_{WindowHandler.InternalCounter}", ref SearchText, 64, new Vector2(ImGui.GetContentRegionAvail().X - buttSize - style.FramePadding.X, buttSize), ImGuiInputTextFlags.CtrlEnterForNewLine | ImGuiInputTextFlags.EnterReturnsTrue))
            {
                clicked |= true;
            }

            SearchText = SearchText.Replace("\n", string.Empty);

            ImGui.SameLine();

            ImGui.PushFont(UiBuilder.IconFont);

            if (ImGui.Button($"{FontAwesomeIcon.Search.ToIconString()}##Search_{WindowHandler.InternalCounter}", new Vector2(buttSize, buttSize)))
            {
                clicked |= true;
            }

            ImGui.PopFont();

            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("Search");
            }

            if (clicked)
            {
                activeSearchText = SearchText;
                Setup();
            }

            Listbox.End();
        }
    }

    public bool Valid(string input)
    {
        if (activeSearchText.IsNullOrWhitespace()) return true;

        return input.Contains(activeSearchText, StringComparison.InvariantCultureIgnoreCase);
    }

    protected override void OnDraw()
    {
        DrawSearchbar();
        DrawList();
    }

    void DrawList()
    {
        if (Listbox.Begin("##userlistListbox", ImGui.GetContentRegionAvail()))
        {
            foreach (PlayerListPlayer entry in activeUsers)
            {
                if (Listbox.Begin($"##Listbox_{WindowHandler.InternalCounter}", new Vector2(ImGui.GetContentRegionAvail().X, 110 * ImGuiHelpers.GlobalScale)))
                {
                    float size = ImGui.GetContentRegionAvail().Y;

                    PlayerImage.Draw(entry.Entry, ImageDatabase);

                    ImGui.SameLine();

                    if (Listbox.Begin($"##Listbox_{WindowHandler.InternalCounter}", ImGui.GetContentRegionAvail()))
                    {
                        Vector2 contentSpot = new Vector2(ImGui.GetContentRegionAvail().X, BarHeight);

                        if (RenameLabel.Draw($"Nickname:", entry.CustomName == entry.TempName, ref entry.TempName, contentSpot))
                        {
                            OnSave(entry.TempName, entry.Entry);
                        }
                        LabledLabel.Draw("Homeworld:", entry.Entry.HomeworldName ?? "...", contentSpot);
                        LabledLabel.Draw("ID:", entry.Entry.ContentID.ToString() ?? "...", contentSpot);
                        Listbox.End();
                    }
                    Listbox.End();
                }
            }
            Listbox.End();
        }
    }

    void OnSave(string? newName, INameDatabaseEntry entry) => DalamudServices.Framework.Run(() => entry.ActiveEntry.SetName(newName));
}
