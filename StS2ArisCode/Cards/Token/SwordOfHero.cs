using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Aris.StS2ArisCode.Character;
using StS2Aris.StS2ArisCode.Keywords;
using StS2Aris.StS2ArisCode.Mechanics;
using StS2Aris.StS2ArisCode.Powers;
using StS2Aris.StS2ArisCode.Utils;

namespace StS2Aris.StS2ArisCode.Cards;
[Pool(typeof(TokenCardPool))]
public class SwordOfHero() : StS2ArisEquipmentCard(1, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy), IArisOutputCard
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(ArisKeywords.Output),
        HoverTipFactory.FromKeyword(ArisKeywords.Reward),
        HoverTipFactory.FromKeyword(ArisKeywords.Equipment),
        HoverTipFactory.FromKeyword(ArisKeywords.ClassChange),
        HoverTipFactory.FromKeyword(ArisKeywords.Job),
        ArisHoverTips.ChargePower()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6, ValueProp.Move),
        new PowerVar<ChargePower>(1m)
    ];

    public override ArisJobPower CreateJobPower()
    {
        return MakeJobPower<JobHeroPower>();
    }

    protected override async Task OnClassChange(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target).Execute(choiceContext);
        await PowerCmd.Apply<ChargePower>(choiceContext, Owner.Creature, DynamicVars["ChargePower"].IntValue, Owner.Creature, this);
    }
}



