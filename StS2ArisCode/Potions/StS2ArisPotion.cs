using BaseLib.Abstracts;
using BaseLib.Utils;
using StS2Aris.StS2ArisCode.Character;

namespace StS2Aris.StS2ArisCode.Potions;

[Pool(typeof(StS2ArisPotionPool))]
public abstract class StS2ArisPotion : CustomPotionModel;