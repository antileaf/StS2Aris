using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.HoverTips;
using StS2Aris.StS2ArisCode.Mechanics;
using ArisCharacter = StS2Aris.StS2ArisCode.Character.StS2Aris;

namespace StS2Aris.StS2ArisCode.Patches;

[HarmonyPatch(typeof(Creature), nameof(Creature.HoverTips), MethodType.Getter)]
public static class ArisCreatureHoverTipsPatch
{
    public static void Postfix(Creature __instance, ref IEnumerable<IHoverTip> __result)
    {
        var player = __instance.Player;
        if (player?.Character is not ArisCharacter)
        {
            return;
        }

        var tips = __result.Where(tip => tip is not CardHoverTip).ToList();
        var equipmentCard = ArisEquipment.GetCurrentJob(player)?.EquipmentCard;
        if (equipmentCard != null)
        {
            tips.Add(HoverTipFactory.FromCard(equipmentCard));
        }

        __result = tips;
    }
}
