using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using StS2Aris.StS2ArisCode.Cards;
using ArisCharacter = StS2Aris.StS2ArisCode.Character.StS2Aris;

namespace StS2Aris.StS2ArisCode.Mechanics;

public static class MimicryEquipmentRegistry
{
    private static readonly Dictionary<string, Type> EquipmentByCharacterKey = new(StringComparer.OrdinalIgnoreCase)
    {
        [nameof(Ironclad)] = typeof(Mask),
        [nameof(Silent)] = typeof(Rogue),
        [nameof(Defect)] = typeof(TrueForm),
        [nameof(Necrobinder)] = typeof(WizardHat),
        [nameof(Regent)] = typeof(StarThrone),
        ["StS2Aris"] = typeof(SuperNova),
        ["STS2ARIS-ST_S2_ARIS"] = typeof(SuperNova),
        ["StS2Aris.StS2ArisCode.Character.StS2Aris"] = typeof(SuperNova),
        ["Kei"] = typeof(LuminousNova),
        ["StS2Kei"] = typeof(LuminousNova),
        ["StS2Hoshino"] = typeof(HoshinoTank),
        ["STS2HOSHINO-ST_S2_HOSHINO"] = typeof(HoshinoTank),
        ["StS2Hoshino.StS2HoshinoCode.Character.StS2Hoshino"] = typeof(HoshinoTank)
    };

    public static void RegisterEquipment(string characterKey, Type equipmentCardType)
    {
        if (!typeof(CardModel).IsAssignableFrom(equipmentCardType))
        {
            return;
        }

        EquipmentByCharacterKey[characterKey] = equipmentCardType;
    }

    public static CardModel? CreateEquipmentFor(Player sourcePlayer, Player owner)
    {
        var equippedCard = sourcePlayer.Character is ArisCharacter
            ? ArisEquipment.GetCurrentJob(sourcePlayer)?.EquipmentCard
            : null;
        var type = equippedCard?.GetType();
        if (type == null)
        {
            if (!TryGetEquipmentType(sourcePlayer.Character, out var registeredType))
            {
                return null;
            }

            type = registeredType;
        }

        var canonical = ModelDb.GetByIdOrNull<CardModel>(ModelDb.GetId(type));
        if (canonical == null)
        {
            return null;
        }

        var copy = owner.Creature.CombatState?.CreateCard(canonical, owner) ?? owner.RunState.CreateCard(canonical, owner);
        if (equippedCard?.IsUpgraded == true)
        {
            CardCmd.Upgrade(copy, CardPreviewStyle.None);
        }

        return copy;
    }

    private static bool TryGetEquipmentType(CharacterModel character, out Type type)
    {
        return EquipmentByCharacterKey.TryGetValue(character.GetType().Name, out type!)
            || EquipmentByCharacterKey.TryGetValue(character.Id.Entry, out type!)
            || EquipmentByCharacterKey.TryGetValue(character.GetType().FullName ?? string.Empty, out type!);
    }
}
