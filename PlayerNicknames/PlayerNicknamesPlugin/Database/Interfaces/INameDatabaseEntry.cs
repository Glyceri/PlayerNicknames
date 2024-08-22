using PlayerNicknames.PlayerNicknamesPlugin.NicknamableUsers.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Serialization;

namespace PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;

internal interface INameDatabaseEntry
{
    ulong ContentID { get; }
    string Name { get; }
    ushort Homeworld { get; }
    string HomeworldName { get; }

    bool IsActive { get; }

    INameEntry ActiveEntry { get; }

    void UpdateEntry(INamableUser pettableUser);
    void UpdateEntry(string username);
    void UpdateEntry(string username, ushort homeworld);

    SerializableUser SerializeEntry();
}
