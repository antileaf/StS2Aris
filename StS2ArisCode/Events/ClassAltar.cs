using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Runs;
using StS2Aris.StS2ArisCode.Cards;
using StS2Aris.StS2ArisCode.Character;
using StS2Aris.StS2ArisCode.Extensions;

namespace StS2Aris.StS2ArisCode.Events;

public sealed class ClassAltar : CustomEventModel
{
    public override ActModel[] Acts => [ModelDb.Act<Overgrowth>(), ModelDb.Act<Underdocks>()];

    public override string CustomInitialPortraitPath => "events/class_altar.png".ImagePath();

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new HealVar(15m)
    ];
    
    public override bool IsAllowed(IRunState runState)
    {
        return runState.CurrentActIndex == 0 &&
               runState.Players.All(player =>
            player.Character is Character.StS2Aris &&
            PileType.Deck.GetPile(player).Cards.Any(card => card is SuperNova));
    }

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        List<(ClassAltarJobDefinition Job, RelicModel Relic)> candidates =
            ClassAltarJobRegistry.GetAvailableJobs().ToList();
        List<(ClassAltarJobDefinition Job, RelicModel Relic)> selected = [];
        List<EventOption> options = [];

        foreach ((ClassAltarJobDefinition job, RelicModel relic) in
                 candidates.Where(candidate => candidate.Job.SpecialOptionChance > 0f))
        {
            if (selected.Count < 3 && Rng.NextFloat() < job.SpecialOptionChance)
            {
                selected.Add((job, relic));
            }
        }

        candidates.RemoveAll(candidate => candidate.Job.SpecialOptionChance > 0f);
        while (selected.Count < 3 && candidates.Count > 0)
        {
            int index = Rng.NextInt(candidates.Count);
            selected.Add(candidates[index]);
            candidates.RemoveAt(index);
        }

        while (selected.Count > 0)
        {
            int index = Rng.NextInt(selected.Count);
            (ClassAltarJobDefinition job, RelicModel relic) = selected[index];
            selected.RemoveAt(index);

            string optionKey = $"{Id.Entry}.pages.INITIAL.options.{job.LocalizationKey}";
            List<IHoverTip> hoverTips = HoverTipFactory.FromRelic(relic).ToList();
            CardModel? equipmentPreview = job.EquipmentPreviewResolver?.Invoke();
            if (equipmentPreview != null)
            {
                hoverTips.Add(HoverTipFactory.FromCard(equipmentPreview));
            }

            options.Add(new EventOption(
                this,
                () => ChooseJob(job, relic),
                optionKey,
                hoverTips));
        }

        options.Add(new EventOption(this, Heal, $"{Id.Entry}.pages.INITIAL.options.HEAL"));
        return options;
    }

    private async Task ChooseJob(ClassAltarJobDefinition job, RelicModel relic)
    {
        await RelicCmd.Obtain(relic.ToMutable(), Owner!);
        SetEventFinished(PageDescription(job.LocalizationKey));
    }

    private async Task Heal()
    {
        await CreatureCmd.Heal(Owner!.Creature, base.DynamicVars.Heal.IntValue);
        SetEventFinished(PageDescription("HEAL"));
    }
}
