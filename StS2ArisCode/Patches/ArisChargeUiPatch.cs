using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Nodes.Combat;
using StS2Aris.StS2ArisCode.UI;

namespace StS2Aris.StS2ArisCode.Patches;

[HarmonyPatch(typeof(NCombatUi))]
public static class ArisChargeUiPatch
{
    private static readonly AccessTools.FieldRef<NCombatUi, NEnergyCounter?> EnergyCounterRef =
        AccessTools.FieldRefAccess<NCombatUi, NEnergyCounter?>("_energyCounter");

    [HarmonyPatch(nameof(NCombatUi.Activate))]
    [HarmonyPostfix]
    public static void AddChargeUi(NCombatUi __instance, CombatState state)
    {
        var player = LocalContext.GetMe(state);
        var energyCounter = EnergyCounterRef(__instance);
        if (energyCounter == null || energyCounter.GetNodeOrNull<NChargeCounter>("ArisChargeCounter") != null)
        {
            return;
        }

        if (player != null)
        {
            energyCounter.AddChild(NChargeCounter.Create(player));
        }
    }
}
