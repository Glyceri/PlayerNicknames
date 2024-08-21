using System;

namespace PlayerNicknames.PlayerNicknamesPlugin.Windowing.Interfaces;

internal interface IPWindow : IDisposable
{
    void Open();
    void Close();
    void Toggle();

    void NotifyDirty();
}
