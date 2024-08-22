using PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;

namespace PlayerNicknames.PlayerNicknamesPlugin.Windowing.Windows.PlayerList;

internal class PlayerListPlayer
{
    public readonly INameDatabaseEntry Entry;
    public string CustomName;
    public string TempName;

    public PlayerListPlayer(in INameDatabaseEntry entry)
    {
        Entry = entry;
        CustomName = entry.ActiveEntry.GetName() ?? string.Empty;
        TempName = CustomName;
    }
}
