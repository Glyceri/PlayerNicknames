using Newtonsoft.Json;
using PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;
using System;

namespace PlayerNicknames.PlayerNicknamesPlugin.Serialization;

[Serializable]
internal class SerializableUser
{
    public readonly ulong ContentID;
    public readonly string Name;
    public readonly ushort Homeworld;

    public readonly string CustomName;

    [JsonConstructor]
    public SerializableUser(ulong contentId, string name, ushort homeworld, int[] softSkeletonData, string customName)
    {
        ContentID = contentId;
        Name = name;
        Homeworld = homeworld;
        CustomName = customName;
    }

    public SerializableUser(in INameDatabaseEntry entry)
    {
        ContentID = entry.ContentID;
        Name = entry.Name;
        Homeworld = entry.Homeworld;
        CustomName = entry.ActiveEntry.GetName() ?? string.Empty;
    }
}
