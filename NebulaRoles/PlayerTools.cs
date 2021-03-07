using Il2CppSystem;
using System.Collections.Generic;
using static NebulaRoles.NebulaLogic;

namespace NebulaRoles
{
    public static class PlayerTools
    {
        public static PlayerControl closestPlayer = null;
        
        public static List<TaskTypes> sabotageTasks = new List<TaskTypes>
        {
            TaskTypes.FixComms,
            TaskTypes.FixLights,
            TaskTypes.ResetReactor,
            TaskTypes.ResetSeismic,
            TaskTypes.RestoreOxy
        };
        
        // Credit: https://extraroles.net
        public static PlayerControl getClosestPlayer(PlayerControl refplayer)
        {
            var mindist = double.MaxValue;
            PlayerControl closestplayer = null;
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Data.IsDead)
                    continue;
                if (player == refplayer)
                    continue;
                var dist = getDistBetweenPlayers(player, refplayer);
                if (dist >= mindist)
                    continue;

                mindist = dist;
                closestplayer = player;
            }

            return closestplayer;
        }
        
        public static double getDistBetweenPlayers(PlayerControl player, PlayerControl refplayer)
        {
            var refpos = refplayer.GetTruePosition();
            var playerpos = player.GetTruePosition();

            return Math.Sqrt((refpos[0] - playerpos[0]) * (refpos[0] - playerpos[0]) +
                             (refpos[1] - playerpos[1]) * (refpos[1] - playerpos[1]));
        }
        
        public static float getSheriffCD()
        {
            var lastAbilityTime = Main.Logic.GetRolePlayer("Sheriff").LastAbilityTime;
            if (lastAbilityTime == null)
            {
                return Main.Config.SheriffCD;
            }

            var now = DateTime.UtcNow;
            var diff = (TimeSpan) (now - lastAbilityTime);

            var killCooldown = Main.Config.SheriffCD * 1000.0f;
            if (killCooldown - (float) diff.TotalMilliseconds < 0)
                return 0;

            return (killCooldown - (float) diff.TotalMilliseconds) / 1000.0f;
        }
    }
}