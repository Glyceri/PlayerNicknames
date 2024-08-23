using System;

namespace PlayerNicknames.PlayerNicknamesPlugin.Hooking.Interfaces;

internal interface IHookableElement : IDisposable
{
    void Init();
}
