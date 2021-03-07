using System;
using HarmonyLib;
using Hazel;
using System.Linq;
using static NebulaRoles.NebulaLogic;

namespace NebulaRoles
{
    enum RPC
    {
        PlayAnimation = 0,
        CompleteTask = 1,
        SyncSettings = 2,
        SetInfected = 3,
        Exiled = 4,
        CheckName = 5,
        SetName = 6,
        CheckColor = 7,
        SetColor = 8,
        SetHat = 9,
        SetSkin = 10,
        ReportDeadBody = 11,
        MurderPlayer = 12,
        SendChat = 13,
        StartMeeting = 14,
        SetScanner = 15,
        SendChatNote = 16,
        SetPet = 17,
        SetStartCounter = 18,
        EnterVent = 19,
        ExitVent = 20,
        SnapTo = 21,
        Close = 22,
        VotingComplete = 23,
        CastVote = 24,
        ClearVote = 25,
        AddVote = 26,
        CloseDoorsOfType = 27,
        RepairSystem = 28,
        SetTasks = 29,
        UpdateGameData = 30,
    }
    
    enum CustomRPC
    {
        SetGodfather = 43,
        SetJanitor = 44,
        SetMafioso = 45,
        SetJester = 46,
        SetSheriff = 47,
        SetDetective = 48,
        SetGuardian = 49,
        SetSnitch = 50,
        SetMorphling = 51,
        ResetVariables = 52,
        SetLocalPlayers = 53,
        JesterWin = 54
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
    class HandleRpcPatch
    {
        static void Postfix(byte HKHMBLJFLMC, MessageReader ALMCIJKELCP)
        {
            var packetId = HKHMBLJFLMC;
            var reader = ALMCIJKELCP;
            
            var setRole = new Action<string>(roleName =>
            {
                var roleId = ALMCIJKELCP.ReadByte();
                foreach (var player in PlayerControl.AllPlayerControls)
                    if (player.PlayerId == roleId)
                        player.GetModdedControl().Role = roleName;
            });

            switch (packetId)
            {
                case (byte) CustomRPC.ResetVariables:
                    Main.Config.SetConfigSettings();
                    Main.Logic.AllModPlayerControl.Clear();
                    Main.State.KilledPlayers.Clear();
                    var Crewmates = PlayerControl.AllPlayerControls.ToArray().ToList();
                    foreach (var plr in Crewmates)
                        Main.Logic.AllModPlayerControl.Add(new ModPlayerControl
                        {
                            PlayerControl = plr, Role = "Impostor"
                        });
                    Crewmates.RemoveAll(x => x.Data.IsImpostor);
                    foreach (var plr in Crewmates)
                        plr.GetModdedControl().Role = "Crewmate";
                    break;
                case (byte) CustomRPC.SetLocalPlayers:
                    Main.State.LocalPlayers.Clear();
                    Main.State.LocalPlayer = PlayerControl.LocalPlayer;
                    var localPlayerBytes = ALMCIJKELCP.ReadBytesAndSize();
                    foreach (var id in localPlayerBytes)
                    foreach (var player in PlayerControl.AllPlayerControls)
                        if (player.PlayerId == id)
                            Main.State.LocalPlayers.Add(player);
                    break;
                case (byte) CustomRPC.SetJester:
                    setRole("Jester");
                    break;
                case (byte) CustomRPC.JesterWin:
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
                    break;
                case (byte) CustomRPC.SetSheriff:
                    setRole("Sheriff");
                    break;
            }
        }
    }
}