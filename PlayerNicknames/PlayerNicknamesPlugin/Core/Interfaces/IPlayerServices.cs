using PlayerNicknames.PlayerNicknamesPlugin.Services.ServiceWrappers.Interfaces;
using PlayerRenamer;

namespace PlayerNicknames.PlayerNicknamesPlugin.Core.Interfaces;

internal interface IPlayerServices
{
    IPetLog PetLog { get; }
    ISheets Sheets { get; }
    Configuration Configuration { get; }
    IStringHelper StringHelper { get; }
    IClippedNameDatabase ClippedNameDatabase { get; }
}
