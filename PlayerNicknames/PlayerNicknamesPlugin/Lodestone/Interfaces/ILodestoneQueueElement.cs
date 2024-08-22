using PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Lodestone.Enums;
using System;
using System.Threading;

namespace PlayerNicknames.PlayerNicknamesPlugin.Lodestone.Interfaces;

internal interface ILodestoneQueueElement 
{
    LodestoneQueueState CurrentState { get; }
    DateTime ElementStarted { get; }
    CancellationTokenSource CancellationTokenSource { get; }
    CancellationToken CancellationToken { get; }
    INameDatabaseEntry Entry { get; }   
    bool Cancelled { get; }
    void Cancel();
}