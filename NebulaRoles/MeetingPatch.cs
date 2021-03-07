using HarmonyLib;
using UnhollowerBaseLib;
using static NebulaRoles.NebulaLogic;

namespace NebulaRoles
{
    [HarmonyPatch(typeof(UnityEngine.Object), nameof(UnityEngine.Object.Destroy),
        new[] {typeof(UnityEngine.Object)})]
    public static class MeetingExiledEndPatch
    {
        static void Prefix(UnityEngine.Object obj)
        {
            if (ExileController.Instance == null || obj != ExileController.Instance.gameObject)
                return;

            if (ExileController.Instance.exiled == null ||
                !ExileController.Instance.exiled._object.IsPlayerRole("Jester"))
                return;

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte) CustomRPC.JesterWin, Hazel.SendOption.None, -1);
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.IsPlayerRole("Jester"))
                    continue;
                player.RemoveInfected();
                player.Die(DeathReason.Exile);
                player.Data.IsDead = true;
                player.Data.IsImpostor = false;
            }

            var jester = Main.Logic.GetRolePlayer("Jester").PlayerControl;
            jester.Revive();
            jester.Data.IsDead = false;
            jester.Data.IsImpostor = true;
        }
    }
    
    [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString),
        new[] {typeof(StringNames), typeof(Il2CppReferenceArray<Il2CppSystem.Object>)})]
    public static class TranslationPatch
    {
        static void Postfix(ref string __result, StringNames HKOIECMDOKL,
            Il2CppReferenceArray<Il2CppSystem.Object> EBKIKEILMLF)
        {
            if (ExileController.Instance == null || ExileController.Instance.exiled == null)
                return;

            switch (HKOIECMDOKL)
            {
                case StringNames.ExileTextPN:
                case StringNames.ExileTextSN:
                {
                    if (ExileController.Instance.exiled.Object.IsPlayerRole("Jester"))
                        __result = ExileController.Instance.exiled.PlayerName + " was The Jester.";
                    else
                        __result = ExileController.Instance.exiled.PlayerName + " was not The Impostor.";
                    break;
                }
                case StringNames.ImpostorsRemainP:
                case StringNames.ImpostorsRemainS:
                {
                    if (ExileController.Instance.exiled.Object.IsPlayerRole("Jester"))
                        __result = "";
                    break;
                }
            }
        }
    }
}