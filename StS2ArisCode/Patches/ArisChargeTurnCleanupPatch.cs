using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using StS2Aris.StS2ArisCode.Mechanics;
using StS2Aris.StS2ArisCode.Powers;

namespace StS2Aris.StS2ArisCode.Patches;

[HarmonyPatch(typeof(CombatManager))]
public static class ArisChargeTurnCleanupPatch
{
    [HarmonyPatch(nameof(CombatManager.StartCombatInternal))]
    [HarmonyPrefix]
    public static void ResetOverloadCount()
    {
        ArisCharge.ResetOverloadCount();
    }

    [HarmonyPatch(nameof(CombatManager.EndPlayerTurnPhaseTwoInternal))]
    [HarmonyPrefix]
    public static void ClearChargeAtTurnEnd(CombatManager __instance)
    {
        var state = __instance.DebugOnlyGetState();
        if (state?.CurrentSide != CombatSide.Player)
        {
            return;
        }

        foreach (var player in state.Players)
        {
            if (player.Creature.GetPower<AuxiliaryPower>() != null)
            {
                continue;
            }

            player.Creature.GetPower<ChargePower>()?.RemoveInternal();
        }
    }
}
