using HarmonyLib;
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
				// load the blueprint into the game
				Regex name_regex = new Regex(@"""Name"":(.*)");
				var match = name_regex.Match(jsonUserBlueprint);
				if (match.Success)
				{
					MelonLoader.MelonLogger.Msg("loading blueprint " + match.Groups[1].Value);
				}
				bool loaded = __instance.LoadUserBlueprint(jsonUserBlueprint);

				if (!loaded)
				{
					// validate the blueprint
					ValidateJsonBlueprint(jsonUserBlueprint);
				}
			}
		}
	}
}