using Dalamud.Configuration;
using Dalamud.Plugin;
using Newtonsoft.Json;
using PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Serialization;
using System;

namespace PlayerRenamer;

[Serializable]
internal class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 1;

    [JsonIgnore]
    IDalamudPluginInterface? PlayerNicknamesPlugin;

    [JsonIgnore]
    INameDatabase? Database = null;

    public SerializableUser[]? SerializableUsers { get; set; } = null;

    public bool showKofiButton = true;
    public bool quickButtonsToggle = true;
    public bool useContextMenus = true;
    public bool downloadProfilePictures = true;
    public bool debugModeActive = false;
    public bool openDebugWindowOnStart = false;
    public bool showOnPartyList = true;
    public bool showOnNameplates = true;

    public void Initialise(IDalamudPluginInterface playerNicknamesPlugin, INameDatabase database)
    {
        PlayerNicknamesPlugin = playerNicknamesPlugin;
        Database = database;

        SerializableUsers ??= [];
    }

    public void Save()
    {
        SerializableUsers = Database!.SerializeDatabase();

        PlayerNicknamesPlugin?.SavePluginConfig(this);
    }
}
