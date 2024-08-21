using System;

namespace PlayerNicknames.PlayerNicknamesPlugin.Windowing.Interfaces;

internal interface IWindowHandler : IDisposable
{
    void Open<T>() where T : IPWindow;
    void Close<T>() where T : IPWindow;
    void Toggle<T>() where T : IPWindow;
    T? GetWindow<T>() where T : IPWindow;
}
