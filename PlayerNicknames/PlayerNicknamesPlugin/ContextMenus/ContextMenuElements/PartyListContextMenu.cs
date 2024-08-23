using Dalamud.Game.Gui.ContextMenu;
using Dalamud.Utility;
using PlayerNicknames.PlayerNicknamesPlugin.ContextMenus.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Core.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Core.Struct;
using PlayerNicknames.PlayerNicknamesPlugin.Core;
using PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Windowing.Interfaces;
using System;
using PlayerNicknames.PlayerNicknamesPlugin.Windowing.Windows;
using PlayerNicknames.PlayerNicknamesPlugin.Hooking.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.NicknamableUsers.Interfaces;

namespace PlayerNicknames.PlayerNicknamesPlugin.ContextMenus.ContextMenuElements;

internal class PartyListContextMenu : IContextMenuElement
{
    // Null means context menu didn't come from an addon
    public virtual string? AddonName { get; } = "_PartyList";

    readonly DalamudServices DalamudServices;
    readonly INameDatabase Database;
    readonly IWindowHandler WindowHandler;
    readonly IPlayerServices PlayerServices;
    readonly IPartyHook PartyHook;
    readonly IUserList UserList;

    public PartyListContextMenu(in DalamudServices dalamudServices, IPlayerServices playerServices, in INameDatabase database, in IWindowHandler windowHandler, IPartyHook partyHook, IUserList userList)
    {
        DalamudServices = dalamudServices;
        PlayerServices = playerServices;
        Database = database;
        WindowHandler = windowHandler;
        PartyHook = partyHook;
        UserList = userList;
    }

    public unsafe Action<IMenuItemClickedArgs>? OnOpenMenu(IMenuOpenedArgs args)
    {
        INamableUser? localUser = UserList.LocalPlayer;
        if (localUser == null) return null;

        ulong contentID = PartyHook.HoveredContentID;
        if (contentID == 0) return null;
        if (contentID == localUser.ContentID) return null;

        INameDatabaseEntry entry = Database.GetEntry(contentID);
        if (entry.Name.IsNullOrWhitespace() || entry.Homeworld == 0)
        {
            entry.UpdateEntry(PartyHook.HoveredName, PartyHook.HoveredHomeworld);
        }
        return (a) => WindowHandler.GetWindow<RenameWindow>()?.SetRenameWindow(entry, true);
    }
}

