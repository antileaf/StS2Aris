using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using Godot;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Aris.StS2ArisCode.Character;
using StS2Aris.StS2ArisCode.Keywords;
using StS2Aris.StS2ArisCode.Mechanics;
using StS2Aris.StS2ArisCode.Powers;
using StS2Aris.StS2ArisCode.Utils;
using StS2Aris.StS2ArisCode.Vfx;

namespace StS2Aris.StS2ArisCode.Cards;
[Pool(typeof(StS2ArisCardPool))]
public class ArisStrike() : StS2ArisCard(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
{
    protected override IEnumerable<string> ExtraRunAssetPaths =>
    [
        NEnergyProjectionVfx.OrbAtlasPath,
        NEnergyProjectionVfx.ImpactScenePath,
        NShivThrowVfx.scenePath,
        NSmallMagicMissileVfx.scenePath
    ];

    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6, ValueProp.Move)];

    protected override async Task OnArisPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        Creature target = play.Target;
        var player = play.GetPlayer();

        var attack = DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCardCompat(this, play)
            .Targeting(target);

        switch (ArisEquipment.GetCurrentJob(player))
        {
            case JobAoePower:
            case JobAtrahasisSuperNovaPower:
                ConfigureEnergyProjection(attack, player, target);
                break;
            case JobWarriorPower:
            case JobHeroPower:
            case JobWizardPower:
                attack.WithHitFx("vfx/vfx_attack_slash");
                break;
            case JobRoguePower:
                attack.WithHitVfxNode(target =>
                    NShivThrowVfx.Create(player.Creature, target, Colors.Green));
                break;
            case JobKingPower:
                attack.BeforeDamage(() => PlaySmallMagicMissile(target));
                break;
            case JobHoshinoTankPower:
                attack.BeforeDamage(() =>
                {
                    HoshinoReflection.PlayStrikeSfx();
                    return Task.CompletedTask;
                });
                break;
            default:
                attack.WithHitFx("vfx/vfx_attack_blunt");
                break;
        }

        await attack.Execute(choiceContext);
    }

    private static void ConfigureEnergyProjection(
        AttackCommand attack,
        MegaCrit.Sts2.Core.Entities.Players.Player player,
        Creature target)
    {
        attack.WithAttackerAnim("Attack2", player.Character.AttackAnimDelay)
            .BeforeDamage(async () =>
            {
                StS2ArisMain.PlayAttackSfx("Aris_laser.mp3".SfxPath());
                NEnergyProjectionVfx? projectile = NEnergyProjectionVfx.Create(player.Creature, target);
                if (projectile != null)
                {
                    NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(projectile);
                }

                await Cmd.Wait(NEnergyProjectionVfx.TravelDuration);
            });
    }

    private static async Task PlaySmallMagicMissile(Creature target)
    {
        NCreature? targetNode = NCombatRoom.Instance?.GetCreatureNode(target);
        if (targetNode == null)
        {
            return;
        }

        NSmallMagicMissileVfx? missile = NSmallMagicMissileVfx.Create(
            targetNode.GetBottomOfHitbox(),
            new Color("50b598"));
        if (missile == null)
        {
            return;
        }

        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(missile);
        await Cmd.Wait(missile.WaitTime);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
