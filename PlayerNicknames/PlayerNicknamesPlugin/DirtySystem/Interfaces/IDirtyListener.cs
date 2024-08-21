using PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;
using System;

namespace PlayerNicknames.PlayerNicknamesPlugin.DirtySystem.Interfaces;

internal interface IDirtyListener
{
    void RegisterOnDirtyName(Action<INameEntry> onNameEntry);
    void RegisterOnDirtyDatabaseEntry(Action<INameDatabaseEntry> onDatabaseEntry);
    void RegisterOnDirtyDatabase(Action<INameDatabase> onDatabase);

    void UnregisterOnDirtyName(Action<INameEntry> onNameEntry);
    void UnregisterOnDirtyDatabaseEntry(Action<INameDatabaseEntry> onDatabaseEntry);
    void UnregisterOnDirtyDatabase(Action<INameDatabase> onDatabase);
}
