using Godot;
using HarmonyLib;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Cards;
using StS2Aris.StS2ArisCode.Cards;

namespace StS2Aris.StS2ArisCode.Patches;

[HarmonyPatch(typeof(NCard), "UpdateTypePlaque")]
public static class ArisQuestTypePlaquePatch
{
    private static readonly AccessTools.FieldRef<NCard, MegaLabel> TypeLabel =
        AccessTools.FieldRefAccess<NCard, MegaLabel>("_typeLabel");

    private static readonly AccessTools.FieldRef<NCard, NinePatchRect> TypePlaque =
        AccessTools.FieldRefAccess<NCard, NinePatchRect>("_typePlaque");

    public static void Postfix(NCard __instance)
    {
        if (__instance.Model is not StS2ArisCard { IsArisQuest: true } card)
        {
            return;
        }

        TypeLabel(__instance).SetTextAutoSize(
            $"{card.Type.ToLocString().GetFormattedText()}/{new LocString("gameplay_ui", "CARD_TYPE.QUEST").GetFormattedText()}");

        var plaque = TypePlaque(__instance);
        var questMaterial = ResourceLoader.Load<Material>(
            "res://materials/cards/banners/card_banner_quest_mat.tres",
            null,
            ResourceLoader.CacheMode.Reuse);
        if (plaque.Material != questMaterial)
        {
            plaque.Material = questMaterial;
        }

        Callable.From(() => ResizeTypePlaque(__instance)).CallDeferred();
    }

    private static void ResizeTypePlaque(NCard cardNode)
    {
        var label = TypeLabel(cardNode);
        var plaque = TypePlaque(cardNode);
        var centerX = plaque.Position.X + plaque.Size.X * 0.5f;

        var size = plaque.Size;
        size.X = Mathf.Max(label.Size.X + 17f, 61f);
        plaque.Size = size;

        var position = plaque.Position;
        position.X = centerX - plaque.Size.X * 0.5f;
        plaque.Position = position;
    }
}
