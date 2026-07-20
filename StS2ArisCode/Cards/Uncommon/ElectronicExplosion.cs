using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Aris.StS2ArisCode.Character;
using StS2Aris.StS2ArisCode.Keywords;
using StS2Aris.StS2ArisCode.Mechanics;
using StS2Aris.StS2ArisCode.Powers;
using StS2Aris.StS2ArisCode.Utils;
using StS2Aris.StS2ArisCode.Vfx;

namespace StS2Aris.StS2ArisCode.Cards;
[Pool(typeof(StS2ArisCardPool))]
public class ElectronicExplosion() : StS2ArisCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<string> ExtraRunAssetPaths =>
    [
        NEnergyProjectionVfx.OrbAtlasPath,
        NEnergyProjectionVfx.ImpactScenePath
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(ArisKeywords.Overload)];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5, ValueProp.Move),
        new CalculationBaseVar(0m),
        new CalculationExtraVar(1m),
        new CalculatedVar("CalculatedHits").WithMultiplier((_, _) => ArisCharge.OverloadsThisCombat)
    ];

    protected override async Task OnArisPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target, "cardPlay.Target");
        var player = play.GetPlayer();
        var attack = DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .WithHitCount((int)((CalculatedVar)base.DynamicVars["CalculatedHits"]).Calculate(play.Target))
            .FromCardCompat(this, play)
            .Targeting(play.Target);

        if (ArisEquipment.IsSuperNovaEquipped(player))
            attack.WithAttackerAnim("Attack2", player.Character.AttackAnimDelay);

        await attack
            .BeforeDamage(async () =>
            {
                StS2ArisMain.PlayAttackSfx("Aris_laser.mp3".SfxPath());
                NEnergyProjectionVfx? projectile = NEnergyProjectionVfx.Create(player.Creature, play.Target);
                if (projectile != null)
                    NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(projectile);

                await Cmd.Wait(NEnergyProjectionVfx.FastDuration);
            })
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
