using CraftingRevisions.Patches;
using Harmony;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppTLD.Cooking;
using Il2CppTLD.Gear;
using System.Text;
using System.Text.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CraftingRevisions
{
	internal sealed class ModUserRecipe
	{
		/// <summary>
		/// optional name, used for debugging
		/// </summary>
		/// 

		public string? RecipeName { get; set; } = null;
		public string? RecipeShortName { get; set; } = null;
		public string? RecipeDescription { get; set; } = null;
		public string? RecipeIcon { get; set; } = null;
		public int RequiredSkillLevel { get; set; } = 1;
		public List<string> AllowedCookingPots { get; set; } = new();
		public ModUserRecipeBlueprintData BlueprintData { get; set; } = new();


		private GearItemData[] GetAllowedCookingPots()
		{
			List<GearItemData> list = new();

//			Logger.LogBlue("GetAllowedCookingPots");

			foreach (string AllowedCookingPot in AllowedCookingPots)
			{
				if(AllowedCookingPot == "GEAR_RecycledCan" || AllowedCookingPot == null)
				{
					list.Add(null);
					continue;
				}
//				Logger.LogBlue("GetAllowedCookingPots " + AllowedCookingPot);
				GameObject go = Addressables.LoadAssetAsync<GameObject>(AllowedCookingPot).WaitForCompletion();
				if (go != null)
				{
//					Logger.LogBlue("GetAllowedCookingPots GO " + go.name);
					GearItem gi = go.GetComponent<GearItem>();
					if (gi != null && gi.GearItemData != null)
					{
//						Logger.LogBlue("GetAllowedCookingPots GI/GID " + gi.name + "/" + gi.GearItemData.name);
						list.Add(gi.GearItemData);
					}
				}
			}

			Il2CppReferenceArray<GearItemData> arr = new Il2CppReferenceArray<GearItemData>(list.Count);
			int i = 0;
			foreach (GearItemData gid in list)
			{
				arr[i] = gid;
				i++;
			}

			return arr;
		}

		public RecipeData GetRecipeData()
		{

			AssetReferenceTexture2D ico = new AssetReferenceTexture2D(RecipeIcon);

			RecipeData recipe = ScriptableObject.CreateInstance<RecipeData>();
			recipe.name = "MODRECIPE_"+RecipeName;
			recipe.m_RecipeName = new LocalizedString() { m_LocalizationID = RecipeName };
			recipe.m_RecipeShortName = new LocalizedString() { m_LocalizationID = RecipeShortName };
			recipe.m_RecipeDescription = new LocalizedString() { m_LocalizationID = RecipeDescription };
			recipe.m_RecipeIcon = ico;
			recipe.m_UnlockRule = RecipeData.UnlockType.Unlocked;
			recipe.m_RequiredSkillLevel = RequiredSkillLevel;
			recipe.m_AllowedCookingPots = GetAllowedCookingPots();
			recipe.m_DishBlueprint = GetBlueprintData();
			return recipe;
		}

		private BlueprintData GetBlueprintData()
		{
			ModUserRecipeBlueprintData mubd = BlueprintData;
			BlueprintData bp = ScriptableObject.CreateInstance<BlueprintData>();

			bp.name = "MOD_BLUEPRINT_" + RecipeName;

			bp.m_RequiredGear = Utils.GetRequiredGearItems(BlueprintData.RequiredGear);
			bp.m_RequiredPowder = Utils.GetRequiredPowder(BlueprintData.RequiredPowder);
			bp.m_RequiredLiquid = Utils.GetRequiredLiquid(BlueprintData.RequiredLiquid);

			bp.m_CraftedResult = Addressables.LoadAssetAsync<GameObject>(mubd.CraftedResult).WaitForCompletion().GetComponent<GearItem>();
			bp.m_CraftedResultCount = mubd.CraftedResultCount;
			bp.m_DurationMinutes = mubd.DurationMinutes;
			bp.m_CraftingAudio = Utils.MakeAudioEvent(mubd.CraftingAudio);

			bp.m_RequiresLight = false;
			bp.m_RequiresLitFire = true;
			bp.m_RequiredCraftingLocation = CraftingLocation.Anywhere;
			bp.m_AppliedSkill = SkillType.Cooking;
			bp.m_ImprovedSkill = SkillType.Cooking;
//			bp.m_CraftingIcon = new AssetReferenceTexture2D(RecipeIcon);

			//			bp.m_proxy = Addressables.LoadAssetAsync<GameObject>(mubd.CraftedResult).WaitForCompletion();
			bp.m_CanIncreaseRepairSkill = false;


			return bp;
		}


		

		#region Json
		public static ModUserRecipe ParseFromJson(string jsonText)
		{
			ModUserRecipe recipe = JsonSerializer.Deserialize<ModUserRecipe>(jsonText) ?? throw new ArgumentException("Could not parse recipe data from the text.", nameof(jsonText));
			recipe.RecipeShortName = recipe.RecipeName;
			return recipe;
		}
		#endregion

		#region validation
		internal bool Validate()
		{
			StringBuilder sb = new StringBuilder();

			if (string.IsNullOrWhiteSpace(RecipeName))
				sb.AppendLine($"RecipeName must be set on '{RecipeName}'");

			if (RequiredSkillLevel < 1)
				sb.AppendLine($"RequiredSkillLevel cannot be less than 1 on '{RecipeName}'");

			if (string.IsNullOrWhiteSpace(BlueprintData.CraftedResult))
				sb.AppendLine($"CraftedResult must be set on '{RecipeName}'");

			if (BlueprintData.CraftedResult != null && Addressables.LoadAssetAsync<GameObject>(BlueprintData.CraftedResult).WaitForCompletion().GetComponent<GearItem>() == null)
			{
				sb.AppendLine($"CraftedResult ({BlueprintData.CraftedResult}) is not a valid GearItem on '{RecipeName}'");
			}

			if (BlueprintData.CraftedResultCount < 1)
				sb.AppendLine($"CraftedResultCount cannot be less than 1 on '{RecipeName}'");

			if (BlueprintData.DurationMinutes < 0)
				sb.AppendLine($"DurationMinutes cannot be negative on '{RecipeName}'");


			if (
				(
				BlueprintData.RequiredGear == null
				&& BlueprintData.RequiredLiquid == null
				&& BlueprintData.RequiredPowder == null
				)
				||
				(
				BlueprintData.RequiredGear != null && BlueprintData.RequiredGear.Count == 0
				&& BlueprintData.RequiredLiquid != null && BlueprintData.RequiredLiquid.Count == 0
				&& BlueprintData.RequiredPowder != null && BlueprintData.RequiredPowder.Count == 0
				)
				)
			{
				sb.AppendLine($"One of RequiredGear/RequiredLiquid/RequiredPowder must be defined on '{RecipeName}'");
			}


			if (BlueprintData.RequiredGear != null)
			{
				int i = 0;
				foreach (ModRequiredGearItem RequiredGearItem in BlueprintData.RequiredGear)
				{
					if (RequiredGearItem.Item != null)
					{
						if (Addressables.LoadAssetAsync<GameObject>(RequiredGearItem.Item).WaitForCompletion().GetComponent<GearItem>() == null)
						{
							sb.AppendLine($"RequiredGearItem[{i}].Item ({RequiredGearItem.Item}) is not a valid GearItem on '{RecipeName}'");
						}
					}
					if (RequiredGearItem.Item == null)
						sb.AppendLine($"RequiredGearItem[{i}].Item must be set on '{RecipeName}'");
					if (RequiredGearItem.Count == 0 && RequiredGearItem.Quantity == 0)
						sb.AppendLine($"RequiredGearItem[{i}].Count or RequiredGearItem[{i}].Quantity must be defined on '{RecipeName}'");
					if (RequiredGearItem.Count > 0 && RequiredGearItem.Quantity > 0)
						sb.AppendLine($"RequiredGearItem[{i}].Count and RequiredGearItem[{i}].Quantity are both > 0, only one must be used on '{RecipeName}'");
					if (RequiredGearItem.Quantity > 0 && RequiredGearItem.Units == Il2CppTLD.Gear.BlueprintData.RequiredGearItem.Units.Count)
						sb.AppendLine($"RequiredGearItem[{i}].Quantity is > 0  but RequiredGearItem[{i}].Units is set to Count, is this intended? on '{RecipeName}'");
					i++;
				}
			}
			if (BlueprintData.RequiredLiquid != null && BlueprintData.RequiredLiquid.Count > 0)
			{
				int i = 0;
				foreach (ModRequiredLiquid RequiredLiquidItem in BlueprintData.RequiredLiquid)
				{
					if (RequiredLiquidItem.Liquid != null)
					{
						if (Addressables.LoadAssetAsync<LiquidType>(RequiredLiquidItem.Liquid).WaitForCompletion() == null)
						{
							sb.AppendLine($"RequiredLiquidItem[{i}].Liquid ({RequiredLiquidItem.Liquid}) is not a valid LiquidType on '{RecipeName}'");
						}
					}
					if (RequiredLiquidItem.Liquid == null)
						sb.AppendLine($"RequiredLiquidItem[{i}].Liquid must be set on '{RecipeName}'");
					if (RequiredLiquidItem.VolumeInLitres < 0)
						sb.AppendLine($"RequiredLiquidItem[{i}].VolumeInLitres cannot be less than 0 on '{RecipeName}'");
					i++;
				}
			}
			if (BlueprintData.RequiredPowder != null && BlueprintData.RequiredPowder.Count > 0)
			{
				int i = 0;
				foreach (ModRequiredPowder RequiredPowderItem in BlueprintData.RequiredPowder)
				{
					if (RequiredPowderItem.Powder != null)
					{
						if (Addressables.LoadAssetAsync<PowderType>(RequiredPowderItem.Powder).WaitForCompletion() == null)
						{
							sb.AppendLine($"RequiredPowderItem[{i}].Powder ({RequiredPowderItem.Powder}) is not a valid PowderType on '{RecipeName}'");
						}
					}
					if (RequiredPowderItem.Powder == null)
						sb.AppendLine($"RequiredPowderItem[{i}].Powder must be set on '{RecipeName}'");
					if (RequiredPowderItem.QuantityInKilograms < 0)
						sb.AppendLine($"RequiredPowderItem[{i}].QuantityInKilograms cannot be less than 0 on '{RecipeName}'");
					i++;
				}
			}
			if (sb.Length > 0)
			{
				Logger.LogError("\n" + sb.ToString().Trim());
				return false;
			}
			return true;

		}
		#endregion
		
	}
}