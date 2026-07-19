using MegaCrit.Sts2.Core.Models;
using StS2Aris.StS2ArisCode.Cards;
using StS2Aris.StS2ArisCode.Relics;

namespace StS2Aris.StS2ArisCode.Events;

public sealed record ClassAltarJobDefinition(
    string LocalizationKey,
    Func<RelicModel?> RelicResolver,
    Func<CardModel?>? EquipmentPreviewResolver = null,
    float SpecialOptionChance = 0f);

public static class ClassAltarJobRegistry
{
    private const string HoshinoCardPoolTypeName =
        "StS2Hoshino.StS2HoshinoCode.Character.StS2HoshinoCardPool";

    private static readonly List<ClassAltarJobDefinition> Jobs = [];
    private static bool _defaultsRegistered;

    public static void Register(ClassAltarJobDefinition job)
    {
        int existingIndex = Jobs.FindIndex(existing => existing.LocalizationKey == job.LocalizationKey);
        if (existingIndex >= 0)
        {
            Jobs[existingIndex] = job;
            return;
        }

        Jobs.Add(job);
    }

    public static IReadOnlyList<(ClassAltarJobDefinition Job, RelicModel Relic)> GetAvailableJobs()
    {
        RegisterDefaults();

        List<(ClassAltarJobDefinition, RelicModel)> available = [];
        foreach (ClassAltarJobDefinition job in Jobs)
        {
            RelicModel? relic = job.RelicResolver();
            if (relic != null)
            {
                available.Add((job, relic));
            }
        }

        return available;
    }

    public static CardPoolModel? FindCardPool(string fullTypeName)
    {
        return ModelDb.AllCardPools.FirstOrDefault(pool => pool.GetType().FullName == fullTypeName);
    }

    private static void RegisterDefaults()
    {
        if (_defaultsRegistered)
        {
            return;
        }

        _defaultsRegistered = true;
        Register(new ClassAltarJobDefinition(
            "IRONCLAD", () => ModelDb.Relic<IroncladClassRelic>(), () => ModelDb.Card<Mask>()));
        Register(new ClassAltarJobDefinition(
            "SILENT", () => ModelDb.Relic<SilentClassRelic>(), () => ModelDb.Card<Rogue>()));
        Register(new ClassAltarJobDefinition(
            "DEFECT", () => ModelDb.Relic<DefectClassRelic>(), () => ModelDb.Card<TrueForm>()));
        Register(new ClassAltarJobDefinition(
            "NECROBINDER", () => ModelDb.Relic<NecrobinderClassRelic>(), () => ModelDb.Card<WizardHat>()));
        Register(new ClassAltarJobDefinition(
            "REGENT", () => ModelDb.Relic<RegentClassRelic>(), () => ModelDb.Card<StarThrone>()));
        Register(new ClassAltarJobDefinition(
            "HOSHINO",
            () => FindCardPool(HoshinoCardPoolTypeName) == null ? null : ModelDb.Relic<HoshinoClassRelic>(),
            () => ModelDb.Card<HoshinoTank>()));
        Register(new ClassAltarJobDefinition(
            "KEI",
            () => ModelDb.Relic<KeiClassRelic>(),
            () => ModelDb.Card<LuminousNova>(),
            0.15f));
    }
}
