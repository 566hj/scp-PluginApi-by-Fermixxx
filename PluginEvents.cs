using System;
using System.Collections.Generic;

public static class PluginEvents {
    internal static Dictionary<string, Delegate> Events = new();

    static PluginEvents() {
        string[] names = {
            "OnPlayerJoined", "OnPlayerLeft", "OnPlayerVerified",
            "OnRoundStart", "OnRoundEnd", "OnPlayerDamaged",
            "OnPlayerChangeRole", "OnDoorOpened", "OnPlayerEnteredRoom",
            "OnKeybindPressed", "OnPlayerScaleChanged", "OnGeneratorStateChanged",
            "OnPlayerCommand", "OnPlayerPickupItem", "OnPlayerDropItem",
            "OnWarheadStatusChanged", "OnPlayerCheckDoor", "OnAmmoPickup", "OnElevatorUsed",
            "OnCamSwitch", "OnGrenadeThrown", "OnFlashlightToggled", "OnScp079Ping",
            "OnDecontaminationStart", "OnDecontaminationEnd", "OnTeslaTrigger",
            "On079CameraInteract", "On079ConsoleUse", "OnPocketDimEnter",
            "On914ProcessStart", "On914ProcessEnd", "On914OutputSpawn",
            "OnScp079Pinging", "OnScp079Pinged", "OnServerRoundEndingConditionsCheck",
            "OnTeamRespawn", "OnChaosSpawn", "OnNtfSpawn", "OnLockdownStart",
            "OnLockdownEnd", "OnElevatorArrive", "OnElevatorDepart", "OnScpRelease",
            "OnScpCapture", "OnEscapeEvent", "OnChestLooted", "OnFirearmReload",
            "OnFirearmShoot", "OnProjectileHit", "OnMedkitUse", "OnAdrenalineUse",
            "OnGrenadePickup", "OnMirvExplode", "OnTeslaGateActivate", "OnScpHealthChange",
            "OnAdminCommandRun", "OnRemoteAdminUse", "OnIntercomUse", "OnRadioSend",
            "OnRadioReceive", "OnRadioRangeChange", "OnLockPickStart", "OnLockPickEnd",
            "OnKeycardSwipe", "OnElevatorCall", "OnDoorLockChange", "OnGateBreach",
            "OnPocketDimExit", "OnRvDetectorUse", "OnScannerUse", "OnFlashbangDetonate",
            "OnSmokeGrenadeDetonate", "OnCASSIEAnnouncementStart", "OnCASSIEAnnouncementEnd",
            "OnChestSpawn", "OnItemDespawn", "OnRoomDecontaminateStart",
            "OnRoomDecontaminateEnd", "OnClassDEscape", "OnSurfaceChopperCall",
            "OnChopperArrive", "OnBroadcastMessage", "OnCustomEventX", "OnCustomEventY", "OnCustomEventZ"
        };
        foreach (var name in names)
            Events[name] = Delegate.Empty;
    }

    public static void AddListener(string name, Delegate handler) {
        if (Events.ContainsKey(name))
            Events[name] = Delegate.Combine(Events[name], handler);
        else
            Events[name] = handler;
    }

    public static void RemoveListener(string name, Delegate handler) {
        if (Events.TryGetValue(name, out var existing))
            Events[name] = Delegate.Remove(existing, handler);
    }

    public static void Raise(string name, params object?[] args) {
        if (Events.TryGetValue(name, out var dlg))
            dlg.DynamicInvoke(args);
    }
}