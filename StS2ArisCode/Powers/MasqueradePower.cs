using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;

namespace StS2Aris.StS2ArisCode.Powers;

public sealed class MasqueradePower : StS2ArisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != CombatSide.Player || Owner.IsDead)
        {
            return;
        }

        var players = combatState.Players
            .Where(player => player.Creature is { IsAlive: true, IsDead: false })
            .ToList();
        if (players.Count <= 1)
        {
            return;
        }

        Flash();
        for (var i = 0; i < Amount; i++)
        {
            foreach (var player in players)
            {
                await CardPileCmd.Draw(choiceContext, player);
            }

            foreach (var player in players)
            {
                await PassOneCard(choiceContext, player, NextPlayer(players, player));
            }
        }
    }

    private async Task PassOneCard(PlayerChoiceContext choiceContext, Player from, Player to)
    {
        var hand = PileType.Hand.GetPile(from);
        if (hand.Cards.Count == 0)
        {
            return;
        }

        var selected = (await CardSelectCmd.FromHand(
            choiceContext,
            from,
            new CardSelectorPrefs(new LocString("cards", "STS2ARIS-MASQUERADE.selectionScreenPrompt"), 1),
            null,
            this)).FirstOrDefault();
        if (selected == null)
        {
            return;
        }

        await CardPileCmdCompat.GiveToAnotherPlayer(selected, to, PileType.Hand);
    }

    private static Player NextPlayer(List<Player> players, Player player)
    {
        var index = players.IndexOf(player);
        return players[(index + 1) % players.Count];
    }
}
