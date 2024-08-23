using PlayerNicknames.PlayerNicknamesPlugin.Services.ServiceWrappers;
using PlayerNicknames.PlayerNicknamesPlugin.Services.ServiceWrappers.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Core.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Core.Wrappers;
using PlayerRenamer;

namespace PlayerNicknames.PlayerNicknamesPlugin.Core;

internal class PlayerServices : IPlayerServices
{
    readonly DalamudServices DalamudServices;

    public IPetLog PetLog { get; }
    public Configuration Configuration { get; }
    public ISheets Sheets { get; }
    public IStringHelper StringHelper { get; }
    public IClippedNameDatabase ClippedNameDatabase { get; }

    public PlayerServices(in DalamudServices services)
    {
        DalamudServices = services;

        ClippedNameDatabase = new ClippedNameWrapper();
        PetLog = new PetLogWrapper(DalamudServices.PluginLog);
        Sheets = new SheetsWrapper(DalamudServices);
        StringHelper = new StringHelperWrapper(Sheets);

        Configuration = DalamudServices.PlayerNicknamesPlugin.GetPluginConfig() as Configuration ?? new Configuration();
    }
}
