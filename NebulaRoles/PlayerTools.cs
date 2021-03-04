using System;
using System.Collections.Generic;

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
    }
}