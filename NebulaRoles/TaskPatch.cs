using HarmonyLib;
using UnityEngine;
using static NebulaRoles.NebulaLogic;

namespace NebulaRoles
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetTasks))]
    public static class TaskPatch
    {
        static void Postfix(PlayerControl __instance)
        {
            if (Main.State.LocalPlayer != null)
            {
                switch (Main.State.LocalPlayer.GetModdedControl().Role)
                {
                    case "Jester":
                    {
                        ImportantTextTask ImportantTasks = new GameObject("JesterTasks").AddComponent<ImportantTextTask>();
                        ImportantTasks.transform.SetParent(__instance.transform, false);
                        ImportantTasks.Text = "[ED54BAFF]Get voted out to win.[]\nNo Tasks";
                        __instance.myTasks.Insert(0, ImportantTasks);
                        break;
                    }
                    case "Sheriff":
                    {
                        ImportantTextTask ImportantTasks = new GameObject("SheriffTasks").AddComponent<ImportantTextTask>();
                        ImportantTasks.transform.SetParent(__instance.transform, false);
                        ImportantTasks.Text = "[38FFDBFF]Shoot the [FF0000FF]Impostor[38FFDBFF].[]";
                        __instance.myTasks.Insert(0, ImportantTasks);
                        break;
                    }
                }
            }
        }
    }
}