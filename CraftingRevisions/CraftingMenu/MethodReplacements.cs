using Il2Cpp;
using Il2CppTLD.Gear;
using UnityEngine;

namespace CraftingRevisions.CraftingMenu
{
	internal static class MethodReplacements
	{
		public static bool ItemPassesFilter(Panel_Crafting __instance, BlueprintData bpi)
		{
			switch ((ModCraftingCategory)__instance.m_CurrentCategory)
			{
				case ModCraftingCategory.All:
					return true;
				case ModCraftingCategory.FireStarting:
					return bpi.m_CraftedResult.IsGearType(GearType.Firestarting);
				case ModCraftingCategory.FirstAid:
					return bpi.m_CraftedResult.IsGearType(GearType.FirstAid);
				case ModCraftingCategory.Clothing:
					return bpi.m_CraftedResult.IsGearType(GearType.Clothing);
				case ModCraftingCategory.Tools:
					return bpi.m_CraftedResult.IsGearType(GearType.Tool) || bpi.m_CraftedResult.IsGearType(GearType.Other);
				case ModCraftingCategory.Materials:
					return bpi.m_CraftedResult.IsGearType(GearType.Material);
				case ModCraftingCategory.Food:
					return bpi.m_CraftedResult.IsGearType(GearType.Food);
				case ModCraftingCategory.Craftable:
					return __instance.CanCraftBlueprint(bpi);
				default:
					return false;
			}
		}

		public static void HandleInput(Panel_Crafting __instance)
		{
			if (InputManager.GetEscapePressed(__instance))
			{
				__instance.OnBackButton();
				return;
			}
			float menuMovementVertical;
			if (Il2Cpp.Utils.IsGamepadActive())
			{
				if (InputManager.GetOpenActionsPanelPressed(__instance))
				{
					__instance.OnBackButton();
					return;
				}
				if (InputManager.GetAltFirePressed(__instance))
				{
					//__instance.OnInventoryNav();
					return;
				}
				if (InputManager.GetFirePressed(__instance))
				{
					if (InterfaceManager.IsUsingSurvivalTabs())
					{
						//__instance.OnJournalNav();
						return;
					}
					//__instance.OnMissionNav();
					return;
				}
				else
				{
					if (__instance.m_CurrentNavArea == Panel_Crafting.NavArea.Category)
					{
						if (InputManager.GetContinuePressed(__instance))
						{
							__instance.SetNavigationArea(Panel_Crafting.NavArea.Blueprint);
							return;
						}
					}
					else
					{
						if (InputManager.GetInventoryExaminePressed(__instance))
						{
							__instance.OnBeginCrafting();
							return;
						}
						if (InputManager.GetInventoryFilterLeftPressed(__instance))
						{
							__instance.m_RequirementContainer.OnPrevious();
							return;
						}
						if (InputManager.GetInventoryFilterRightPressed(__instance))
						{
							__instance.m_RequirementContainer.OnNext();
							return;
						}
						if (InputManager.GetInventoryDropPressed(__instance))
						{
							__instance.m_RequirementContainer.HandleNavigation();
							return;
						}
						if (InputManager.GetInventorySortPressed(__instance))
						{
							Panel_Crafting.Filter filter = __instance.m_CurrentFilter + 1;
							if (filter >= Panel_Crafting.Filter.Count)
							{
								filter = Panel_Crafting.Filter.All;
							}
							__instance.SetFilter(filter);
							return;
						}
					}
					menuMovementVertical = Il2Cpp.Utils.GetMenuMovementVertical(__instance, true, true);
				}
			}
			else
			{
				float axisScrollWheel = InputManager.GetAxisScrollWheel(__instance);
				int numBlueprintDisplays = __instance.m_Blueprints.Count; // 7
				int numFilteredItems = __instance.m_FilteredBlueprints.Count; // total number of blueprints in that filtered list

				/*
				if (!Il2Cpp.Utils.IsZero(axisScrollWheel, 0.0001f) && numFilteredItems > numBlueprintDisplays)
				{
					int maxChange = Mathf.Clamp(Settings.instance.numCraftingSteps, 1, numBlueprintDisplays);
					int num = __instance.m_CurrentBlueprintDisplayOffset;
					num += ((axisScrollWheel < 0f) ? maxChange : (-maxChange));
					num = Mathf.Clamp(num, 0, numFilteredItems - numBlueprintDisplays);
					if (num != __instance.m_CurrentBlueprintIndex)
					{
						__instance.m_CurrentBlueprintIndex += num - __instance.m_CurrentBlueprintDisplayOffset;
						__instance.m_CurrentBlueprintDisplayOffset = num;
						__instance.RefreshBlueprintDisplayList();
						__instance.RefreshSelectedBlueprint();
					}
					return;
				}*/
				menuMovementVertical = Il2Cpp.Utils.GetMenuMovementVertical(__instance, true, true);
				if (__instance.m_CurrentNavArea != Panel_Crafting.NavArea.Blueprint)
				{
					__instance.SetNavigationArea(Panel_Crafting.NavArea.Blueprint);
				}
			}
			if (!Il2Cpp.Utils.IsZero(menuMovementVertical, 0.0001f))
			{
				//__instance.HandleVerticalNavigation(menuMovementVertical);
			}
		}
	}
}
