using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using StS2Aris.StS2ArisCode.Mechanics;

namespace StS2Aris.StS2ArisCode.Powers;

public sealed class JobAtrahasisSuperNovaPower : ArisJobPower
{
    private int _strengthApplied;

    public override string AnimationSuffix => "AOEDPS";

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        await RefreshStrength(new ThrowingPlayerChoiceContext());
    }

    public override async Task AfterEnergyReset(Player player)
    {
        if (player == PlayerOwner)
        {
            await RefreshStrength(new ThrowingPlayerChoiceContext());
        }
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner == PlayerOwner)
        {
            await RefreshStrength(choiceContext);
        }
    }

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power == this)
        {
            await RefreshStrength(choiceContext);
        }
    }

    public override async Task AfterRemoved(Creature oldOwner)
    {
        await RemoveStrength(new ThrowingPlayerChoiceContext(), oldOwner);
    }

    private async Task RefreshStrength(PlayerChoiceContext choiceContext)
    {
        if (Owner.IsDead)
        {
            return;
        }

        var desiredAmount = PlayerOwner != null && ArisCharge.IsOverloadState(PlayerOwner)
            ? (int)(EquipmentStrengthAmount * EffectApplications)
            : 0;
        var amountToApply = desiredAmount - _strengthApplied;
        if (amountToApply == 0)
        {
            return;
        }

        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, amountToApply, Owner, EquipmentCard);
        _strengthApplied = desiredAmount;
    }

    private async Task RemoveStrength(PlayerChoiceContext choiceContext, Creature owner)
    {
        if (_strengthApplied == 0 || owner.IsDead)
        {
            return;
        }

        await PowerCmd.Apply<StrengthPower>(choiceContext, owner, -_strengthApplied, owner, EquipmentCard);
        _strengthApplied = 0;
    }

    public override async Task OnClassChange(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var equipment = EquipmentCard;
        var combatState = equipment?.CombatState ?? equipment?.Owner.Creature.CombatState;
        if (equipment == null || combatState == null)
        {
            return;
        }

        await DamageCmd.Attack(equipment.DynamicVars.Damage.BaseValue).FromCard(equipment, play)
            .TargetingAllOpponents(combatState)
            .WithAttackerAnim("Cast", 0.5f)
            .BeforeDamage(async () =>
            {
                var enemies = combatState.Enemies.Where(e => e.IsAlive).ToList();
                if (enemies.Count == 0)
                {
                    return;
                }

                var beam = NHyperbeamVfx.Create(equipment.Owner.Creature, enemies.Last());
                if (beam != null)
                {
                    NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(beam);
                    await Cmd.Wait(0.5f);
                }

                foreach (var enemy in enemies)
                {
                    var impact = NHyperbeamImpactVfx.Create(equipment.Owner.Creature, enemy);
                    if (impact != null)
                    {
                        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(impact);
                    }
                }
            })
            .Execute(choiceContext);
    }

    public override Task OnLevelUpChanged(PlayerChoiceContext choiceContext)
    {
        return RefreshStrength(choiceContext);
    }
}
