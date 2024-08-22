using PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Lodestone.Structs;
using System;

namespace PlayerNicknames.PlayerNicknamesPlugin.Lodestone.Interfaces;

internal interface ILodestoneNetworker
{
    ILodestoneQueueElement SearchCharacter(INameDatabaseEntry entry, Action<INameDatabaseEntry, LodestoneSearchData> success, Action<Exception> failure);
    bool IsBeingDownloaded(INameDatabaseEntry entry);
}
