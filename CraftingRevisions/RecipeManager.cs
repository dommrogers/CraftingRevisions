using HarmonyLib;
using Il2Cpp;
using Il2CppTLD.Cooking;
using Il2CppTLD.Gear;
using System.Text.RegularExpressions;
using UnityEngine;

namespace CraftingRevisions
{
	[HarmonyPatch]
	public static class RecipeManager
	{
		private static HashSet<string> jsonUserRecipes = new();
		private static List<RecipeData> userRecipes = new();

		public static void AddRecipeFromJson(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
			{
				throw new ArgumentException("Recipe text contains no information", nameof(text));
			}

			// add the blueprint to the HasSet
			jsonUserRecipes.Add(text);
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(RecipeBook), nameof(RecipeBook.Start))]
		private static void RecipeBook_Start(RecipeBook __instance)
		{

			foreach (string jsonUserRecipe in jsonUserRecipes)
			{
				ModUserRecipe recipe = ModUserRecipe.ParseFromJson(jsonUserRecipe);

				bool isValid = recipe.Validate();

				if (isValid)
				{
					RecipeData newRecipe = recipe.GetRecipeData();

					// store the processed recipe
					__instance.AllRecipes.Add(newRecipe);
					Logger.Log("Added Recipe " + recipe.RecipeName);
				}
			}

		}
	}
}