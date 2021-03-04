using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using Reactor;
using Essentials.Options;
using Il2CppSystem;
using Il2CppSystem.Text;
using UnityEngine;

namespace NebulaRoles
{
    [BepInPlugin(Id)]
    [BepInProcess("Among Us.exe")]
    [BepInDependency(ReactorPlugin.Id)]
    public class NebulaPlugin : BasePlugin
    {
        public const string Id = "dev.igalaxy.nebula";

        public static ManualLogSource Logger;
        
        public Harmony Harmony { get; } = new Harmony(Id);

        [HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
        public static class VersionStartPatch
        {
            static void Postfix(VersionShower __instance)
            {
                __instance.text.Text += "\n \n \n \n[6B30BCFF]Nebula Roles v1.0.0[]\n[8E56DBFF]Created by iGalaxy[]\n \nCredits:\nCode Reference\nextraroles.net by NotHunter101\n \nOptions + Cooldown Buttons\nReactor-Essentials by DorCoMaNdO";
            }
        }
    
        [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
        public static class PingPatch
        {
            public static void Postfix(PingTracker __instance)
            {
                __instance.text.Text += "\n \n[6B30BCFF]Nebula Roles v1.0.0[]";
                __instance.text.Text += "\n[8E56DBFF]Created by iGalaxy[]";
                __instance.text.Text += "\n[6A737DFF]GitHub: []iGalaxyYT";
            }
        }
        
        [HarmonyPatch(typeof(GameOptionsData), nameof(GameOptionsData.Method_24))]
        public static class GameOptionsDataPatch
        {
            [HarmonyBefore(new string[] {"com.comando.essentials"})]
            private static void Postfix(ref string __result)
            {
                StringBuilder builder = new StringBuilder(__result);
                builder.AppendLine(" ");
                builder.AppendLine("Roles:");

                __result = builder.ToString();
            }
        }

        [HarmonyPatch(typeof(StatsManager), nameof(StatsManager.AmBanned), MethodType.Getter)]
        public static class AmBannedPatch
        {
            public static void Postfix(out bool __result)
            {
                __result = false;
            }
        }

        public static CustomStringOption MafiaRoles =
            CustomOption.AddString("Mafia", "  [FF0000FF]Mafia[]", new[] {"0%", "10%", "20%", "30%", "40%", "50%", "60%", "70%", "80%", "90%", "100%"});
        public static CustomStringOption JesterRole =
            CustomOption.AddString("Jester", "  [ED54BAFF]Jester[]", new[] {"0%", "10%", "20%", "30%", "40%", "50%", "60%", "70%", "80%", "90%", "100%"});
        public static CustomStringOption SheriffRole = 
            CustomOption.AddString("Sheriff", "  [38FFDBFF]Sheriff[]", new[] {"0%", "10%", "20%", "30%", "40%", "50%", "60%", "70%", "80%", "90%", "100%"});
        public static CustomStringOption DetectiveRole = 
            CustomOption.AddString("Detective", "  [4466D1FF]Detective[]", new[] {"0%", "10%", "20%", "30%", "40%", "50%", "60%", "70%", "80%", "90%", "100%"});
        public static CustomStringOption GuardianRole = 
            CustomOption.AddString("Guardian", "  [4FEF3AFF]Guardian[]", new[] {"0%", "10%", "20%", "30%", "40%", "50%", "60%", "70%", "80%", "90%", "100%"});
        public static CustomStringOption SnitchRole = 
            CustomOption.AddString("Snitch", "  [F2F459FF]Snitch[]", new[] {"0%", "10%", "20%", "30%", "40%", "50%", "60%", "70%", "80%", "90%", "100%"});
        public static CustomStringOption MorphlingRole = 
            CustomOption.AddString("Morphling", "  [754EBCFF]Morphling[]", new[] {"0%", "10%", "20%", "30%", "40%", "50%", "60%", "70%", "80%", "90%", "100%"});

        public override void Load()
        {
            Logger = Log;
            
            // Disables Reactor-Essentials credit, re-added it to title screen instead
            // Contacted creator and confirmed that it was fine with them
            CustomOption.ShamelessPlug = false;
            
            RegisterInIl2CppAttribute.Register();
            Harmony.PatchAll();
        }

    }
}
