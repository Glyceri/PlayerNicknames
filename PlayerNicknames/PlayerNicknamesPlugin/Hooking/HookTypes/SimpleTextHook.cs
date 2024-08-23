using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Addon.Lifecycle;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PlayerNicknames.PlayerNicknamesPlugin.Hooking.Interfaces;
using System;
using PlayerNicknames.PlayerNicknamesPlugin.Hooking;
using PlayerNicknames.PlayerNicknamesPlugin.Core.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Core;
using PlayerNicknames.PlayerNicknamesPlugin.DirtySystem.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.NicknamableUsers.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;

namespace PlayerNicknames.PlayerNicknamesPlugin.Hooking.HookTypes;

internal unsafe class SimpleTextHook : ITextHook
{
    protected string EarlyLastAnswer = "";
    protected string LastAnswer = "";

    public bool Faulty { get; protected set; } = true;

    protected DalamudServices Services = null!;
    protected IUserList UserList { get; set; } = null!;
    protected IPlayerServices PlayerServices { get; set; } = null!;
    IDirtyListener DirtyListener { get; set; } = null!;

    protected uint[] TextPos { get; set; } = Array.Empty<uint>();
    protected Func<INameDatabaseEntry, bool> AllowedToFunction = _ => false;

    INamableUser? lastNamableUser = null;

    public virtual void Setup(DalamudServices services, IUserList userList, IPlayerServices playerServices, IDirtyListener dirtyListener, string AddonName, uint[] textPos, Func<INameDatabaseEntry, bool> allowedCallback)
    {
        Services = services;
        UserList = userList;
        PlayerServices = playerServices;
        DirtyListener = dirtyListener;
        TextPos = textPos;
        AllowedToFunction = allowedCallback;

        DirtyListener.RegisterOnDirtyName(OnName);
        DirtyListener.RegisterOnDirtyDatabaseEntry(OnEntry);

        services.AddonLifecycle.RegisterListener(AddonEvent.PostRequestedUpdate, AddonName, HandleUpdate);
    }

    public void SetUnfaulty() => Faulty = false;
    public void SetFaulty() => Faulty = true;

    protected void HandleUpdate(AddonEvent addonEvent, AddonArgs addonArgs) => HandleRework((AtkUnitBase*)addonArgs.Addon);
    
    
    void OnName(INameEntry nameDatabase)
    {
        SetDirty();
    }

    void OnEntry(INameDatabaseEntry entry)
    {
        SetDirty();
    }

    bool isDirty = false;

    void SetDirty()
    {
        isDirty = true;
    }

    void ClearDirty()
    {
        isDirty = false;
    }

    void HandleRework(AtkUnitBase* baseElement)
    {
        if (BlockedCheck()) return;

        if (TextPos.Length == 0) return;
        if (!baseElement->IsVisible) return;
       
        BaseNode bNode = new BaseNode(baseElement);
        AtkTextNode* tNode = GetTextNode(in bNode);
        if (tNode == null) return;

        // Make sure it only runs once
        string tNodeText = tNode->NodeText.ToString();
        if ((tNodeText == string.Empty || tNodeText == LastAnswer) && !isDirty) return;

        ClearDirty();

        if (!OnTextNode(tNode, tNodeText)) LastAnswer = tNodeText;
    }

    protected virtual bool BlockedCheck() => Faulty;

    protected virtual bool OnTextNode(AtkTextNode* textNode, string text)
    {
        INamableUser? user = lastNamableUser = GetUser();
        if (user == null) return false;

        string? customName = user.Name;
        if (customName == null) return false;

        SetText(textNode, user);
        return true;
    }

    protected virtual void SetText(AtkTextNode* textNode, INamableUser user)
    {
        string baseText = textNode->NodeText.ToString() ?? string.Empty;
        if (!CheckIfCanFunction(baseText, user)) return;
        IClippedName clippedName = PlayerServices.ClippedNameDatabase.GetClippedName(user.DatabaseEntry);
        //LastAnswer = PlayerServices.StringHelper.ReplaceATKString(textNode, user.CustomName ?? baseText, clippedName) ?? string.Empty;
    }

    protected virtual bool CheckIfCanFunction(string text, INamableUser user)
    {
        if (AllowedToFunction == null) return true;
        if (AllowedToFunction.Invoke(user.DatabaseEntry)) return true;
        LastAnswer = text;
        return false;
    }

    protected virtual INamableUser? GetUser() => UserList.LocalPlayer;

    protected virtual AtkTextNode* GetTextNode(in BaseNode bNode)
    {
        if (TextPos.Length > 1)
        {
            ComponentNode cNode = bNode.GetComponentNode(TextPos[0]);
            for (int i = 1; i < TextPos.Length - 1; i++)
            {
                if (cNode == null) return null!;
                cNode = cNode.GetComponentNode(TextPos[i]);
            }
            if (cNode == null) return null!;
            return cNode.GetNode<AtkTextNode>(TextPos[^1]);
        }
        return bNode.GetNode<AtkTextNode>(TextPos[0]);
    }

    public void Dispose()
    {
        OnDispose();
        DirtyListener.UnregisterOnDirtyName(OnName);
        DirtyListener.UnregisterOnDirtyDatabaseEntry(OnEntry);

        Services.AddonLifecycle.UnregisterListener(AddonEvent.PostRequestedUpdate, HandleUpdate);
    }

    public virtual void OnDispose() { }
}
