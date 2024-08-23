using PlayerNicknames.PlayerNicknamesPlugin.Hooking.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Core.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Core;
using PlayerNicknames.PlayerNicknamesPlugin.DirtySystem.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.NicknamableUsers.Interfaces;
using System;
using System.Collections.Generic;
using PlayerNicknames.PlayerNicknamesPlugin.Hooking.HookElements;
using PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;

namespace PlayerNicknames.PlayerNicknamesPlugin.Hooking;
internal class HookHandler : IDisposable
{
    readonly DalamudServices DalamudServices;
    readonly IPlayerServices PlayerServices;
    readonly IUserList UserList;
    readonly IDirtyListener DirtyListener;
    readonly INameDatabase Database;

    public HookHandler(DalamudServices services, IUserList userList, IPlayerServices playerServices, IDirtyListener dirtyListener, INameDatabase database)
    {
        DalamudServices = services;
        PlayerServices = playerServices;
        UserList = userList;
        DirtyListener = dirtyListener;
        Database = database;

        _Register();
    }

    void _Register()
    {
        Register(new PartyHook(DalamudServices, UserList, PlayerServices, DirtyListener, Database));
        Register(new NamePlateHook(DalamudServices, PlayerServices, UserList, DirtyListener));
    }

    readonly List<IHookableElement> hookableElements = new List<IHookableElement>();

    void Register(IHookableElement element)
    {
        hookableElements.Add(element);
        element?.Init();
    }

    public void Dispose()
    {
        foreach(IHookableElement hookableElement in hookableElements)
            hookableElement.Dispose();
    }
}
