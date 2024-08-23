using PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;

namespace PlayerNicknames.PlayerNicknamesPlugin.Core.Interfaces;

internal interface IClippedNameDatabase
{
    IClippedName GetClippedName(INameDatabaseEntry entry);
    IClippedName? FromCustomName(INameDatabaseEntry entry);
}

internal interface IClippedName
{
    public int Length { get; }
    public string this[int index] { get; }
}
