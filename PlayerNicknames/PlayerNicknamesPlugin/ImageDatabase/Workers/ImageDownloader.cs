using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using PlayerNicknames.PlayerNicknamesPlugin.Core;
using PlayerNicknames.PlayerNicknamesPlugin.Core.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.ImageDatabase.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Lodestone.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Lodestone.Structs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PlayerNicknames.PlayerNicknamesPlugin.ImageDatabase.Workers;

internal class ImageDownloader : IImageDownloader
{
    readonly HttpClient httpClient = new HttpClient();
    readonly DalamudServices DalamudServices;
    readonly ILodestoneNetworker Networker;
    readonly IPlayerServices PetServices;

    readonly Dictionary<ulong, CancellationTokenSource> cancellationTokes = new Dictionary<ulong, CancellationTokenSource>();

    public ImageDownloader(DalamudServices dalamudServices, IPlayerServices petServices, ILodestoneNetworker networker)
    {
        DalamudServices = dalamudServices;
        PetServices = petServices;
        Networker = networker;
    }

    public void DownloadImage(INameDatabaseEntry entry, Action<INameDatabaseEntry, IDalamudTextureWrap> success, Action<Exception> failure, bool comesFromAutomation = false)
    {
        if (IsBeingDownloaded(entry)) return;

        if (FindFileLocally(entry, success, failure))
        {
            return;
        }

        if (!PetServices.Configuration.downloadProfilePictures && comesFromAutomation) return;

        Networker.SearchCharacter(entry, (entry, searchData) => OnSuccess(entry, searchData, success, failure), (e) => PetServices.PetLog.LogVerbose(e));
    }

    void OnSuccess(INameDatabaseEntry entry, LodestoneSearchData searchData, Action<INameDatabaseEntry, IDalamudTextureWrap> success, Action<Exception> failure)
    {
        CancellationTokenSource tokenSource = new CancellationTokenSource();
        cancellationTokes.Add(entry.ContentID, tokenSource);
        CancellationToken token = tokenSource.Token;
        Task.Run(async () => await Download(entry, searchData, (entry, wrap) =>
        {
            cancellationTokes.Remove(entry.ContentID);
            success?.Invoke(entry, wrap);
        }, (e) =>
        {
            cancellationTokes.Remove(entry.ContentID);
            failure?.Invoke(e);
        }, token), token);
    }

    async Task Download(INameDatabaseEntry entry, LodestoneSearchData searchData, Action<INameDatabaseEntry, IDalamudTextureWrap> success, Action<Exception> failure, CancellationToken cancellationToken)
    {
        try
        {
            string? URL = searchData.imageURL;
            if (string.IsNullOrEmpty(URL)) throw new NullReferenceException(URL);

            string filePath = GetFilePath(entry);

            using HttpResponseMessage response = await httpClient.GetAsync(URL, cancellationToken);
            if (response == null)
            {
                failure?.Invoke(new Exception("Response is Null for: " + URL + " at: " + filePath));
                return;
            }
            response.EnsureSuccessStatusCode();

            // Thank DarkArchon for this code :D

            FileStream fileStream = File.Create(filePath);
            await using (fileStream.ConfigureAwait(false))
            {
                int bufferSize = response.Content.Headers.ContentLength > 1024 * 1024 ? 4096 : 1024;
                byte[] buffer = new byte[bufferSize];

                int bytesRead = 0;
                while ((bytesRead = await (await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false)).ReadAsync(buffer, cancellationToken).ConfigureAwait(false)) > 0)
                {
                    await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken).ConfigureAwait(false);
                }
            }
        }
        catch (Exception e)
        {
            failure?.Invoke(e);
            return;
        }

        if (!FindFileLocally(entry, success, failure))
        {
            failure?.Invoke(new FileNotFoundException());
            return;
        }
    }

    bool FindFileLocally(INameDatabaseEntry databaseEntry, Action<INameDatabaseEntry, IDalamudTextureWrap> success, Action<Exception> failure)
    {
        string filepath = GetFilePath(databaseEntry);

        try
        {
            if (File.Exists(filepath))
            {
                Task.Run(async () => await ReadImage(databaseEntry, success, failure));
                return true;
            }
        }
        catch (Exception e)
        {
            failure?.Invoke(e);
        }

        return false;
    }

    async Task ReadImage(INameDatabaseEntry databaseEntry, Action<INameDatabaseEntry, IDalamudTextureWrap> success, Action<Exception> failure)
    {
        try
        {
            ISharedImmediateTexture texture = DalamudServices.TextureProvider.GetFromFile(GetFilePath(databaseEntry));
            if (texture == null)
            {
                failure.Invoke(new Exception("Texture couldnt load"));
                return;
            }
            IDalamudTextureWrap textureWrap = await texture.RentAsync();
            if (textureWrap == null)
            {
                failure.Invoke(new Exception("Wrap null"));
                return;
            }
            success?.Invoke(databaseEntry, textureWrap);
        }
        catch (Exception e)
        {
            failure.Invoke(e);
            return;
        }
    }

    string GetFilePath(INameDatabaseEntry entry) => Path.Combine(Path.GetTempPath(), $"PetNicknames_{entry.Name}_{entry.Homeworld}.jpg");

    public void RedownloadImage(INameDatabaseEntry entry, Action<INameDatabaseEntry, IDalamudTextureWrap> success, Action<Exception> failure)
    {
        if (IsBeingDownloaded(entry)) return;
        DoRedownloadImage(entry, success, failure);
    }

    void DoRedownloadImage(INameDatabaseEntry entry, Action<INameDatabaseEntry, IDalamudTextureWrap> success, Action<Exception> failure)
    {
        string filepath = GetFilePath(entry);
        try
        {
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
            DownloadImage(entry, success, failure);
        }
        catch (Exception e)
        {
            failure?.Invoke(e);
            return;
        }
    }

    public void Dispose()
    {
        foreach (ulong id in cancellationTokes.Keys)
        {
            CancellationTokenSource source = cancellationTokes[id];
            try
            {
                source.Cancel();
                source.Dispose();
            }
            catch { }
        }

        cancellationTokes.Clear();
    }

    public bool IsBeingDownloaded(INameDatabaseEntry entry)
    {
        if (Networker.IsBeingDownloaded(entry)) return true;
        foreach (ulong id in cancellationTokes.Keys)
        {
            if (id != entry.ContentID) continue;
            return true;
        }
        return false;
    }
}
