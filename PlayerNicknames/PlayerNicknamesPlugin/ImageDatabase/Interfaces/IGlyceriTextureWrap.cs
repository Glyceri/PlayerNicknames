using Dalamud.Interface.Textures.TextureWraps;
using System;
using PetUser = (string, ushort);

namespace PlayerNicknames.PlayerNicknamesPlugin.ImageDatabase.Interfaces;

internal interface IGlyceriTextureWrap : IDisposable
{
    IDalamudTextureWrap? TextureWrap { get; set; }
    PetUser User { get; }

    bool IsOld { get; }

    void Update();
    void Refresh();
}
