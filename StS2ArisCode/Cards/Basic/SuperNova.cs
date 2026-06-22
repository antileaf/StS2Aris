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
public class SuperNova() : StS2ArisEquipmentCard(2, CardType.Attack, CardRarity.Basic, TargetType.AllEnemies)
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
        new DamageVar(8, ValueProp.Move),
        new PowerVar<StrengthPower>(3m)
    ];

    public override ArisJobPower CreateJobPower()
    {
        return MakeJobPower<JobAoePower>();
    }

    protected override async Task OnClassChange(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (CombatState == null)
        {
            return;
        }

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(CombatState)
            .WithAttackerAnim("Cast", 0.5f)
            .BeforeDamage(async () =>
            {
                var targets = CombatState.HittableEnemies.ToList();
                var vfx = NSweepingBeamVfx.Create(Owner.Creature, targets);
                if (vfx != null)
                {
                    NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(vfx);
                    await Cmd.Wait(0.5f);
                }
            })
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["StrengthPower"].UpgradeValueBy(1m);
    }
}
