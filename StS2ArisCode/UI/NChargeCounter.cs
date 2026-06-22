using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using StS2Aris.StS2ArisCode.Extensions;
using StS2Aris.StS2ArisCode.Formatters;
using StS2Aris.StS2ArisCode.Mechanics;
using StS2Aris.StS2ArisCode.Powers;

namespace StS2Aris.StS2ArisCode.UI;

public partial class NChargeCounter : Control
{
    public enum ChargeCounterPosition
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
        None
    }

    private const ChargeCounterPosition CurrentPosition = ChargeCounterPosition.TopRight;

    private static readonly StringName FontColor = ThemeConstants.Label.FontColor;
    private static readonly StringName FontOutlineColor = ThemeConstants.Label.FontOutlineColor;
    private static readonly StringName FontSize = ThemeConstants.Label.FontSize;
    private static readonly StringName OutlineSize = ThemeConstants.Label.OutlineSize;
    private const string DefaultLabelFontPath = "res://StS2Aris/etc/kreon_bold_shared.tres";
    private static readonly string ChargeIconPath = "aris_charge.png".CharacterUiPath();
    private static readonly string[] ChargeLayerPaths =
    [
        "aris_charge_layer_2.png".CharacterUiPath(),
        "aris_charge_layer_3.png".CharacterUiPath(),
        "aris_charge_layer_4.png".CharacterUiPath(),
        "aris_charge_layer_5.png".CharacterUiPath(),
        "aris_charge_layer_6.png".CharacterUiPath()
    ];
    private const float OverloadLayerInterval = 0.24f;

    private Player? _player;
    private ChargePower? _power;
    private MegaLabel _label = null!;
    private TextureRect _icon = null!;
    private readonly List<TextureRect> _chargeLayers = [];
    private HoverTip _hoverTip;
    private float _lerpingCount;
    private float _velocity;
    private float _glowCounter;
    private int _displayedCount = -1;

    public static NChargeCounter Create(Player player)
    {
        var counter = new NChargeCounter
        {
            Name = "ArisChargeCounter",
            Size = new Vector2(96f, 96f),
            PivotOffset = new Vector2(48f, 48f),
            _player = player
        };
        counter.ApplyCounterPosition(CurrentPosition);
        return counter;
    }

    public override void _Ready()
    {
        MouseFilter = MouseFilterEnum.Stop;

        _icon = new TextureRect
        {
            Name = "Icon",
            AnchorRight = 1f,
            AnchorBottom = 1f,
            MouseFilter = MouseFilterEnum.Ignore,
            Texture = PreloadManager.Cache.GetTexture2D(ChargeIconPath),
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
            Modulate = new Color(0.35f, 0.75f, 1f)
        };
        AddChild(_icon);
        foreach (var layerPath in ChargeLayerPaths)
        {
            _chargeLayers.Add(AddIconLayer(layerPath));
        }

        _label = new MegaLabel
        {
            Name = "CountLabel",
            AnchorRight = 1f,
            AnchorBottom = 1f,
            OffsetLeft = 10f,
            OffsetTop = 10f,
            OffsetRight = -10f,
            OffsetBottom = -12f,
            MouseFilter = MouseFilterEnum.Ignore,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            AutoSizeEnabled = true,
            MinFontSize = 18,
            MaxFontSize = 30
        };
        _label.AddThemeFontOverride(ThemeConstants.Label.Font, PreloadManager.Cache.GetAsset<Font>(DefaultLabelFontPath));
        _label.AddThemeFontSizeOverride(FontSize, 30);
        _label.AddThemeConstantOverride(OutlineSize, 8);
        AddChild(_label);

        var description = new LocString("static_hover_tips", "STS2ARIS-CHARGE.description");
        description.Add("singleChargeIcon", ChargeIconFormatter.Icon);
        _hoverTip = new HoverTip(new LocString("static_hover_tips", "STS2ARIS-CHARGE.title"), description);

        Connect(SignalName.MouseEntered, Callable.From(OnHovered));
        Connect(SignalName.MouseExited, Callable.From(OnUnhovered));
        Visible = false;
        SyncPower();
        RefreshVisibility();
    }

    public override void _ExitTree()
    {
        DisconnectPower();
        base._ExitTree();
    }

    public override void _Process(double delta)
    {
        if (_player == null)
        {
            Visible = false;
            return;
        }

        SyncPower();
        int charge = ArisCharge.Get(_player);
        _lerpingCount = MathHelper.SmoothDamp(_lerpingCount, charge, ref _velocity, 0.1f, (float)delta);
        SetCountText(Mathf.RoundToInt(_lerpingCount));

        bool overloaded = charge > 0 && ArisCharge.IsOverloadAvailable(_player);
        if (overloaded)
        {
            _glowCounter += (float)delta;
        }
        else
        {
            _glowCounter = 0f;
        }

        float pulse = overloaded ? 0.5f + Mathf.Sin(_glowCounter * 4f) * 0.5f : 0f;
        _icon.Modulate = overloaded
            ? new Color(0.25f + pulse * 0.75f, 0.55f + pulse * 0.45f, 0.75f + pulse * 0.25f)
            : charge == 0
                ? new Color(0.22f, 0.35f, 0.45f, 0.85f)
                : new Color(0.35f, 0.75f, 1f);
        UpdateOverloadLayers(overloaded);

        RefreshVisibility();
    }

    private void SyncPower()
    {
        var current = _player?.Creature.GetPower<ChargePower>();
        if (ReferenceEquals(current, _power))
        {
            return;
        }

        DisconnectPower();
        _power = current;
        if (_power != null)
        {
            _power.DisplayAmountChanged += OnChargeChanged;
            _power.Removed += OnChargeRemoved;
            _lerpingCount = _power.Amount;
            SetCountText(_power.Amount);
        }
    }

    private void DisconnectPower()
    {
        if (_power == null)
        {
            return;
        }

        _power.DisplayAmountChanged -= OnChargeChanged;
        _power.Removed -= OnChargeRemoved;
        _power = null;
    }

    private void OnChargeChanged()
    {
        if (_power != null)
        {
            SetCountText(_power.Amount);
        }
        RefreshVisibility();
    }

    private void OnChargeRemoved()
    {
        DisconnectPower();
        SetCountText(0);
        RefreshVisibility();
    }

    private void SetCountText(int charge)
    {
        if (_displayedCount == charge)
        {
            return;
        }

        _displayedCount = charge;
        _label.AddThemeColorOverride(FontColor, charge == 0 ? StsColors.red : StsColors.cream);
        _label.AddThemeColorOverride(FontOutlineColor, new Color(0.02f, 0.12f, 0.35f));
        _label.Text = charge.ToString();
    }

    private TextureRect AddIconLayer(string texturePath)
    {
        var layer = new TextureRect
        {
            AnchorRight = 1f,
            AnchorBottom = 1f,
            MouseFilter = MouseFilterEnum.Ignore,
            Texture = PreloadManager.Cache.GetTexture2D(texturePath),
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
            Visible = false
        };
        _icon.AddChild(layer);
        return layer;
    }

    private void UpdateOverloadLayers(bool overloaded)
    {
        if (!overloaded)
        {
            foreach (var layer in _chargeLayers)
            {
                layer.Visible = false;
            }
            return;
        }

        var visibleLayerCount = Mathf.FloorToInt(_glowCounter / OverloadLayerInterval) % _chargeLayers.Count + 1;
        for (var i = 0; i < _chargeLayers.Count; i++)
        {
            _chargeLayers[i].Visible = i < visibleLayerCount;
        }
    }

    private void ApplyCounterPosition(ChargeCounterPosition position)
    {
        Position = position switch
        {
            ChargeCounterPosition.TopLeft => new Vector2(-68f, -34f),
            ChargeCounterPosition.TopRight => new Vector2(78f, -34f),
            ChargeCounterPosition.BottomLeft => new Vector2(-68f, 82f),
            ChargeCounterPosition.BottomRight => new Vector2(78f, 82f),
            ChargeCounterPosition.None => Vector2.Zero,
            _ => new Vector2(100f, -14f)
        };

        Visible = position != ChargeCounterPosition.None;
    }

    private void RefreshVisibility()
    {
        if (_player == null)
        {
            Visible = false;
            return;
        }

        int charge = ArisCharge.Get(_player);
        Visible = CurrentPosition != ChargeCounterPosition.None && (Visible || _player.Character is Character.StS2Aris || charge > 0);
    }

    private void OnHovered()
    {
        NHoverTipSet.CreateAndShow(this, _hoverTip)?.SetGlobalPosition(GlobalPosition + new Vector2(-34f, -260f));
    }

    private void OnUnhovered()
    {
        NHoverTipSet.Remove(this);
    }
}
