using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using StS2Aris.StS2ArisCode.Character;
using StS2Aris.StS2ArisCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Aris.StS2ArisCode.CardModels;
using StS2Aris.StS2ArisCode.Formatters;
using StS2Aris.StS2ArisCode.Keywords;
using StS2Aris.StS2ArisCode.Mechanics;
using StS2Aris.StS2ArisCode.Powers;

namespace StS2Aris.StS2ArisCode.Cards;

[Pool(typeof(StS2ArisCardPool))]
public abstract class StS2ArisCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{
    private bool _isGlitchReplay;

    protected override bool ShouldGlowGoldInternal =>
        Owner != null
        && ArisCharge.WillBeOverloadAfterSpending(this)
        && (this is IOverload || Owner.Creature.GetPower<SystemOverloadPower>() != null);

    protected virtual Task OnArisPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        return Task.CompletedTask;
    }

    protected sealed override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        return OnArisPlayWrapper(choiceContext, play);
    }

    private async Task OnArisPlayWrapper(PlayerChoiceContext choiceContext, CardPlay play)
    {
        bool overloaded = ArisCharge.IsOverloadState(Owner);
        await RunArisPlayOnce(choiceContext, play, overloaded);

        if (_isGlitchReplay || !overloaded || Owner.Creature.GetPower<GlitchPower>() is not { } glitch)
        {
            return;
        }

        _isGlitchReplay = true;
        try
        {
            for (int i = 0; i < glitch.Amount; i++)
            {
                await RunArisPlayOnce(choiceContext, play, overloaded);
            }
        }
        finally
        {
            _isGlitchReplay = false;
        }
    }

    private async Task RunArisPlayOnce(PlayerChoiceContext choiceContext, CardPlay play, bool overloaded)
    {
        await OnArisPlay(choiceContext, play);
        if (!overloaded)
        {
            return;
        }

        if (this is IOverload overloadCard)
        {
            await overloadCard.OnOverload(choiceContext, play);
            ArisCharge.NotifyOverload();
        }

        if (Owner.Creature.GetPower<SystemOverloadPower>() is { } systemOverload)
        {
            await CreatureCmd.GainBlock(Owner.Creature, systemOverload.Amount, ValueProp.Move, play);
            ExhaustOnNextPlay = true;
        }
    }

    protected override void AddExtraArgsToDescription(LocString description)
    {
        description.Add("chargeIcon", ChargeIconFormatter.Icon);
    }

    public override string CustomPortraitPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_p.png".CardImagePath();
            return ResourceLoader.Exists(path) ? path : (Type==CardType.Attack?"temp_attack_p.png":
                (Type==CardType.Power?"temp_power_p.png":"temp_skill_p.png")).CardImagePath();
        }
    }
    public override string PortraitPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
            return ResourceLoader.Exists(path) ? path : (Type==CardType.Attack?"temp_attack.png":
                (Type==CardType.Power?"temp_power.png":"temp_skill.png")).CardImagePath();
        }
    }
}
