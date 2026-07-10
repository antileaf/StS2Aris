using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using StS2Aris.StS2ArisCode.Character;

namespace StS2Aris.StS2ArisCode.Cards;

[Pool(typeof(StS2ArisCardPool))]
public class CurrentAmplification() : StS2ArisCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Shock>(true)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];

    protected override async Task OnArisPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (Owner.Creature.CombatState == null)
        {
            return;
        }

        await Shock.CreateInHand(Owner, DynamicVars.Cards.IntValue, Owner.Creature.CombatState);
        foreach (var shock in Owner.PlayerCombatState?.Hand.Cards.OfType<Shock>() ?? [])
        {
            if (shock.IsUpgradable)
            {
                CardCmd.Upgrade(shock);
            }
        }
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}
