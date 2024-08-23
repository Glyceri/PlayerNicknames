using PlayerNicknames.PlayerNicknamesPlugin.Serialization;

namespace PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;

internal interface INameDatabase
{
    INameDatabaseEntry[] Entries { get; }

    INameDatabaseEntry GetEntry(ulong contentID);
    INameDatabaseEntry? GetEntry(string name, ushort homeworld);

    SerializableUser[] SerializeDatabase();
}
