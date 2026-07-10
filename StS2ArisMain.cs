using System.Reflection;
using Godot;
using Godot.Bridge;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using StS2Aris.StS2ArisCode.Patches;

namespace StS2Aris;

[ModInitializer(nameof(Initialize))]
public partial class StS2ArisMain : Node
{
    public const string ModId = "StS2Aris"; //Used for resource filepath

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
        new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        Harmony harmony = new(ModId);

        var assembly = Assembly.GetExecutingAssembly();
        ScriptManagerBridge.LookupScriptsInAssembly(assembly);
        harmony.PatchAll();
        BugDescriptionFormatPatch.Register();
    }
}
