using HarmonyLib;
using Hazel;
using System.Collections.Generic;
using System.Linq;
using Il2CppSystem;
using UnhollowerBaseLib;
using static NebulaRoles.NebulaLogic;

namespace NebulaRoles
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSetInfected))]
    public static class SetInfectedPatch
    {
        public static void Postfix(Il2CppReferenceArray<GameData.PlayerInfo> JPGEIBIBJPJ)
        {
            Main.Config.SetConfigSettings();
            Main.Logic.AllModPlayerControl.Clear();
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte) CustomRPC.ResetVariables, Hazel.SendOption.None, -1);
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            var crewmates = PlayerControl.AllPlayerControls.ToArray().ToList();
            foreach (var plr in crewmates)
                Main.Logic.AllModPlayerControl.Add(new ModPlayerControl
                {
                    PlayerControl = plr, Role = "Impostor"
                });
            crewmates.RemoveAll(x => x.Data.IsImpostor);
            foreach (var plr in crewmates)
                plr.GetModdedControl().Role = "Crewmate";

            
            var roles = new List<(string roleName, float spawnChance, CustomRPC rpc)>()
            {
                ("Jester", Main.Config.JesterRole, CustomRPC.SetJester)
            };

            var rand = new Random();
            foreach (var (roleName, spawnChance, rpc) in roles)
            {
                var shouldSpawn = crewmates.Count > 0 && rand.Next(0, 100) <= spawnChance;
                if (!shouldSpawn)
                    continue;
                
                var randomCrewmateIndex = rand.Next(0, crewmates.Count);
                crewmates[randomCrewmateIndex].GetModdedControl().Role = roleName;
                var playerIdForRole = crewmates[randomCrewmateIndex].PlayerId;
                crewmates.RemoveAt(randomCrewmateIndex);

                System.Console.WriteLine($"Spawning {roleName} with PlayerID = {playerIdForRole}");

                writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte) rpc, Hazel.SendOption.None, -1);
                writer.Write(playerIdForRole);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }

            Main.State.LocalPlayers.Clear();
            Main.State.LocalPlayer = PlayerControl.LocalPlayer;
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                var shouldAddPlayer = !player.Data.IsImpostor && !player.IsPlayerRole("Joker");
                if (shouldAddPlayer)
                {
                    Main.State.LocalPlayers.Add(player);
                }
            }
            
            writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte) CustomRPC.SetLocalPlayers, Hazel.SendOption.None, -1);
            writer.WriteBytesAndSize(Main.State.LocalPlayers.Select(player => player.PlayerId).ToArray());
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }
    }
}