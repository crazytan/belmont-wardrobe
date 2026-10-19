using System;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

[BepInPlugin("local.midnightbonuses", "Belmont Wardrobe", "1.1.0")]
public class Plugin : BasePlugin
{
#if STORY_ONLY
    private const bool SkipPlatformVerification = false;
    private const string Edition = "Story Costumes";
    internal static readonly string[] BonusIds = {
        "BonusContentKonamiCode", "BonusContentDoppleganger", "BonusContentBadEnding"
    };
#else
    private const bool SkipPlatformVerification = true;
    private const string Edition = "Full";
    internal static readonly string[] BonusIds = {
        "4708550", "4708530", "BonusContentKonamiCode",
        "BonusContentDoppleganger", "BonusContentBadEnding"
    };
#endif
    internal static ManualLogSource Logger;
    private static bool reported;
    public override void Load()
    {
        Logger = Log;
        new Harmony("local.midnightbonuses").PatchAll();
        AddComponent<MidnightUnlock>();
        Log.LogInfo("Loaded " + Edition + " edition. Enables " + BonusIds.Length + " bonus packs; load a save to receive their items.");
    }

    internal static void Enable(BonusContentManager manager)
    {
        bool allUnlocked = true;
        foreach (var id in BonusIds)
        {
            if (!manager.HasBonusContentUnlocked(id))
            {
                manager.UnlockBonusContent(id, false, SkipPlatformVerification);
                Logger.LogInfo("Bonus " + id + " unlocked=" + manager.HasBonusContentUnlocked(id));
            }
            allUnlocked &= manager.HasBonusContentUnlocked(id);
        }
        if (allUnlocked && !reported) { reported = true; Logger.LogInfo("Verified: all " + BonusIds.Length + " targeted bonus packs unlocked."); }
    }

    [HarmonyPatch(typeof(BonusContentManager), "LockBonusContent")]
    private static class KeepMidnightEnabled
    {
        private static bool Prefix(BonusContentData __0)
        {
            return __0 == null || Array.IndexOf(BonusIds, __0.GetID()) < 0;
        }
    }

    [HarmonyPatch(typeof(BonusContentManager), "OnFirstRoomLoaded")]
    private static class GiveItemsAfterLoad
    {
        private static void Postfix(BonusContentManager __instance)
        {
            try
            {
                Enable(__instance);
                __instance.CheckAllBonusContentIsInPlayerInventory();
                Logger.LogInfo("Checked bonus inventory after save loaded.");
            }
            catch (Exception ex) { Logger.LogError(ex); }
        }
    }
}

public class MidnightUnlock : MonoBehaviour
{
    public MidnightUnlock(IntPtr ptr) : base(ptr) { }
    private float nextCheck;
    private bool failed;
    public void Update()
    {
        if (failed || Time.realtimeSinceStartup < nextCheck) return;
        nextCheck = Time.realtimeSinceStartup + 3;
        try
        {
            var manager = BonusContentManager.instance;
            if (manager == null || !manager._isInitialized) return;
            Plugin.Enable(manager);
        }
        catch (Exception ex) { failed = true; Plugin.Logger.LogError(ex); }
    }
}
