namespace PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;

internal interface INameEntry
{
    string? GetName();
    string? GetOldName();
    void SetName(string? name);
}
