using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using StS2Aris.StS2ArisCode.Character;
using StS2Aris.StS2ArisCode.Keywords;
using StS2Aris.StS2ArisCode.Powers;

namespace StS2Aris.StS2ArisCode.Cards;

[Pool(typeof(StS2ArisCardPool))]
public class ElectromagneticInduction() : StS2ArisCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(ArisKeywords.Shock),
        HoverTipFactory.FromPower<ElectromagneticInductionPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<ShockPower>(2m)];

    protected override async Task OnArisPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (play.Target == null)
        {
            return;
        }

        await PowerCmd.Apply<ShockPower>(choiceContext, play.Target, DynamicVars["ShockPower"].BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<ElectromagneticInductionPower>(choiceContext, play.Target, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ShockPower"].UpgradeValueBy(1m);
    }
}
