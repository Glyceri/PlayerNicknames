using Dalamud.Plugin.Services;

namespace PlayerNicknames.PlayerNicknamesPlugin.Updating.Interfaces;

internal interface IUpdatable
{
    void OnUpdate(IFramework framework);
}
