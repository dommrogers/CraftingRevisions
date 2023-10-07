using HarmonyLib;
using Il2CppTLD.Cooking;
using Il2CppTLD.Gear;
using System.Text.RegularExpressions;

namespace CraftingRevisions
{
	[HarmonyPatch]
	public static class BlueprintManager
	{
		private static HashSet<string> jsonUserBlueprints = new();

		public static void AddBlueprintFromJson(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
			{
				throw new ArgumentException("Blueprint text contains no information", nameof(text));
			}

			// add the blueprint to the HasSet
			jsonUserBlueprints.Add(text);
		}

		internal static void ValidateJsonBlueprint(string json)
		{
			ModUserBlueprintData testBluePrint = ModUserBlueprintData.ParseFromJson(json);
			testBluePrint.Validate();
		}

		// patched into postfix BlueprintManager.LoadAllUserBlueprints
		// (BlueprintManager.RemoveUserBlueprints is not guaranteed to get called)
		[HarmonyPostfix]
		[HarmonyPatch(typeof(Il2CppTLD.Gear.BlueprintManager), nameof(Il2CppTLD.Gear.BlueprintManager.LoadAllUserBlueprints))]
		private static void BlueprintManager_LoadAllUserBlueprints_Postfix(Il2CppTLD.Gear.BlueprintManager __instance)
		{
			// loop over the items
			foreach (string jsonUserBlueprint in jsonUserBlueprints)
			{


					ModUserBlueprintData blueprint = ModUserBlueprintData.ParseFromJson(jsonUserBlueprint);
				try
				{

					bool isValid = blueprint.Validate();

					if (isValid)
					{
						BlueprintData newBlueprint = blueprint.GetBlueprintData();

						// store the processed recipe
						__instance.m_AllBlueprints.Add(newBlueprint);
						Logger.Log("Added Blueprint " + blueprint.Name);
					}
				} catch (Exception e)
				{
					Logger.LogError("Blueprint Exception" + blueprint.Name +"\n"+ e);
				}
			}
		}
	}
}