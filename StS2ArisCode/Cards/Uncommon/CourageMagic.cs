using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using StS2Aris.StS2ArisCode.Character;
using StS2Aris.StS2ArisCode.Powers;
using StS2Aris.StS2ArisCode.Utils;

namespace StS2Aris.StS2ArisCode.Cards;

[Pool(typeof(StS2ArisCardPool))]
public class CourageMagic() : StS2ArisCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllAllies)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [ArisHoverTips.ChargePower()];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<ChargePower>(2m)];

    protected override async Task OnArisPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (CombatState == null)
        {
            return;
        }

        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
        var targets = CombatState.GetTeammatesOf(Owner.Creature)
            .Where(creature => creature is { IsAlive: true, IsPlayer: true } && creature != Owner.Creature);

        foreach (var target in targets)
        {
            await PowerCmd.Apply<ChargePower>(choiceContext, target, DynamicVars["ChargePower"].BaseValue, Owner.Creature, this);
            await PowerCmd.Apply<ChargeRetentionPower>(choiceContext, target, 1m, Owner.Creature, this);
        }
    }
}
