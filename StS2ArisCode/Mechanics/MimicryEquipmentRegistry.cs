using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using StS2Aris.StS2ArisCode.Cards;

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
        ["Rabbit"] = typeof(UsagiFlap),
        ["StS2Rabbit"] = typeof(UsagiFlap),
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
        if (!TryGetEquipmentType(sourcePlayer.Character, out var type))
        {
            return null;
        }

        var canonical = ModelDb.GetByIdOrNull<CardModel>(ModelDb.GetId(type));
        if (canonical == null)
        {
            return null;
        }

        return owner.Creature.CombatState?.CreateCard(canonical, owner) ?? owner.RunState.CreateCard(canonical, owner);
    }

    private static bool TryGetEquipmentType(CharacterModel character, out Type type)
    {
        return EquipmentByCharacterKey.TryGetValue(character.GetType().Name, out type!)
            || EquipmentByCharacterKey.TryGetValue(character.Id.Entry, out type!)
            || EquipmentByCharacterKey.TryGetValue(character.GetType().FullName ?? string.Empty, out type!);
    }
}
