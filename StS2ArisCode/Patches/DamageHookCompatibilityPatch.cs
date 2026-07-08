using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Aris.StS2ArisCode.Cards;
using StS2Aris.StS2ArisCode.Powers;

namespace StS2Aris.StS2ArisCode.Patches;

[HarmonyPatch]
public static class ModifyDamageAdditiveCompatibilityPatch
{
    public static IEnumerable<MethodBase> TargetMethods()
    {
        return typeof(AbstractModel).GetMethods()
            .Where(method => method.Name == nameof(AbstractModel.ModifyDamageAdditive));
    }

    public static bool Prefix(
        AbstractModel __instance,
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        object[] __args,
        ref decimal __result)
    {
        if (__instance is not AbiEshuhFormPower abiEshuhFormPower)
        {
            return true;
        }

        CardPlay? cardPlay = __args.Length > 5 ? __args[5] as CardPlay : null;
        __result = abiEshuhFormPower.ModifyDamageAdditiveCompat(target, amount, props, dealer, cardSource, cardPlay);
        return false;
    }
}

[HarmonyPatch]
public static class ModifyDamageMultiplicativeCompatibilityPatch
{
    public static IEnumerable<MethodBase> TargetMethods()
    {
        return typeof(AbstractModel).GetMethods()
            .Where(method => method.Name == nameof(AbstractModel.ModifyDamageMultiplicative));
    }

    public static bool Prefix(
        AbstractModel __instance,
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        object[] __args,
        ref decimal __result)
    {
        CardPlay? cardPlay = __args.Length > 5 ? __args[5] as CardPlay : null;
        switch (__instance)
        {
            case JobMaidPower jobMaidPower:
                __result = jobMaidPower.ModifyDamageMultiplicativeCompat(target, amount, props, dealer, cardSource, cardPlay);
                return false;
            case EnergyProjection energyProjection:
                __result = energyProjection.ModifyDamageMultiplicativeCompat(target, amount, props, dealer, cardSource);
                return false;
            default:
                return true;
        }
    }
}
