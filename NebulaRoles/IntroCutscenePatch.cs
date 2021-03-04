using HarmonyLib;
using static NebulaRoles.NebulaLogic;

namespace NebulaRoles
{
    [HarmonyPatch(typeof(IntroCutscene.CoBegin__d), nameof(IntroCutscene.CoBegin__d.MoveNext))]
    public class IntroCutscenePatch
    {
        static bool Prefix(IntroCutscene.CoBegin__d __instance)
        {
            if (!PlayerControl.LocalPlayer.IsPlayerRole("Jester"))
                return true;

            var jesterTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            jesterTeam.Add(PlayerControl.LocalPlayer);
            __instance.yourTeam = jesterTeam;
            return true;
        }
        
        static void Postfix(IntroCutscene.CoBegin__d __instance)
        {
            if (PlayerControl.LocalPlayer.IsPlayerRole("Jester"))
            {
                __instance.__this.Title.Text = "Jester";
                __instance.__this.Title.Color = Main.Palette.JesterColor;
                __instance.__this.ImpostorText.Text = "Get voted out to win";
                __instance.__this.BackgroundBar.material.color = Main.Palette.JesterColor;
            }
        }
    }
}