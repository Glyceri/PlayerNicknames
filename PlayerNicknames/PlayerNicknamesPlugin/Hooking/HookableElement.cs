using PlayerNicknames.PlayerNicknamesPlugin.Hooking.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Core.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Core;
using PlayerNicknames.PlayerNicknamesPlugin.DirtySystem.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.NicknamableUsers.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;

namespace PlayerNicknames.PlayerNicknamesPlugin.Hooking;

internal abstract class HookableElement : IHookableElement
{
    public readonly DalamudServices DalamudServices;
    public readonly IUserList UserList;
    public readonly IPlayerServices PlayerServices;
    public readonly IDirtyListener DirtyListener;

    public HookableElement(DalamudServices services, IUserList userList, IPlayerServices petServices, IDirtyListener dirtyListener)
    {
        DalamudServices = services;
        UserList = userList;
        PlayerServices = petServices;
        DirtyListener = dirtyListener;

        DirtyListener.RegisterOnDirtyDatabase(OnPettableDatabaseChange);
        DirtyListener.RegisterOnDirtyDatabaseEntry(OnPettableEntryChange);
        DirtyListener.RegisterOnDirtyName(OnNameDatabaseChange);

        DalamudServices.Hooking.InitializeFromAttributes(this);
    }

    public abstract void Init();
    protected abstract void OnDispose();

    protected virtual void OnNameDatabaseChange(INameEntry nameDatabase) => Refresh();
    protected virtual void OnPettableDatabaseChange(INameDatabase pettableDatabase) => Refresh();
    protected virtual void OnPettableEntryChange(INameDatabaseEntry pettableEntry) => Refresh();
    protected virtual void Refresh() { }

    public void Dispose()
    {
        DirtyListener.UnregisterOnDirtyDatabase(OnPettableDatabaseChange);
        DirtyListener.UnregisterOnDirtyDatabaseEntry(OnPettableEntryChange);
        DirtyListener.UnregisterOnDirtyName(OnNameDatabaseChange);

        OnDispose();
    }
}
