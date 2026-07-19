using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Aris.StS2ArisCode.Character;
using StS2Aris.StS2ArisCode.CardModels;
using StS2Aris.StS2ArisCode.Keywords;
using StS2Aris.StS2ArisCode.Mechanics;
using StS2Aris.StS2ArisCode.Powers;

namespace StS2Aris.StS2ArisCode.Cards;
[Pool(typeof(StS2ArisCardPool))]
public class PrismaticBeams() : StS2ArisCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy), IOverload
{
    private const string BeamScenePath = "res://scenes/vfx/monsters/kin_priest_beam_vfx.tscn";
    private const string BeamSfx = "event:/sfx/enemy/enemy_attacks/the_kin_priest/the_kin_priest_soul_beam";
    private const float BeamDamageDelay = 0.18f;
    private const float BeamCleanupDelay = 0.8f;
    private const int MaxBeamUseCount = 5;
    private const float BeamScalePerUse = 1.5f;

    private decimal _temporaryDamageBonus;
    private int _beamUseCount;

    protected override IEnumerable<string> ExtraRunAssetPaths => [BeamScenePath];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(ArisKeywords.Overload)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7, ValueProp.Move),
        new DynamicVar("Magic", 3m)
    ];

    protected override async Task OnArisPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (play.Target == null)
            return;

        _beamUseCount = Math.Min(_beamUseCount + 1, MaxBeamUseCount);
        float beamThicknessScale = 1f + (_beamUseCount - 1) * BeamScalePerUse;

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardCompat(this, play)
            .WithAttackerAnim("Cast", 0.5f)
            .Targeting(play.Target)
            .BeforeDamage(async () =>
            {
                NCreature? sourceNode = NCombatRoom.Instance?.GetCreatureNode(play.Player.Creature);
                NCreature? targetNode = NCombatRoom.Instance?.GetCreatureNode(play.Target);
                if (sourceNode == null || targetNode == null)
                    return;

                NKinPriestBeamVfx beam = PreloadManager.Cache.GetScene(BeamScenePath)
                    .Instantiate<NKinPriestBeamVfx>(PackedScene.GenEditState.Disabled);
                beam.GlobalPosition = sourceNode.VfxSpawnPosition;
                float direction = targetNode.GlobalPosition.X >= sourceNode.GlobalPosition.X ? 1f : -1f;
                beam.Scale = new Vector2(-direction, beamThicknessScale);
                NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(beam);

                await beam.AwaitProcessFrame();
                FireStraightBeam(beam);
                SfxCmd.Play(BeamSfx);
                beam.GetTree().CreateTimer(BeamCleanupDelay).Timeout += () => beam.QueueFreeSafely();
                await Cmd.Wait(BeamDamageDelay);
            })
            .WithHitFx("vfx/vfx_starry_impact")
            .Execute(choiceContext);
    }

    private static void FireStraightBeam(NKinPriestBeamVfx beam)
    {
        Node2D beamHolder = beam.GetNode<Node2D>("BeamHolder");
        GpuParticles2D staticParticles = beam.GetNode<GpuParticles2D>("BeamHolder/StaticParticles");

        beam.RotationDegrees = 0f;
        staticParticles.Restart();
        staticParticles.Visible = true;
        beamHolder.Visible = true;
        beamHolder.Scale = Vector2.One;

        Tween lengthTween = beam.CreateTween();
        lengthTween.TweenProperty(beamHolder, "scale:x", 4f, 0.38f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Expo);
        lengthTween.Chain().TweenProperty(beamHolder, "scale:x", 0.5f, 0.6f)
            .SetEase(Tween.EaseType.In)
            .SetTrans(Tween.TransitionType.Expo);
        lengthTween.TweenCallback(Callable.From(() =>
        {
            staticParticles.Emitting = false;
            staticParticles.Visible = false;
            beamHolder.Visible = false;
        }));
    }

    public Task OnOverload(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var increase = DynamicVars["Magic"].BaseValue;
        DynamicVars.Damage.BaseValue += increase;
        _temporaryDamageBonus += increase;
        Owner.PlayerCombatState?.RecalculateCardValues();
        return Task.CompletedTask;
    }

    public override Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side == CombatSide.Player && participants.Contains(Owner.Creature))
        {
            _beamUseCount = 0;
            if (_temporaryDamageBonus == 0m)
                return Task.CompletedTask;

            DynamicVars.Damage.BaseValue -= _temporaryDamageBonus;
            _temporaryDamageBonus = 0m;
            Owner.PlayerCombatState?.RecalculateCardValues();
        }

        return Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["Magic"].UpgradeValueBy(1m);
    }
}
