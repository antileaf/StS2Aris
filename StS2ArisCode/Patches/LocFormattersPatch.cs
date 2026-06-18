using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using SmartFormat;
using StS2Aris.StS2ArisCode.Formatters;

namespace StS2Aris.StS2ArisCode.Patches;

[HarmonyPatch(typeof(LocManager))]
public static class LocFormattersPatch
{
    [HarmonyPatch("LoadLocFormatters")]
    [HarmonyPostfix]
    public static void LoadLocFormattersPostfix()
    {
        Smart.Default.AddExtensions(new ChargeIconFormatter());
    }
}
