using System.Runtime.CompilerServices;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using StS2Aris.StS2ArisCode.Utils;

namespace StS2Aris.StS2ArisCode.Powers;

public sealed class MasqueradePower : StS2ArisPower
{
    private static readonly ConditionalWeakTable<CardModel, TransferMarker> TransferMarkers = new();

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (Owner.IsDead || player != Owner.Player || CombatState == null)
        {
            return;
        }

        var players = CombatState.Players
            .Where(player => player.Creature is { IsAlive: true, IsDead: false })
            .ToList();
        if (players.Count <= 1)
        {
            return;
        }

        Flash();
        await CardPileCmd.Draw(choiceContext, Amount, player);
        await PassOneCard(choiceContext, player, NextPlayer(players, player));
    }

    private async Task PassOneCard(PlayerChoiceContext choiceContext, Player from, Player to)
    {
        var combatState = CombatState;
        if (combatState == null)
        {
            return;
        }

        var hand = PileType.Hand.GetPile(from);
        bool CanSelect(CardModel card) => !WasTransferredThisTurn(card, combatState, from);
        if (!hand.Cards.Any(CanSelect))
        {
            return;
        }

        var selected = (await CardSelectCmd.FromHand(
            choiceContext,
            from,
            new CardSelectorPrefs(new LocString("cards", "STS2ARIS-MASQUERADE.selectionScreenPrompt"), 1),
            CanSelect,
            this)).FirstOrDefault();
        if (selected == null)
        {
            return;
        }

        await GiveToAnotherPlayerHand(selected, to);
    }

    private static async Task GiveToAnotherPlayerHand(CardModel card, Player player)
    {
        MarkTransferredThisTurn(card, player);

        // The Ball transfers a card from the play area. Moving there first also removes the
        // selected-hand holder before ownership changes, keeping both players' hands consistent.
        await CardPileCmd.Add(card, PileType.Play);
        await CardPileCmdCompat.GiveToAnotherPlayer(card, player, PileType.Hand);

        EnsureLocalHandNode(card, player);
    }

    private static void MarkTransferredThisTurn(CardModel card, Player player)
    {
        var combatState = card.CombatState;
        if (combatState == null)
        {
            return;
        }

        var marker = TransferMarkers.GetOrCreateValue(card);
        marker.CombatState = combatState;
        marker.Player = player;
        marker.TurnNumber = player.PlayerCombatState?.TurnNumber ?? -1;
    }

    private static bool WasTransferredThisTurn(CardModel card, ICombatState combatState, Player player)
    {
        return TransferMarkers.TryGetValue(card, out var marker)
               && ReferenceEquals(marker.CombatState, combatState)
               && ReferenceEquals(marker.Player, player)
               && marker.TurnNumber == (player.PlayerCombatState?.TurnNumber ?? -1);
    }

    private static void EnsureLocalHandNode(CardModel card, Player player)
    {
        if (!LocalContext.IsMe(player) || card.Pile?.Type != PileType.Hand)
        {
            return;
        }

        var hand = NPlayerHand.Instance;
        if (hand == null || hand.GetCard(card) != null)
        {
            return;
        }

        var cardNode = NCard.Create(card);
        if (cardNode == null || NCombatRoom.Instance == null)
        {
            return;
        }

        NCombatRoom.Instance.Ui.AddChildSafely(cardNode);
        cardNode.UpdateVisuals(PileType.Hand, CardPreviewMode.Normal);
        hand.Add(cardNode);
    }

    private static Player NextPlayer(List<Player> players, Player player)
    {
        var index = players.IndexOf(player);
        return players[(index + 1) % players.Count];
    }

    private sealed class TransferMarker
    {
        public ICombatState? CombatState { get; set; }
        public Player? Player { get; set; }
        public int TurnNumber { get; set; }
    }
}
