using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace StS2Aris.StS2ArisCode.Utils;

public static class CardPileCmdCompat
{
    private static readonly MethodInfo? GiveToAnotherPlayerMethod = typeof(CardPileCmd)
        .GetMethods(BindingFlags.Public | BindingFlags.Static)
        .FirstOrDefault(method => method.Name == "GiveToAnotherPlayer");

    private static readonly FieldInfo? OwnerField = AccessTools.Field(typeof(CardModel), "_owner");

    public static async Task GiveToAnotherPlayer(
        CardModel card,
        Player player,
        PileType pileType,
        CardPilePosition position = CardPilePosition.Bottom,
        AbstractModel? clonedBy = null)
    {
        if (GiveToAnotherPlayerMethod != null)
        {
            await (Task)GiveToAnotherPlayerMethod.Invoke(null, [card, player, pileType, position, clonedBy])!;
            return;
        }

        CardPile? oldPile = card.Pile;
        oldPile?.RemoveInternal(card);
        OwnerField?.SetValue(card, null);
        card.Owner = player;
        await CardPileCmd.Add(card, pileType, position, clonedBy);
    }
}
