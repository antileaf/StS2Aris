using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Aris.StS2ArisCode.Character;
using StS2Aris.StS2ArisCode.Mechanics;

namespace StS2Aris.StS2ArisCode.Cards;

[Pool(typeof(StS2ArisCardPool))]
public class AwakeningSuperNovaAttack() : StS2ArisCard(0, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
{
    protected override bool HasEnergyCostX => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5, ValueProp.Move),
        new CalculationBaseVar(0m),
        new CalculationExtraVar(1m),
        new CalculatedVar("CalculatedHits").WithMultiplier((card, _) => CalculateExpectedHits(card))
    ];

    protected override async Task OnArisPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (CombatState == null)
        {
            return;
        }

        try
        {
            var hits = ResolveEnergyXValue() + ArisCharge.GetSpent(this);
            if (hits > 0)
            {
                await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", 0.5f);
                await PlayHyperbeamVfx();
            }

            for (var i = 0; i < hits; i++)
            {
                await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play)
                    .TargetingAllOpponents(CombatState)
                    .Execute(choiceContext);
                await Cmd.Wait(0.1f);
            }
        }
        finally
        {
            ArisCharge.ClearSpent(this);
        }
    }

    private async Task PlayHyperbeamVfx()
    {
        if (CombatState == null)
        {
            return;
        }

        var enemies = CombatState.Enemies.Where(enemy => enemy.IsAlive).ToList();
        if (enemies.Count == 0)
        {
            return;
        }

        var beam = NHyperbeamVfx.Create(Owner.Creature, enemies.Last());
        if (beam != null)
        {
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(beam);
        }

        foreach (var enemy in enemies)
        {
            var impact = NHyperbeamImpactVfx.Create(Owner.Creature, enemy);
            if (impact != null)
            {
                NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(impact);
            }
        }

        await Task.CompletedTask;
    }

    private static decimal CalculateExpectedHits(CardModel card)
    {
        var player = card.Owner;
        var state = player?.PlayerCombatState;
        return player == null || state == null ? 0m : state.Energy + ArisCharge.Get(player) * 2;
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
