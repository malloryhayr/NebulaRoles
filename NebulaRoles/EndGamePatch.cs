using System.Linq;
using HarmonyLib;
using UnityEngine;
using static NebulaRoles.NebulaLogic;

namespace NebulaRoles
{
    [HarmonyPatch(typeof(EndGameManager), "SetEverythingUp")]
    public static class EndGamePatch
    {
        public static bool Prefix()
        {
            if (TempData.winners.Count <= 1 || !TempData.DidHumansWin(TempData.EndReason))
                return true;

            TempData.winners.Clear();
            var orderLocalPlayers = Main.State.LocalPlayers.Where(player => player.PlayerId == Main.State.LocalPlayer.PlayerId).ToList();
            orderLocalPlayers.AddRange(Main.State.LocalPlayers.Where(player => player.PlayerId != Main.State.LocalPlayer.PlayerId));

            foreach (var winner in orderLocalPlayers)
                TempData.winners.Add(new WinningPlayerData(winner.Data));

            return true;
        }

        public static void Postfix(EndGameManager __instance)
        {
            foreach (var poolablePlayer in Object.FindObjectsOfType<PoolablePlayer>())
            {
                if (poolablePlayer.NameText.Text == Main.Logic.GetRolePlayer("Jester").PlayerControl.nameText.Text)
                {
                    poolablePlayer.NameText.Color = Main.Palette.JesterColor;
                }
            }
            
            if (!TempData.DidHumansWin(TempData.EndReason))
                switch (Main.State.LocalPlayer.GetModdedControl().Role)
                {
                    case "Jester":
                        __instance.WinText.Text = "Victory";
                        __instance.WinText.Color = Main.Palette.JesterColor;
                        __instance.BackgroundBar.material.color = Main.Palette.JesterColor;
                        return;
                    default:
                        return;
                }

            var flag = Main.State.LocalPlayers.Count(player => player.PlayerId == Main.State.LocalPlayer.PlayerId) == 0;

            if (!flag)
                return;

            __instance.WinText.Text = "Defeat";
            __instance.WinText.Color = Palette.ImpostorRed;
            __instance.BackgroundBar.material.color = new Color(1, 0, 0);
        }
    }
}