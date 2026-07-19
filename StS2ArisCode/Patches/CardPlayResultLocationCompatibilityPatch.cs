using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using StS2Aris.StS2ArisCode.CardModels;
using StS2Aris.StS2ArisCode.Cards;
using StS2Aris.StS2ArisCode.Mechanics;

namespace StS2Aris.StS2ArisCode.Patches;

[HarmonyPatch]
public static class CardPlayResultPileCompatibilityPatch
{
    public static MethodBase? TargetMethod()
    {
        return AccessTools.DeclaredMethod(typeof(AbstractModel), "ModifyCardPlayResultPileTypeAndPosition");
    }

    public static bool Prepare()
    {
        return TargetMethod() != null;
    }

    public static void Postfix(
        AbstractModel __instance,
        CardModel card,
        ref (PileType, CardPilePosition) __result)
    {
        if (__instance is IArisEquipmentCard equipmentCard && card == equipmentCard)
        {
            __result = (PileType.None, CardPilePosition.Bottom);
            return;
        }

        if (__instance is PrismaticBeams prismaticBeams &&
            card == prismaticBeams &&
            __result.Item1 == PileType.Discard &&
            ArisCharge.IsOverloadAvailable(prismaticBeams.Owner))
        {
            __result = (PileType.Hand, CardPilePosition.Bottom);
        }
    }
}

[HarmonyPatch]
public static class CardPlayResultLocationCompatibilityPatch
{
    private static readonly Type? CardLocationType = AccessTools.TypeByName("MegaCrit.Sts2.Core.Entities.Cards.CardLocation");
    private static readonly FieldInfo? PlayerField = CardLocationType?.GetField("player");
    private static readonly FieldInfo? PileTypeField = CardLocationType?.GetField("pileType");
    private static readonly FieldInfo? PositionField = CardLocationType?.GetField("position");

    public static MethodBase? TargetMethod()
    {
        return AccessTools.DeclaredMethod(typeof(AbstractModel), "ModifyCardPlayResultLocation");
    }

    public static bool Prepare()
    {
        return TargetMethod() != null &&
               CardLocationType != null &&
               PlayerField != null &&
               PileTypeField != null &&
               PositionField != null;
    }

    public static void Postfix(AbstractModel __instance, CardModel card, ref object __result)
    {
        if (__instance is IArisEquipmentCard equipmentCard && card == equipmentCard)
        {
            __result = CreateLocation(__result, PileType.None, CardPilePosition.Bottom);
            return;
        }

        if (__instance is PrismaticBeams prismaticBeams &&
            card == prismaticBeams &&
            (PileType?)PileTypeField?.GetValue(__result) == PileType.Discard &&
            ArisCharge.IsOverloadAvailable(prismaticBeams.Owner))
        {
            __result = CreateLocation(__result, PileType.Hand, CardPilePosition.Bottom);
        }
    }

    private static object CreateLocation(object currentLocation, PileType pileType, CardPilePosition position)
    {
        return Activator.CreateInstance(
            CardLocationType!,
            PlayerField!.GetValue(currentLocation),
            pileType,
            position)!;
    }
}
