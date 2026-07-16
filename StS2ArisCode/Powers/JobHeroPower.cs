using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using StS2Aris.StS2ArisCode.Cards;
using StS2Aris.StS2ArisCode.Utils;

namespace StS2Aris.StS2ArisCode.Powers;

public sealed class JobHeroPower : ArisJobPower
{
    public override string AnimationSuffix => "Hero";
    public int UnequipIncrease => EffectApplications;

    public override async Task OnClassChange(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var equipment = EquipmentCard;
        var combatState = equipment?.CombatState ?? equipment?.Owner.Creature.CombatState;
        if (equipment == null || combatState == null)
        {
            return;
        }

        await DamageCmd.Attack(equipment.DynamicVars.Damage.BaseValue)
            .FromCardCompat(equipment, play)
            .TargetingRandomOpponents(combatState)
            .WithHitCount(HeroSword.GetClassChangeHits(equipment.Owner))
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    public override Task OnUnequipped(PlayerChoiceContext choiceContext, PileType resultPileType)
    {
        HeroSword.IncreaseClassChangeHits(PlayerOwner, UnequipIncrease);
        return Task.CompletedTask;
    }

    public override Task OnLevelUpChanged(PlayerChoiceContext choiceContext)
    {
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override void AddDumbVariablesToPowerDescription(LocString description)
    {
        base.AddDumbVariablesToPowerDescription(description);
        description.Add("Increase", UnequipIncrease);
    }
}
