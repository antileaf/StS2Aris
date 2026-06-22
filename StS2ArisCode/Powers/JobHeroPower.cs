using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using StS2Aris.StS2ArisCode.Cards;

namespace StS2Aris.StS2ArisCode.Powers;

public sealed class JobHeroPower : ArisJobPower
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Amount", 1m)];
    public override string AnimationSuffix => "Hero";

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (PlayerOwner == null || cardPlay.Card.Owner.Creature != Owner || cardPlay.Card.Type != CardType.Skill || CombatState == null)
        {
            return;
        }

        Flash();
        for (var i = 0; i < Amount + LevelBonus; i++)
        {
            await CardPileCmd.AddGeneratedCardToCombat(CombatState.CreateCard<SwordOfHero>(PlayerOwner), PileType.Hand, PlayerOwner);
        }
    }

    public override async Task OnClassChange(PlayerChoiceContext choiceContext)
    {
        if (EquipmentCard == null)
        {
            return;
        }

        Flash();
        await PowerCmd.Apply<ChargePower>(choiceContext, Owner, EquipmentCard.DynamicVars["ChargePower"].BaseValue, Owner, EquipmentCard);
    }
}
