using PlayerNicknames.PlayerNicknamesPlugin.Core.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Core.Wrappers;
using SamplePlugin;

namespace PlayerNicknames.PlayerNicknamesPlugin.Core;

internal class PlayerServices : IPlayerServices
{
    readonly DalamudServices DalamudServices;

    public IPetLog PetLog { get; }
    public Configuration Configuration { get; }
    public ISheets Sheets { get; }

    public PlayerServices(in DalamudServices services)
    {
        DalamudServices = services;

        PetLog = new PetLogWrapper(DalamudServices.PluginLog);
        Sheets = new SheetsWrapper(DalamudServices);

        Configuration = DalamudServices.PlayerNicknamesPlugin.GetPluginConfig() as Configuration ?? new Configuration();
    }
}
