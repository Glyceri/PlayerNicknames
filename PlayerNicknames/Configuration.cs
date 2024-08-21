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
