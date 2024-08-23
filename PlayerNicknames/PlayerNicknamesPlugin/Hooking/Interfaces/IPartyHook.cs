namespace PlayerNicknames.PlayerNicknamesPlugin.Hooking.Interfaces;

internal interface IPartyHook : IHookableElement
{
    ulong HoveredContentID { get; }
    string HoveredName { get; }
    ushort HoveredHomeworld {  get; }
}
