using Dalamud.Interface.Textures.TextureWraps;
using PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;
using System;

namespace PlayerNicknames.PlayerNicknamesPlugin.ImageDatabase.Interfaces;

internal interface IImageDownloader : IDisposable
{
    void DownloadImage(INameDatabaseEntry entry, Action<INameDatabaseEntry, IDalamudTextureWrap> success, Action<Exception> failure, bool comesFromAutomation = false);
    void RedownloadImage(INameDatabaseEntry entry, Action<INameDatabaseEntry, IDalamudTextureWrap> success, Action<Exception> failure);
    bool IsBeingDownloaded(INameDatabaseEntry entry);
}
