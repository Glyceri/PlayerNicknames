using Dalamud.Interface.Textures.TextureWraps;
using PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;
using System;

namespace PlayerNicknames.PlayerNicknamesPlugin.ImageDatabase.Interfaces;

internal interface IImageDatabase : IDisposable
{
    bool IsDirty { get; }

    void Redownload(INameDatabaseEntry entry, Action<bool>? callback = null);
    IDalamudTextureWrap? GetWrapFor(INameDatabaseEntry? databaseEntry);
    bool IsBeingDownloaded(INameDatabaseEntry? databaseEntry);
    void OnSuccess(INameDatabaseEntry entry, IDalamudTextureWrap textureWrap);

    void Update();
}
