using PlayerRenamer;

namespace PlayerNicknames.PlayerNicknamesPlugin.Core.Interfaces;

internal interface IPlayerServices
{
    public IPetLog PetLog { get; }
    public ISheets Sheets { get; }
    public Configuration Configuration { get; }
}
