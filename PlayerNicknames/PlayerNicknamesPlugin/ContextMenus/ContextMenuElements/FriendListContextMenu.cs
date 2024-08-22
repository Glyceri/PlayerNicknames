using Dalamud.Game.Gui.ContextMenu;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using PlayerNicknames.PlayerNicknamesPlugin.ContextMenus.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Core;
using PlayerNicknames.PlayerNicknamesPlugin.Core.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Core.Struct;
using PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Windowing.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Windowing.Windows;
using System;

namespace PlayerNicknames.PlayerNicknamesPlugin.ContextMenus.ContextMenuElements;

internal class FriendListContextMenu : IContextMenuElement
{
    // Null means context menu didn't come from an addon
    public string? AddonName { get; } = "FriendList";

    readonly DalamudServices DalamudServices;
    readonly INameDatabase Database;
    readonly IWindowHandler WindowHandler;
    readonly IPlayerServices PlayerServices;

    public FriendListContextMenu(in DalamudServices dalamudServices, IPlayerServices playerServices, in INameDatabase database, in IWindowHandler windowHandler)
    {
        DalamudServices = dalamudServices;
        PlayerServices = playerServices;
        Database = database;
        WindowHandler = windowHandler;
    }

    public unsafe Action<IMenuItemClickedArgs>? OnOpenMenu(IMenuOpenedArgs args)
    {
        ulong contentID = AgentFriendlist.Instance()->SelectedContentId;
        if (contentID == 0) return null;

        string selectedName = SeString.Parse(AgentFriendlist.Instance()->SelectedPlayerName.StringPtr).TextValue;
        if (selectedName.IsNullOrWhitespace()) return null;

        PlayerStruct? player = PlayerServices.StringHelper.ParseStickyPlayerString(selectedName);
        if (player == null) return null;

        INameDatabaseEntry entry = Database.GetEntry(contentID);
        if (entry.Name.IsNullOrWhitespace() || entry.Homeworld == 0)
        {
            entry.UpdateEntry(player.Value.Username, player.Value.Homeworld);
        }
        return (a) => WindowHandler.GetWindow<RenameWindow>()?.SetRenameWindow(entry, true);
    }
}
