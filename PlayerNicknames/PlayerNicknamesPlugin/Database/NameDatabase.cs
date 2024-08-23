using Dalamud.Utility;
using PlayerNicknames.PlayerNicknamesPlugin.Core.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.DirtySystem.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Serialization;
using System.Collections.Generic;

namespace PlayerNicknames.PlayerNicknamesPlugin.Database;

internal class NameDatabase : INameDatabase
{
    protected List<INameDatabaseEntry> _entries = new List<INameDatabaseEntry>();

    public INameDatabaseEntry[] Entries { get => _entries.ToArray(); }

    readonly IPlayerServices PlayerServices;
    readonly IDirtyCaller DirtyCaller;

    public NameDatabase(IPlayerServices playerServices, IDirtyCaller dirtyCaller)
    {
        PlayerServices = playerServices;
        DirtyCaller = dirtyCaller;

        Setup();
    }

    void Setup()
    {
        List<INameDatabaseEntry> entries = new List<INameDatabaseEntry>();

        SerializableUser[]? users = PlayerServices.Configuration.SerializableUsers;
        if (users == null) return;

        foreach (SerializableUser user in users)
        {
            string customName = user.CustomName;
            if (customName.IsNullOrWhitespace()) continue;

            entries.Add(new NameDatabaseEntry(PlayerServices, DirtyCaller, user.ContentID, user.Name, user.Homeworld, user.CustomName));
        }

        _entries = entries;
    }

    public INameDatabaseEntry GetEntry(ulong contentID)
    {
        int entriesCount = _entries.Count;

        for (int i = 0; i < entriesCount; i++)
        {
            INameDatabaseEntry entry = _entries[i];
            if (entry.ContentID != contentID) continue;

            return entry;
        }

        INameDatabaseEntry newEntry = new NameDatabaseEntry(PlayerServices, DirtyCaller, contentID, "[UNKNOWN]", 0, null);
        _entries.Add(newEntry);
        return newEntry;
    }

    public INameDatabaseEntry? GetEntry(string name, ushort homeworld)
    {
        int entriesCount = _entries.Count;

        for (int i = 0; i < entriesCount; i++)
        {
            INameDatabaseEntry entry = _entries[i];
            if (entry.Homeworld != homeworld) continue;
            if (!name.Equals(entry.Name, System.StringComparison.InvariantCultureIgnoreCase)) continue;

            return entry;
        }

        return null;
    }

    public SerializableUser[] SerializeDatabase()
    {
        List<SerializableUser> users = new List<SerializableUser>();
        int entryCount = _entries.Count;

        for (int i = 0; i < entryCount; i++)
        {
            INameDatabaseEntry entry = _entries[i];
            if (!entry.IsActive) continue;
            if (entry.ActiveEntry.GetName().IsNullOrWhitespace()) continue;

            users.Add(entry.SerializeEntry());
        }

        return users.ToArray();
    }
}
