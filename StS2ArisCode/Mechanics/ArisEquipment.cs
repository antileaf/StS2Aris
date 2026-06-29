using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using StS2Aris.StS2ArisCode.Cards;
using StS2Aris.StS2ArisCode.Hooks;
using StS2Aris.StS2ArisCode.Powers;

namespace StS2Aris.StS2ArisCode.Mechanics;

public static class ArisEquipment
{
    public static ArisJobPower? GetCurrentJob(Player player)
    {
        return player.Creature.Powers.OfType<ArisJobPower>().FirstOrDefault();
    }

    public static string? GetCurrentAnimationSuffix(Player player)
    {
        return GetCurrentJob(player)?.AnimationSuffix;
    }

    public static bool ShouldTriggerClassChange(Player player, CardModel equipmentCard)
    {
        var currentJob = GetCurrentJob(player);
        return currentJob?.EquipmentCard?.GetType() != equipmentCard.GetType();
    }

    public static async Task Equip(PlayerChoiceContext choiceContext, CardModel equipmentCard, ArisJobPower nextJob)
    {
        var owner = equipmentCard.Owner;
        var currentJob = GetCurrentJob(owner);
        var changed = currentJob == null || currentJob.GetType() != nextJob.GetType();

        if (currentJob != null)
        {
            if (currentJob.EquipmentCard != null)
            {
                await ReturnEquipmentCard(currentJob.EquipmentCard, PileType.Discard);
            }

            await PowerCmd.Remove(currentJob);
        }

        nextJob.EquipmentCard = equipmentCard;
       // HoldPlayedEquipmentCard(equipmentCard);
        await PowerCmd.Apply(choiceContext, nextJob, owner.Creature, 1m, owner.Creature, equipmentCard);
        await CreatureCmd.TriggerAnim(owner.Creature, "Idle", 0f);

        if (changed)
        {
            await OnJobChanged(choiceContext, owner);
        }
    }

    public static async Task ReturnCurrentJob(PlayerChoiceContext choiceContext, Player player, PileType pileType)
    {
        var currentJob = GetCurrentJob(player);
        if (currentJob == null)
        {
            return;
        }

        await PowerCmd.Remove(currentJob);
        if (currentJob.EquipmentCard != null)
        {
            await ReturnEquipmentCard(currentJob.EquipmentCard, pileType);
        }
    }

    public static async Task TriggerClassChange(PlayerChoiceContext choiceContext, Player player)
    {
        var currentJob = GetCurrentJob(player);
        if (currentJob != null)
        {
            await currentJob.OnClassChange(choiceContext);
            await ArisHook.OnClassChanged(choiceContext, player, currentJob.EquipmentCard);
        }
    }

    private static async Task OnJobChanged(PlayerChoiceContext choiceContext, Player player)
    {
        foreach (var power in player.Creature.Powers.OfType<WeaponMasterPower>())
        {
            await CardPileCmd.Draw(choiceContext, power.Amount, player);
        }
    }

    private static void HoldPlayedEquipmentCard(CardModel equipmentCard)
    {
        if (equipmentCard.Pile?.Type == PileType.Play)
        {
            equipmentCard.RemoveFromCurrentPile();
        }
    }

    private static async Task ReturnEquipmentCard(CardModel equipmentCard, PileType pileType)
    {
        if (!pileType.IsCombatPile())
        {
            return;
        }

        var combatState = equipmentCard.Owner.Creature.CombatState;
        if (combatState != null)
        {
            var replacement = combatState.CloneCard(equipmentCard);
            var result = await CardPileCmd.AddGeneratedCardToCombat(replacement, pileType, equipmentCard.Owner);
            CardCmd.PreviewCardPileAdd(result);
        }
    }

}
