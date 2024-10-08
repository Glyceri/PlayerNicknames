﻿using Dalamud.Game.Gui.ContextMenu;
using Dalamud.Game.Text;
using PlayerNicknames.PlayerNicknamesPlugin.ContextMenus.ContextMenuElements;
using PlayerNicknames.PlayerNicknamesPlugin.ContextMenus.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Core;
using PlayerNicknames.PlayerNicknamesPlugin.Core.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Hooking;
using PlayerNicknames.PlayerNicknamesPlugin.NicknamableUsers.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Windowing.Interfaces;
using System;
using System.Collections.Generic;

namespace PlayerNicknames.PlayerNicknamesPlugin.ContextMenus;

internal class ContextMenuHandler : IDisposable
{
    readonly DalamudServices DalamudServices;
    readonly IPlayerServices PlayerServices;
    readonly IUserList UserList;
    readonly IWindowHandler WindowHandler;
    readonly INameDatabase Database;
    readonly HookHandler HookHandler;

    readonly List<IContextMenuElement> ContextMenuElements = new List<IContextMenuElement>();

    public ContextMenuHandler(in DalamudServices dalamudServices, in IPlayerServices playerServices, in IUserList userList, in IWindowHandler windowHandler, in HookHandler hookHandler, INameDatabase database)
    {
        DalamudServices = dalamudServices;
        WindowHandler = windowHandler;
        PlayerServices = playerServices;
        UserList = userList;
        Database = database;
        HookHandler = hookHandler;

        DalamudServices.ContextMenu.OnMenuOpened += OnOpenMenu;

        _Register();
    }

    void _Register()
    {
        Register(new TargetContextMenu(in DalamudServices, in UserList, in WindowHandler));
        Register(new PartyListContextMenu(in DalamudServices, PlayerServices, in Database, WindowHandler, HookHandler.PartyHook, UserList));
        Register(new FriendListContextMenu(in DalamudServices, PlayerServices, in Database, WindowHandler));
    }

    void Register(IContextMenuElement contextMenuElement)
    {
        ContextMenuElements.Add(contextMenuElement);
    }

    void OnOpenMenu(IMenuOpenedArgs args)
    {
        if (!PlayerServices.Configuration.useContextMenus) return;

        PlayerServices.PetLog.LogVerbose($"Context menu for: {args.AddonName}");

        foreach (IContextMenuElement contextMenuElement in ContextMenuElements)
        {
            if (contextMenuElement.AddonName != args.AddonName) continue;

            Action<IMenuItemClickedArgs>? callback = contextMenuElement.OnOpenMenu(args);
            if (callback == null) continue;

            RegisterCallback(args, callback);
        }
    }

    void RegisterCallback(IMenuOpenedArgs args, Action<IMenuItemClickedArgs> callback)
    {
        args.AddMenuItem(new MenuItem()
        {
            Name = "Give Nickname",
            Prefix = SeIconChar.BoxedLetterP,
            PrefixColor = 3,
            OnClicked = callback
        });
    }

    public void Dispose()
    {
        DalamudServices.ContextMenu.OnMenuOpened -= OnOpenMenu;
    }
}
