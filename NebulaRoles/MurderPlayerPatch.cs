using HarmonyLib;
using Il2CppSystem;
using static NebulaRoles.NebulaLogic;

namespace NebulaRoles
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
    public static class MurderPlayerPatch
    {
        public static bool Prefix(PlayerControl __instance, PlayerControl CAKODNGLPDF)
        {
            if (__instance.IsPlayerRole("Sheriff"))
            {
                __instance.Data.IsImpostor = true;
            }

            return true;
        }

        public static void Postfix(PlayerControl __instance, PlayerControl CAKODNGLPDF)
        {
            var deadBody = new DeadPlayer
            {
                PlayerId = CAKODNGLPDF.PlayerId,
                KillerId = __instance.PlayerId,
                KillTime = DateTime.UtcNow,
                DeathReason = DeathReason.Kill
            };

            if (__instance.IsPlayerRole("Sheriff"))
            {
                __instance.Data.IsImpostor = false;
            }

            if (__instance.PlayerId == CAKODNGLPDF.PlayerId)
            {
                deadBody.DeathReason = (DeathReason) 3;
            }

            Main.State.KilledPlayers.Add(deadBody);
        }
    }
}