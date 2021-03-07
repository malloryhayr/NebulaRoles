using System.Collections.Generic;
using HarmonyLib;
using Il2CppSystem;
using UnityEngine;

namespace NebulaRoles
{
    public class DeadPlayer
    {
        public byte KillerId { get; set; }
        public byte PlayerId { get; set; }
        public DateTime KillTime { get; set; }
        public DeathReason DeathReason { get; set; }
    }
    
    [HarmonyPatch]
    public static class NebulaLogic
    {
        public static class Main
        {
            public static ModLogic Logic = new ModLogic();
            public static ModPalette Palette = new ModPalette();
            public static ModConfig Config = new ModConfig();
            public static GameState State = new GameState();
        }

        public class ModLogic
        {
            public ModPlayerControl GetRolePlayer(string roleName)
            {
                return Main.Logic.AllModPlayerControl.Find(x => x.Role == roleName);
            }

            public void ClearJesterTasks()
            {
                var jester = Main.Logic.GetRolePlayer("Jester");
                if (jester == null) return;
                var removeTasks = new List<PlayerTask>();

                var index = 0;
                foreach (var task in jester.PlayerControl.myTasks)
                {
                    if (!PlayerTools.sabotageTasks.Contains(task.TaskType) && index > 0)
                    {
                        removeTasks.Add(task);
                    }
                    index++;
                }

                foreach (var task in removeTasks)
                    jester.PlayerControl.RemoveTask(task);
            }
            
            public List<ModPlayerControl> AllModPlayerControl = new List<ModPlayerControl>();
        }

        public class ModPalette
        {
            public Color MafiaColor = Color.red;
            public Color JesterColor = new Color(0.93f, 0.33f, 0.73f);
            public Color SheriffColor = new Color(0.22f, 1f, 0.86f);
            public Color DetectiveColor = new Color(0.27f, 0.4f, 0.82f);
            public Color GuardianColor = new Color(0.31f, 0.94f, 0.23f);
            public Color SnitchColor = new Color(0.95f, 0.96f, 0.35f);
            public Color MorphlingColor = new Color(0.46f, 0.31f, 0.74f);
        }

        public class ModConfig
        {
            public float MafiaRoles { get; set; }
            public float JesterRole { get; set; }
            public float SheriffRole { get; set; }
            public float DetectiveRole { get; set; }
            public float GuardianRole { get; set; }
            public float SnitchRole { get; set; }
            public float MorphlingRole { get; set; }
            public float SheriffCD { get; set; }

            public void SetConfigSettings()
            {
                this.MafiaRoles = float.Parse(NebulaPlugin.MafiaRoles.GetValue() + "0");
                this.JesterRole = float.Parse(NebulaPlugin.JesterRole.GetValue() + "0");
                this.SheriffRole = float.Parse(NebulaPlugin.SheriffRole.GetValue() + "0");
                this.DetectiveRole = float.Parse(NebulaPlugin.DetectiveRole.GetValue() + "0");
                this.GuardianRole = float.Parse(NebulaPlugin.GuardianRole.GetValue() + "0");
                this.SnitchRole = float.Parse(NebulaPlugin.SnitchRole.GetValue() + "0");
                this.MorphlingRole = float.Parse(NebulaPlugin.MorphlingRole.GetValue() + "0");
                this.SheriffCD = NebulaPlugin.SheriffKillCooldown.GetValue();
            }
        }
        
        public class GameState
        {
            public bool GameStarted = false;
            public List<DeadPlayer> KilledPlayers = new List<DeadPlayer>();
            public List<PlayerControl> LocalPlayers = new List<PlayerControl>();
            public PlayerControl LocalPlayer = null;
        }

        public class ModPlayerControl
        {
            public PlayerControl PlayerControl { get; set; }
            public string Role { get; set; }
            public DateTime? LastAbilityTime { get; set; }
            public bool UsedAbility { get; set; }
        }
        
        public static Color VecToColor(Vector3 vec)
        {
            return new Color(vec.x, vec.y, vec.z);
        }

        public static Vector3 ColorToVec(Color color)
        {
            return new Vector3(color.r, color.g, color.b);
        }
    }

    public static class Extensions
    {
        public static bool IsPlayerRole(this PlayerControl player, string roleName)
        {
            return player.GetModdedControl() != null && player.GetModdedControl().Role == roleName;
        }

        public static NebulaLogic.ModPlayerControl GetModdedControl(this PlayerControl player)
        {
            return NebulaLogic.Main.Logic.AllModPlayerControl.Find(x => x.PlayerControl == player);
        }
    }
}