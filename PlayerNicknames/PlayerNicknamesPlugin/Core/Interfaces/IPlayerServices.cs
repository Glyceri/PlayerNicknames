using PetRenamer.PetNicknames.Services.ServiceWrappers;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PlayerRenamer;

namespace PlayerNicknames.PlayerNicknamesPlugin.Core.Interfaces;

internal interface IPlayerServices
{
    IPetLog PetLog { get; }
    ISheets Sheets { get; }
    Configuration Configuration { get; }
    IStringHelper StringHelper { get; }
}
