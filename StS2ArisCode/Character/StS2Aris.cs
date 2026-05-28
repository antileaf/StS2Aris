using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using StS2Aris.StS2ArisCode.Cards;
using StS2Aris.StS2ArisCode.Extensions;
using StS2Aris.StS2ArisCode.Relics;

namespace StS2Aris.StS2ArisCode.Character;

public class StS2Aris : PlaceholderCharacterModel
{
    public const string ModId = "StS2Aris";
    public const string CharacterId = "StS2Aris";

    public static readonly Color Color = new("0a0ac8");

    public override Color NameColor => Color;
    public override Color MapDrawingColor => Color;
    public override CharacterGender Gender => CharacterGender.Feminine;
    public override int StartingHp => 80;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<ArisStrike>(),
        ModelDb.Card<ArisStrike>(),
        ModelDb.Card<ArisStrike>(),
        ModelDb.Card<ArisStrike>(),
        ModelDb.Card<ArisDefend>(),
        ModelDb.Card<ArisDefend>(),
        ModelDb.Card<ArisDefend>(),
        ModelDb.Card<ArisDefend>(),
        ModelDb.Card<SuperNova>(),
        ModelDb.Card<EnergyCharge>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<ArisBaseRelic>()
    ];

    public override CardPoolModel CardPool => ModelDb.CardPool<StS2ArisCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<StS2ArisRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<StS2ArisPotionPool>();

    public override float AttackAnimDelay => 0.15f;
    public override float CastAnimDelay => 0.25f;

    public override string CustomIconTexturePath => "character_icon_aris.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "char_select_aris.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "char_select_aris_locked.png".CharacterUiPath();
    public override string CustomMapMarkerPath => "map_marker_aris.png".CharacterUiPath();
    public override string CustomVisualPath => "res://StS2Aris/scenes/char_aris.tscn";
    public override string CustomCharacterSelectBg => "res://StS2Aris/scenes/char_select_bg_aris.tscn";
    public override string CustomRestSiteAnimPath => "res://StS2Aris/scenes/aris_rest_site.tscn";
    public override string CustomMerchantAnimPath => "res://StS2Aris/scenes/aris_merchant.tscn";
}
