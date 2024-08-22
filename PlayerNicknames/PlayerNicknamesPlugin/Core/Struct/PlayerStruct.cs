namespace PlayerNicknames.PlayerNicknamesPlugin.Core.Struct;

internal struct PlayerStruct
{
    public readonly string Username;
    public readonly ushort Homeworld;

    public PlayerStruct(string username, ushort homeworld)
    {
        Username = username;
        Homeworld = homeworld;
    }
}
