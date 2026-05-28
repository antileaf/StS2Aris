using BaseLib.Abstracts;
using StS2Aris.StS2ArisCode.Extensions;
using Godot;

namespace StS2Aris.StS2ArisCode.Character;

public class StS2ArisPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => StS2Aris.Color;


    public override string BigEnergyIconPath => "charui/aris_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/aris_text_energy.png".ImagePath();
}