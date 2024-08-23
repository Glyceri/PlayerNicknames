using PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.DirtySystem.Interfaces;
using System;
using System.Runtime.CompilerServices;

namespace PlayerNicknames.PlayerNicknamesPlugin.DirtySystem;

internal class DirtyHandler : IDirtyListener, IDirtyCaller
{
    Action<INameDatabase>? OnDatabase = _ => { };
    Action<INameDatabaseEntry>? OnEntry = _ => { };
    Action<INameEntry>? OnName = _ => { };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DirtyDatabase(INameDatabase database)
    {
        OnDatabase?.Invoke(database);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DirtyEntry(INameDatabaseEntry databaseEntry)
    {
        OnEntry?.Invoke(databaseEntry);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DirtyName(INameEntry name)
    {
        OnName?.Invoke(name);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RegisterOnDirtyDatabase(Action<INameDatabase> onDatabase)
    {
        OnDatabase -= onDatabase;
        OnDatabase += onDatabase;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RegisterOnDirtyDatabaseEntry(Action<INameDatabaseEntry> onDatabaseEntry)
    {
        OnEntry -= onDatabaseEntry;
        OnEntry += onDatabaseEntry;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RegisterOnDirtyName(Action<INameEntry> onNameEntry)
    {
        OnName -= onNameEntry;
        OnName += onNameEntry;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UnregisterOnDirtyDatabase(Action<INameDatabase> onDatabase)
    {
        OnDatabase -= onDatabase;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UnregisterOnDirtyDatabaseEntry(Action<INameDatabaseEntry> onDatabaseEntry)
    {
        OnEntry -= onDatabaseEntry;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UnregisterOnDirtyName(Action<INameEntry> onNameEntry)
    {
        OnName -= onNameEntry;
    }
}
