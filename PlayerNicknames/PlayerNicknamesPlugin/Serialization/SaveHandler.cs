using PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.DirtySystem.Interfaces;
using SamplePlugin;
using System;

namespace PlayerNicknames.PlayerNicknamesPlugin.Serialization;

internal class SaveHandler : IDisposable
{
    readonly Configuration Configuration;
    readonly IDirtyListener DirtyListener;

    public SaveHandler(Configuration configuration, IDirtyListener dirtyListener)
    {
        Configuration = configuration;
        DirtyListener = dirtyListener;

        DirtyListener.RegisterOnDirtyDatabase(OnDirtyDatabase);
        DirtyListener.RegisterOnDirtyDatabaseEntry(OnDirtyEntry);
        DirtyListener.RegisterOnDirtyName(OnDirtyName);
    }

    void OnDirtyName(INameEntry nameEntry)
    {
        Save();
    }

    void OnDirtyEntry(INameDatabaseEntry databaseEntry) 
    {
        Save();
    }

    void OnDirtyDatabase(INameDatabase database)
    {
        Save();
    }

    void Save()
    {
        Configuration.Save();
    }

    public void Dispose()
    {
        DirtyListener.UnregisterOnDirtyDatabase(OnDirtyDatabase);
        DirtyListener.UnregisterOnDirtyDatabaseEntry(OnDirtyEntry);
        DirtyListener.UnregisterOnDirtyName(OnDirtyName);
    }
}
