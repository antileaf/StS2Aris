using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.TestSupport;

namespace StS2Aris.StS2ArisCode.Vfx;

public partial class NEnergyProjectionVfx : Node2D
{
    public const string OrbAtlasPath = "res://animations/monsters/kin_priest/kin_priest.png";
    public const string ImpactScenePath = "res://scenes/vfx/monsters/kin_priest_grenade_vfx.tscn";
    public const float TravelDuration = 0.1f;
    public const float FastDuration = 0.03f;

    private static readonly Rect2 OrbRegion = new(833f, 279f, 58f, 58f);

    private Vector2 _startPosition;
    private Vector2 _endPosition;
    private Creature _target = null!;
    private Sprite2D _orb = null!;
    private Sprite2D _glow = null!;

    public static NEnergyProjectionVfx? Create(Creature source, Creature target)
    {
        if (TestMode.IsOn)
            return null;

        NCreature? sourceNode = NCombatRoom.Instance?.GetCreatureNode(source);
        NCreature? targetNode = NCombatRoom.Instance?.GetCreatureNode(target);
        if (sourceNode == null || targetNode == null)
            return null;

        NEnergyProjectionVfx vfx = new()
        {
            _startPosition = sourceNode.VfxSpawnPosition,
            _endPosition = targetNode.GetBottomOfHitbox(),
            _target = target
        };
        vfx.GlobalPosition = vfx._startPosition;
        vfx.BuildVisuals();
        return vfx;
    }

    private void BuildVisuals()
    {
        AtlasTexture orbTexture = new()
        {
            Atlas = PreloadManager.Cache.GetTexture2D(OrbAtlasPath),
            Region = OrbRegion,
            FilterClip = true
        };

        _glow = new Sprite2D
        {
            Texture = orbTexture,
            Scale = Vector2.One * 1.45f,
            Modulate = new Color(0.45f, 1f, 0.7f, 0.3f),
            ZIndex = -1
        };
        AddChild(_glow);

        _orb = new Sprite2D
        {
            Texture = orbTexture,
            Scale = Vector2.One * 0.8f
        };
        AddChild(_orb);

    }

    public override void _Ready()
    {
        TaskHelper.RunSafely(Play());
    }

    private async Task Play()
    {
        float elapsed = 0f;
        while (elapsed < TravelDuration)
        {
            float progress = Mathf.Clamp(elapsed / TravelDuration, 0f, 1f);
            float acceleratedProgress = progress * progress;
            GlobalPosition = _startPosition.Lerp(_endPosition, acceleratedProgress);
            float delta = await this.AwaitProcessFrame();
            _orb.Rotation += 8f * delta;
            elapsed += delta;
        }

        GlobalPosition = _endPosition;
        _orb.Visible = false;
        _glow.Visible = false;

        NKinPriestGrenadeVfx? impact = NKinPriestGrenadeVfx.Create(_target);
        if (impact != null)
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(impact);

        await Cmd.Wait(0.3f);
        this.QueueFreeSafely();
    }
}
