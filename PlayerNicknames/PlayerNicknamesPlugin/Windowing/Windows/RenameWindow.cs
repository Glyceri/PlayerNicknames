using Dalamud.Interface.Utility;
using ImGuiNET;
using PlayerNicknames.PlayerNicknamesPlugin.Core;
using PlayerNicknames.PlayerNicknamesPlugin.Core.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.ImageDatabase.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.NicknamableUsers.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Windowing.Base;
using PlayerNicknames.PlayerNicknamesPlugin.Windowing.Components;
using PlayerNicknames.PlayerNicknamesPlugin.Windowing.Components.Image;
using PlayerNicknames.PlayerNicknamesPlugin.Windowing.Components.Labels;
using System.Numerics;

namespace PlayerNicknames.PlayerNicknamesPlugin.Windowing.Windows;

internal class RenameWindow : PlayerWindow
{
    readonly IUserList UserList;
    readonly IPlayerServices PlayerServices;
    readonly IImageDatabase ImageDatabase;

    protected override Vector2 MinSize { get; } = new Vector2(437, 216);
    protected override Vector2 MaxSize { get; } = new Vector2(1500, 216);
    protected override Vector2 DefaultSize { get; } = new Vector2(437, 216);
    protected override bool HasTopBar { get; } = true;

    INameDatabaseEntry? ActiveEntry;
    string ActiveCustomName = string.Empty;
    string ActualCustomName = string.Empty;

    float BarHeight => 30 * ImGuiHelpers.GlobalScaleSafe;

    public RenameWindow(in WindowHandler windowHandler, in DalamudServices dalamudServices, IPlayerServices playerServices, IUserList userList, IImageDatabase imageDatabase) : base(windowHandler, dalamudServices, playerServices.Configuration, "Player Rename Window", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        UserList = userList;
        PlayerServices = playerServices;
        ImageDatabase = imageDatabase;
    }

    public void SetRenameWindow(INameDatabaseEntry? entry, bool open = false)
    {
        if (open)
        {
            Open();
        }

        Setup(entry);
    }

    protected override void OnDirty()
    {
        Setup(ActiveEntry);
    }

    void Setup(INameDatabaseEntry? newEntry)
    {
        ActiveEntry = newEntry;
        ActiveCustomName = newEntry?.ActiveEntry.GetName() ?? string.Empty;
        ActualCustomName = ActiveCustomName;
    }

    protected override void OnDraw()
    {
        DrawElement();        
    }

    void DrawElement()
    {
        ImGuiStylePtr stylePtr = ImGui.GetStyle();
        float framePaddingX = stylePtr.ItemSpacing.X;

        Vector2 region = ImGui.GetContentRegionAvail();
        float regionHeight = region.Y;

        if (Listbox.Begin("##RenameHolder", ImGui.GetContentRegionAvail() - new Vector2(regionHeight + framePaddingX, 0)))
        {
            DrawInternals();
            Listbox.End();
        }

        ImGui.SameLine();

        DrawImageInternals(regionHeight);
    }

    void DrawInternals()
    {
        if (ActiveEntry == null)
        {
            CenteredLabel.Draw("Please right click a user to give them a nickname.", new Vector2(ImGui.GetContentRegionAvail().X * 0.8f, BarHeight));
        }
        else
        {
            DrawUserData();
        }
    }

    void DrawUserData()
    {
        Vector2 contentSpot = new Vector2(ImGui.GetContentRegionAvail().X, BarHeight);

        LabledLabel.Draw("Player Name:", ActiveEntry?.Name ?? "...", contentSpot);
        LabledLabel.Draw("Homeworld:", ActiveEntry?.HomeworldName ?? "...", contentSpot);
        LabledLabel.Draw("ID:", ActiveEntry?.ContentID.ToString() ?? "...", contentSpot);

        if (RenameLabel.Draw($"Nickname:", ActiveCustomName == ActualCustomName, ref ActiveCustomName, contentSpot))
        {
            OnSave(ActiveCustomName);
        }

        ActiveCustomName = ActiveCustomName.Replace("\n", string.Empty);
    }

    void OnSave(string? newName)
    {
        ActiveEntry?.ActiveEntry.SetName(newName);
    }

    void DrawImageInternals(float regionHeight)
    {
        ImGuiStylePtr stylePtr = ImGui.GetStyle();
        float framePaddingX = stylePtr.FramePadding.X;
        float framePaddingY = stylePtr.FramePadding.Y;

        Vector2 size = new Vector2(regionHeight, regionHeight);


        if (Listbox.Begin("##image", size))
        {
            PlayerImage.Draw(ActiveEntry, ImageDatabase);

            Listbox.End();
        }
    }
}
