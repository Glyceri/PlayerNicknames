using PlayerNicknames.PlayerNicknamesPlugin.Hooking.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Core.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Core;
using PlayerNicknames.PlayerNicknamesPlugin.DirtySystem.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.NicknamableUsers.Interfaces;
using System;
using System.Collections.Generic;
using PlayerNicknames.PlayerNicknamesPlugin.Hooking.HookTypes;
using PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;

namespace PlayerNicknames.PlayerNicknamesPlugin.Hooking;

internal abstract class QuickHookableElement : HookableElement
{
    public QuickHookableElement(DalamudServices services, IUserList userList, IPlayerServices petServices, IDirtyListener dirtyListener) : base(services, userList, petServices, dirtyListener) { }

    readonly List<ITextHook> textHooks = new List<ITextHook>();

    public T Hook<T>(string addonName, uint[] textPos, Func<INameDatabaseEntry, bool> allowedCallback) where T : ITextHook, new()
    {
        T t = new T();
        t.Setup(DalamudServices, UserList, PlayerServices, DirtyListener, addonName, textPos, allowedCallback);
        // Cant use the [t is SimpleTextHook tHook] because it can only run this code if it is of the ACTUAL type SimpleTextHook.
        // Not any inherited type
        if (t.GetType() == typeof(SimpleTextHook))
        {
            (t as SimpleTextHook)!.SetUnfaulty();
        }
        textHooks.Add(t);
        return t;
    }

    protected sealed override void OnDispose()
    {
        OnQuickDispose();

        foreach(ITextHook hook in textHooks)
        {
            hook.Dispose();
        }    

        textHooks.Clear();
    }

    protected virtual void OnQuickDispose() { }
}
