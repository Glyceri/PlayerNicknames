using System;

namespace PlayerNicknames.PlayerNicknamesPlugin.Core.Interfaces;
// Yes... I call it Pet Log because thats what I'm so used to now :sob:
internal interface IPetLog
{
    void Log(object? message);
    void LogInfo(object? obj);
    void LogWarning(object? obj);
    void LogFatal(object? obj);
    void LogVerbose(object? obj);
    void LogError(Exception e, object? obj);
    void LogException(Exception e);
}
