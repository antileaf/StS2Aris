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
public class Thunder() : StS2ArisCard(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(ArisKeywords.Overload),
        HoverTipFactory.FromKeyword(ArisKeywords.Charge)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(16, ValueProp.Move),
        new PowerVar<ChargePower>(2m)
    ];

    protected override async Task OnArisPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target).Execute(choiceContext);
        await PowerCmd.Apply<ChargePower>(choiceContext, Owner.Creature, DynamicVars["ChargePower"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(6m);
    }
}



