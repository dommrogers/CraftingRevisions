using HarmonyLib;
using Il2Cpp;
using static Il2Cpp.PlayerManager;

namespace CraftingRevisions.Patches
{
	internal static class WatchHandleCraftingSuccess
	{
		internal static bool isExecuting = false;
	}

	[HarmonyPatch(typeof(Panel_Crafting), nameof(Panel_Crafting.OnCraftingSuccess))]
	internal static class Panel_Crafting_CraftingEnd
	{
		private static void Prefix()
		{
			WatchHandleCraftingSuccess.isExecuting = true;
		}
		private static void Postfix()
		{
			WatchHandleCraftingSuccess.isExecuting = false;
		}
	}
	[HarmonyPatch(typeof(PlayerManager), nameof(PlayerManager.InstantiateItemInPlayerInventory), new Type[] { typeof(GearItem), typeof(int), typeof(float), typeof(InventoryInstantiateFlags) })]
	internal static class PlayerManager_InstantiateItemInPlayerInventory
	{
		private static void Postfix(ref GearItem __result, float normalizedCondition)
		{
			if (WatchHandleCraftingSuccess.isExecuting && normalizedCondition < 0)
			{
				__result.CurrentHP = __result.GearItemData.m_MaxHP;
			}
		}
	}
}
