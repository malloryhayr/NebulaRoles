using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using static NebulaRoles.NebulaLogic;

namespace NebulaRoles
{
    public class UpdatePatch
    {
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public static class HudUpdateManagerPatch
        {
            static void Postfix(HudManager __instance)
            {
                
                Main.Logic.ClearJesterTasks();
                
                var roles = new List<(string roleName, Color color)>()
                {
                    ("Jester", Main.Palette.JesterColor)
                };
                
                foreach (var player in PlayerControl.AllPlayerControls)
                    player.nameText.Color = player.Data.IsImpostor && PlayerControl.LocalPlayer.Data.IsImpostor
                        ? Color.red
                        : Color.white;
                
                foreach (var (roleName, roleColor) in roles)
                {
                    var role = Main.Logic.GetRolePlayer(roleName);
                    if (role == null)
                        continue;
                    if (PlayerControl.LocalPlayer.IsPlayerRole(roleName))
                        role.PlayerControl.nameText.Color = roleColor;
                }
                
                foreach (var player in PlayerControl.AllPlayerControls)
                    if (MeetingHud.Instance != null)
                        foreach (var playerVoteArea in MeetingHud.Instance.playerStates)
                            if (playerVoteArea.NameText != null && player.PlayerId == playerVoteArea.TargetPlayerId)
                                playerVoteArea.NameText.Color = player.nameText.Color;
            }
        }
    }
}