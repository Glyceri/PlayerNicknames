using PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Lodestone.Enums;
using PlayerNicknames.PlayerNicknamesPlugin.Lodestone.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Lodestone.Structs;
using System;
using System.Threading;

namespace PlayerNicknames.PlayerNicknamesPlugin.Lodestone.Lodestone;

internal class LodestoneQueueElement : ILodestoneQueueElement, IDisposable
{
    public LodestoneQueueState CurrentState { get; protected set; } = LodestoneQueueState.Cooking;
    public DateTime ElementStarted { get; } = DateTime.Now;
    public CancellationTokenSource CancellationTokenSource { get; } = new CancellationTokenSource();
    public CancellationToken CancellationToken { get => CancellationTokenSource.Token; }

    public INameDatabaseEntry Entry { get; private set; }
    public bool Cancelled { get => CancellationToken.IsCancellationRequested; }

    public readonly Action<INameDatabaseEntry, LodestoneSearchData>? Success;
    public readonly Action<Exception>? Failure;

    public LodestoneQueueElement(in INameDatabaseEntry entry, in Action<INameDatabaseEntry, LodestoneSearchData> success, in Action<Exception> failure)
    {
        Entry = entry;
        Success = success;
        Failure = failure;
    }

    public void Cancel()
    {
        try
        {
            CancellationTokenSource.Cancel();
        }
        catch { }
    }
    
    public void Dispose() {
        Cancel();
        CancellationTokenSource?.Dispose();
        CurrentState = LodestoneQueueState.Disposed;
    }

    public void SetState(LodestoneQueueState state)
    {
        if (CurrentState == LodestoneQueueState.Disposed) return;
        CurrentState = state;
    }
}
