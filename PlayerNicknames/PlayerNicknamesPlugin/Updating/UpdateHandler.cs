using Dalamud.Plugin.Services;
using PlayerNicknames.PlayerNicknamesPlugin.Core;
using PlayerNicknames.PlayerNicknamesPlugin.Core.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.DirtySystem.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.ImageDatabase.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Lodestone;
using PlayerNicknames.PlayerNicknamesPlugin.NicknamableUsers.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Update.Updatables;
using PlayerNicknames.PlayerNicknamesPlugin.Updating.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Updating.Updatables;
using System.Collections.Generic;

namespace PlayerNicknames.PlayerNicknamesPlugin.Updating;

internal class UpdateHandler : IUpdateHandler
{
    readonly List<IUpdatable> _updatables = new List<IUpdatable>();

    readonly DalamudServices DalamudServices;
    readonly IPlayerServices PlayerServices;
    readonly INameDatabase Database;
    readonly IUserList UserList;
    readonly LodestoneNetworker LodestoneNetworker;
    readonly IImageDatabase ImageDatabase;

    public UpdateHandler(DalamudServices dalamudServices, IPlayerServices playerServices, INameDatabase database, IUserList userList, in LodestoneNetworker lodestoneNetworker, in IImageDatabase imageDatabase)
    {
        DalamudServices = dalamudServices;
        PlayerServices = playerServices;
        Database = database;
        UserList = userList;
        LodestoneNetworker = lodestoneNetworker;
        ImageDatabase = imageDatabase;

        DalamudServices.Framework.Update += OnUpdate;
        Setup();
    }

    void Setup()
    {
        _updatables.Add(new PlayerFindUpdatable(DalamudServices, PlayerServices, Database, UserList));
        _updatables.Add(new LodestoneQueueHelper(LodestoneNetworker, ImageDatabase));
    }

    void OnUpdate(IFramework framework)
    {
        int updatableCount = _updatables.Count;
        for (int i = 0; i < updatableCount; i++)
        {
            _updatables[i].OnUpdate(framework);
        }
    }

    public void Dispose()
    {
        DalamudServices.Framework.Update -= OnUpdate;
    }
}
