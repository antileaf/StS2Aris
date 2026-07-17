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
using StS2Aris.StS2ArisCode.Utils;

namespace StS2Aris.StS2ArisCode.Cards;
[Pool(typeof(StS2ArisCardPool))]
public class ThresholdBreak() : StS2ArisCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [ArisHoverTips.ChargePower()];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HpLossVar(5),
        new PowerVar<ChargePower>(3m)
    ];

    protected override async Task OnArisPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmdCompat.DamageFromCard(choiceContext, Owner.Creature, DynamicVars.HpLoss.BaseValue, ValueProp.Unpowered | ValueProp.Move, Owner.Creature, this, play);
        await PowerCmd.Apply<ChargePower>(choiceContext, Owner.Creature, DynamicVars["ChargePower"].IntValue, Owner.Creature, this);
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override void OnUpgrade()
    {
        DynamicVars["ChargePower"].UpgradeValueBy(1m);
    }
}


