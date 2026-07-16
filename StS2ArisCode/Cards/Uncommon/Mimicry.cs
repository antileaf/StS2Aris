using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using StS2Aris.StS2ArisCode.Character;
using StS2Aris.StS2ArisCode.Mechanics;

namespace StS2Aris.StS2ArisCode.Cards;

[Pool(typeof(StS2ArisCardPool))]
public class Mimicry() : StS2ArisCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyAlly)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];

    protected override async Task OnArisPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (play.Target?.Player == null || play.Target.Player == Owner)
        {
            return;
        }

        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);

        var equipment = MimicryEquipmentRegistry.CreateEquipmentFor(play.Target.Player, Owner);
        if (equipment != null)
        {
            await CardPileCmd.AddGeneratedCardToCombat(equipment, PileType.Hand, Owner);
        }

        var options = play.Target.Player.Character.CardPool
            .GetUnlockedCards(play.Target.Player.UnlockState, play.Target.Player.RunState.CardMultiplayerConstraint)
            .Where(card => card.MultiplayerConstraint != CardMultiplayerConstraint.SingleplayerOnly);
        var cards = CardFactory.GetDistinctForCombat(Owner, options, DynamicVars.Cards.IntValue, Owner.RunState.Rng.CombatCardGeneration);
        await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
