using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Aris.StS2ArisCode.Character;
using StS2Aris.StS2ArisCode.Powers;
using StS2Aris.StS2ArisCode.Utils;

namespace StS2Aris.StS2ArisCode.Cards;

[Pool(typeof(StS2ArisCardPool))]
public class EnergyCharge() : StS2ArisCard(0, CardType.Skill, CardRarity.Basic, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [ArisHoverTips.ChargePower()];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(4, ValueProp.Move),
        new PowerVar<ChargePower>(1m)
    ];

    protected override async Task OnArisPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        await CommonActions.CardBlock(this, play);

        int energyToSpend = DynamicVars["ChargePower"].IntValue;
        int energySpent = Math.Min(Owner.PlayerCombatState?.Energy ?? 0, energyToSpend);
        if (energySpent <= 0)
        {
            return;
        }

        ICombatState? combatState = CombatState ?? Owner.Creature.CombatState;
        if (combatState == null)
        {
            return;
        }

        CombatManager.Instance.History.EnergySpent(combatState, energySpent, Owner);
        Owner.PlayerCombatState!.LoseEnergy(energySpent);
        await Hook.AfterEnergySpent(combatState, this, energySpent);
        await PowerCmd.Apply<ChargePower>(choiceContext, Owner.Creature, energySpent, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars["ChargePower"].UpgradeValueBy(1m);
    }
}
