namespace PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;

internal interface INameEntry
{
    string? GetName();
    void SetName(string? name);
}
