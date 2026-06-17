using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Aris.StS2ArisCode.Character;
using StS2Aris.StS2ArisCode.Keywords;
using StS2Aris.StS2ArisCode.Powers;

namespace StS2Aris.StS2ArisCode.Cards;
[Pool(typeof(StS2ArisCardPool))]
public class Firewall() : StS2ArisCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(ArisKeywords.Shock)];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Weak", 2m),
        new PowerVar<ShockPower>(6m)
    ];

    protected override async Task OnArisPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await PowerCmd.Apply<WeakPower>(choiceContext, play.Target, DynamicVars["Weak"].IntValue, Owner.Creature, this);
        await PowerCmd.Apply<ShockPower>(choiceContext, play.Target, DynamicVars["ShockPower"].IntValue, Owner.Creature, this);
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override void OnUpgrade()
    {
        DynamicVars["Weak"].UpgradeValueBy(1m);
        DynamicVars["ShockPower"].UpgradeValueBy(1m);
    }
}




