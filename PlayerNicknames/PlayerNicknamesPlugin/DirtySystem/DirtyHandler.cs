using PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.DirtySystem.Interfaces;
using System;

namespace PlayerNicknames.PlayerNicknamesPlugin.DirtySystem;

internal class DirtyHandler : IDirtyListener, IDirtyCaller
{
    Action<INameDatabase>? OnDatabase = _ => { };
    Action<INameDatabaseEntry>? OnEntry = _ => { };
    Action<INameEntry>? OnName = _ => { };

    public void DirtyDatabase(INameDatabase database)
    {
        OnDatabase?.Invoke(database);
    }

    public void DirtyEntry(INameDatabaseEntry databaseEntry)
    {
        OnEntry?.Invoke(databaseEntry);
    }

    public void DirtyName(INameEntry name)
    {
        OnName?.Invoke(name);
    }

    public void RegisterOnDirtyDatabase(Action<INameDatabase> onDatabase)
    {
        OnDatabase -= onDatabase;
        OnDatabase += onDatabase;
    }

    public void RegisterOnDirtyDatabaseEntry(Action<INameDatabaseEntry> onDatabaseEntry)
    {
        OnEntry -= onDatabaseEntry;
        OnEntry += onDatabaseEntry;
    }

    public void RegisterOnDirtyName(Action<INameEntry> onNameEntry)
    {
        OnName -= onNameEntry;
        OnName += onNameEntry;
    }

    public void UnregisterOnDirtyDatabase(Action<INameDatabase> onDatabase)
    {
        OnDatabase -= onDatabase;
    }

    public void UnregisterOnDirtyDatabaseEntry(Action<INameDatabaseEntry> onDatabaseEntry)
    {
        OnEntry -= onDatabaseEntry;
    }

    public void UnregisterOnDirtyName(Action<INameEntry> onNameEntry)
    {
        OnName -= onNameEntry;
    }
}
