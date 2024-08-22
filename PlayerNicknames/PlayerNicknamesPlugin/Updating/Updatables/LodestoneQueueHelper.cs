using Dalamud.Plugin.Services;
using PlayerNicknames.PlayerNicknamesPlugin.ImageDatabase.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Lodestone;
using PlayerNicknames.PlayerNicknamesPlugin.Updating.Interfaces;

namespace PlayerNicknames.PlayerNicknamesPlugin.Update.Updatables;

internal class LodestoneQueueHelper(in LodestoneNetworker networker, in IImageDatabase imageDatabase) : IUpdatable
{
    public bool Enabled { get; set; } = true;

    readonly LodestoneNetworker Networker = networker;
    readonly IImageDatabase ImageDatabase = imageDatabase;

    public void OnUpdate(IFramework framework)
    {
        ImageDatabase.Update();
        Networker.Update(framework);
    }
}
