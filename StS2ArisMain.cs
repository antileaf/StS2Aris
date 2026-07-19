using System.Reflection;
using BaseLib.Audio;
using BaseLib.Config;
using Godot;
using Godot.Bridge;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using StS2Aris.StS2ArisCode.Config;
using StS2Aris.StS2ArisCode.Patches;

namespace StS2Aris;

[ModInitializer(nameof(Initialize))]
public partial class StS2ArisMain : Node
{
    public const string ModId = "StS2Aris"; //Used for resource filepath
    public static readonly AutoModAudio Audio = new($"res://{ModId}/audio");
    private const float DefaultVolumePercent = 140.0f;

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
        new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        Harmony harmony = new(ModId);

        ModConfigRegistry.Register(ModId, new ArisModConfig());
        var assembly = Assembly.GetExecutingAssembly();
        ScriptManagerBridge.LookupScriptsInAssembly(assembly);
        harmony.PatchAll();
        BugDescriptionFormatPatch.Register();
    }

    public static void PlayAttackSfx(string path, float volumeMult = 1f)
    {
        float configuredMultiplier = ArisModConfig.AttackSfxVolumePercent / DefaultVolumePercent;
        Audio.PlaySfx(path, volumeMult: volumeMult * configuredMultiplier);
    }
}
