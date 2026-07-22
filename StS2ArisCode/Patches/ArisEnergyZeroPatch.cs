using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using StS2Aris.StS2ArisCode.Powers;

namespace StS2Aris.StS2ArisCode.Patches;

[HarmonyPatch(typeof(PlayerCmd), nameof(PlayerCmd.LoseEnergy))]
public static class ArisEnergyZeroPatch
{
    public static void Prefix(Player player, out int __state)
    {
        __state = player.PlayerCombatState?.Energy ?? 0;
    }

    public static void Postfix(Player player, int __state, ref Task __result)
    {
        if (__state > 0)
        {
            __result = TriggerAfterEnergyLoss(__result, player);
        }
    }

    private static async Task TriggerAfterEnergyLoss(Task originalTask, Player player)
    {
        await originalTask;
        if (player.PlayerCombatState?.Energy == 0)
        {
            await EmergencyPowerPower.OnEnergyBecameZero(player);
        }
    }
}
