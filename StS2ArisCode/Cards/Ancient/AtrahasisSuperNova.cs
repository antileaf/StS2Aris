using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Aris.StS2ArisCode.CardModels;
using StS2Aris.StS2ArisCode.Character;
using StS2Aris.StS2ArisCode.Keywords;
using StS2Aris.StS2ArisCode.Powers;

namespace StS2Aris.StS2ArisCode.Cards;

[Pool(typeof(StS2ArisCardPool))]
public class AtrahasisSuperNova() : StS2ArisEquipmentCard(1, CardType.Attack, CardRarity.Ancient, TargetType.AllEnemies)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(ArisKeywords.Equipment),
        HoverTipFactory.FromKeyword(ArisKeywords.ClassChange),
        HoverTipFactory.FromKeyword(ArisKeywords.Job),
        HoverTipFactory.FromPower<StrengthPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(16, ValueProp.Move),
        new PowerVar<StrengthPower>(5m)
    ];

    public override ArisJobPower CreateJobPower()
    {
        return MakeJobPower<JobAtrahasisSuperNovaPower>();
    }

    protected override async Task OnClassChange(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (CombatState == null)
        {
            return;
        }

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play).TargetingAllOpponents(CombatState)
            .WithAttackerAnim("Cast", 0.5f)
            .BeforeDamage(async () =>
            {
                var enemies = CombatState.Enemies.Where(e => e.IsAlive).ToList();
                if (enemies.Count == 0)
                {
                    return;
                }

                var beam = NHyperbeamVfx.Create(Owner.Creature, enemies.Last());
                if (beam != null)
                {
                    NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(beam);
                    await Cmd.Wait(0.5f);
                }

                foreach (var enemy in enemies)
                {
                    var impact = NHyperbeamImpactVfx.Create(Owner.Creature, enemy);
                    if (impact != null)
                    {
                        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(impact);
                    }
                }
            })
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["StrengthPower"].UpgradeValueBy(2m);
    }
}
