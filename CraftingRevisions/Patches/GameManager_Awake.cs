using HarmonyLib;
using Il2Cpp;


namespace CraftingRevisions.Patches
{
	// load the blueprints when the crafting panel is initialized
	// (needed to be this late otherwise we have no instance of BlueprintManager)
	[HarmonyPatch(typeof(Panel_Crafting), nameof(Panel_Crafting.Initialize))]
	internal class Panel_Crafting_Initialize
	{
		private static void Postfix()
		{
			// call TLD LoadAllUserBlueprints
			InterfaceManager.m_Instance.m_BlueprintManager.LoadAllUserBlueprints();
		}
	}

	// These patches are to work around the fact that "UserBlueprintData.MakeRuntimeWwiseEvent" appears to be currently broken or unfinished
	[HarmonyPatch(typeof(Il2CppTLD.Gear.UserBlueprintData), nameof(Il2CppTLD.Gear.UserBlueprintData.MakeRuntimeWwiseEvent), new Type[] { typeof(string) })]
	internal class UserBlueprintData_MakeRuntimeWwiseEvent
	{
		private static void Prefix(string eventName, ref bool __runOriginal, ref Il2CppAK.Wwise.Event? __result)
		{
			__result = Utils.MakeAudioEvent(eventName);
			__runOriginal = false;
		}
	}
}
