using Dalamud.Utility;
using PlayerNicknames.PlayerNicknamesPlugin.Core.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.DirtySystem.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.NicknamableUsers.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Serialization;

namespace PlayerNicknames.PlayerNicknamesPlugin.Database;

internal class NameDatabaseEntry : INameDatabaseEntry
{
    public bool IsActive => !ActiveEntry.GetName().IsNullOrWhitespace();

    public ulong ContentID { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public ushort Homeworld { get; private set; }
    public string HomeworldName { get; private set; } = string.Empty;
    public INameEntry ActiveEntry { get; }

    readonly IPlayerServices PlayerServices;
    readonly IDirtyCaller DirtyCaller;

    public NameDatabaseEntry(IPlayerServices playerServices, IDirtyCaller dirtyCaller, ulong contentID, string name, ushort homeworld, string? nickname)
    {
        PlayerServices = playerServices;
        DirtyCaller = dirtyCaller;

        ContentID = contentID;
        ActiveEntry = new NameEntry(DirtyCaller, nickname);

        SetName(name);
        SetHomeworld(homeworld);
    }

    void SetHomeworld(ushort homeworld)
    {
        Homeworld = homeworld;
        HomeworldName = PlayerServices.Sheets.GetWorldName(Homeworld) ?? "...";
    }

    void SetName(string name)
    {
        Name = name;
    }

    public void UpdateEntry(INamableUser namableUser)
    {
        SetName(namableUser.Name);
        SetHomeworld(namableUser.Homeworld);

        if (IsActive) return;
        if (!namableUser.IsLocalPlayer) return;
        MarkDirty();
    }

    void MarkDirty()
    {
        DirtyCaller.DirtyEntry(this);
    }

    public SerializableUser SerializeEntry() => new SerializableUser(this);
}
