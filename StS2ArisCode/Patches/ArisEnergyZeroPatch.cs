using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using StS2Aris.StS2ArisCode.Powers;

namespace StS2Aris.StS2ArisCode.Patches;

[HarmonyPatch(typeof(PlayerCombatState), nameof(PlayerCombatState.LoseEnergy))]
public static class ArisEnergyZeroPatch
{
    public static void Prefix(PlayerCombatState __instance, out int __state)
    {
        __state = __instance.Energy;
    }

    public static void Postfix(PlayerCombatState __instance, Player ____player, int __state)
    {
        if (__state > 0 && __instance.Energy == 0)
        {
            TaskHelper.RunSafely(EmergencyPowerPower.OnEnergyBecameZero(____player));
        }
    }
}
