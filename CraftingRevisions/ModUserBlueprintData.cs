using Il2Cpp;
using Il2CppTLD.Gear;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CraftingRevisions
{
	internal sealed class ModUserBlueprintData
	{
		/// <summary>
		/// optional name, used for debugging
		/// </summary>
		public string? Name { get; set; } = null;
		public List<ModRequiredGearItem> RequiredGear { get; set; } = new();
		public List<ModRequiredPowder>? RequiredPowder { get; set; } = new();
		public List<ModRequiredLiquid>? RequiredLiquid { get; set; } = new();
		public string? RequiredTool { get; set; } = null;
		public List<string>? OptionalTools { get; set; } = new();
		public string? CraftedResult { get; set; } = null;
		public int CraftedResultCount { get; set; } = 0;
		public int DurationMinutes { get; set; } = 0;
		public string? CraftingAudio { get; set; } = null;
		public float? KeroseneLitersRequired { get; set; } = null;
		public float? GunpowderKGRequired { get; set; } = null;
		public bool RequiresLight { get; set; } = false;
		public bool Locked { get; set; } = false;
		public bool IgnoreLockInSurvival { get; set; } = true;
		public bool AppearsInStoryOnly { get; set; } = false;
		public bool AppearsInSurvivalOnly { get; set; } = false;
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public SkillType AppliedSkill { get; set; } = SkillType.None;
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public SkillType ImprovedSkill { get; set; } = SkillType.None;
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public CraftingLocation RequiredCraftingLocation { get; set; } = CraftingLocation.Anywhere;
		public bool RequiresLitFire { get; set; } = false;
		public bool CanIncreaseRepairSkill { get; set; } = false;

		#region Json
		public static ModUserBlueprintData ParseFromJson(string jsonText)
		{
			return JsonSerializer.Deserialize<ModUserBlueprintData>(jsonText) ?? throw new ArgumentException("Could not parse blueprint data from the text.", nameof(jsonText));
		}
		#endregion

		#region validation
		internal bool Validate()
		{
			StringBuilder sb = new StringBuilder();
			StringBuilder sbwarn = new StringBuilder();

			if (string.IsNullOrWhiteSpace(CraftedResult))
				sb.AppendLine($"CraftedResult must be set on '{Name}'");

			if (CraftedResult != null && Addressables.LoadAssetAsync<GameObject>(CraftedResult).WaitForCompletion().GetComponent<GearItem>() == null)
			{
				sb.AppendLine($"CraftedResult ({CraftedResult}) is not a valid GearItem on '{Name}'");
			}

			if (CraftedResultCount < 1)
				sb.AppendLine($"CraftedResultCount cannot be less than 1 on '{Name}'");

			if (DurationMinutes < 0)
				sb.AppendLine($"DurationMinutes cannot be negative on '{Name}'");

			if (!Utils.EnumValues<CraftingLocation>.Contains(RequiredCraftingLocation))
				sb.AppendLine($"Unsupported value {RequiredCraftingLocation} for RequiredCraftingLocation on '{Name}'");

			if (!Utils.EnumValues<SkillType>.Contains(AppliedSkill))
				sb.AppendLine($"Unsupported value {AppliedSkill} for AppliedSkill on '{Name}'");

			if (!Utils.EnumValues<SkillType>.Contains(ImprovedSkill))
				sb.AppendLine($"Unsupported value {ImprovedSkill} for RequiredCraftingLocation on '{Name}'");

			if (
				(
				RequiredGear == null
				//&& RequiredLiquid == null
				&& RequiredPowder == null
				)
				||
				(
				RequiredGear != null && RequiredGear.Count == 0
				//&& RequiredLiquid != null && RequiredLiquid.Count == 0
				&& RequiredPowder != null && RequiredPowder.Count == 0
				)
				)
			{
				//				sb.AppendLine($"One of RequiredGear/RequiredLiquid/RequiredPowder must be defined on '{Name}'");
				sb.AppendLine($"One of RequiredGear/RequiredPowder must be defined on '{Name}'");
			}


			if (RequiredGear != null && RequiredGear.Count > 0)
			{
				int i = 0;
				foreach (ModRequiredGearItem RequiredGearItem in RequiredGear)
				{
					if (RequiredGearItem.Item != null)
					{
						if (Addressables.LoadAssetAsync<GameObject>(RequiredGearItem.Item).WaitForCompletion().GetComponent<GearItem>() == null)
						{
							sb.AppendLine($"RequiredGearItem[{i}].Item ({RequiredGearItem.Item}) is not a valid GearItem on '{Name}'");
						}
					}
					if (RequiredGearItem.Item == null)
						sb.AppendLine($"RequiredGearItem[{i}].Item must be set on '{Name}'");
					if (RequiredGearItem.Count < 1)
						sb.AppendLine($"RequiredGearItem[{i}].Count cannot be less than 1 on '{Name}'");
					i++;
				}
			}
			//if (RequiredLiquid != null && RequiredLiquid.Count > 0)
			//{
			//	int i = 0;
			//	foreach (ModRequiredLiquid RequiredLiquidItem in RequiredLiquid)
			//	{
			//		if (RequiredLiquidItem.Liquid != null)
			//		{
			//			if (Addressables.LoadAssetAsync<LiquidType>(RequiredLiquidItem.Liquid).WaitForCompletion() == null)
			//			{
			//				sb.AppendLine($"RequiredLiquidItem[{i}].Liquid ({RequiredLiquidItem.Liquid}) is not a valid LiquidType on '{Name}'");
			//			}
			//		}
			//		if (RequiredLiquidItem.Liquid == null)
			//			sb.AppendLine($"RequiredLiquidItem[{i}].Liquid must be set on '{Name}'");
			//		if (RequiredLiquidItem.VolumeInLitres < 0)
			//			sb.AppendLine($"RequiredLiquidItem[{i}].VolumeInLitres cannot be less than 0 on '{Name}'");
			//		i++;
			//	}
			//}
			if (RequiredPowder != null && RequiredPowder.Count > 0)
			{
				int i = 0;
				foreach (ModRequiredPowder RequiredPowderItem in RequiredPowder)
				{
					if (RequiredPowderItem.Powder != null)
					{
						if (Addressables.LoadAssetAsync<PowderType>(RequiredPowderItem.Powder).WaitForCompletion() == null)
						{
							sb.AppendLine($"RequiredPowderItem[{i}].Powder ({RequiredPowderItem.Powder}) is not a valid PowderType on '{Name}'");
						}
					}
					if (RequiredPowderItem.Powder == null)
						sb.AppendLine($"RequiredPowderItem[{i}].Powder must be set on '{Name}'");
					if (RequiredPowderItem.QuantityInKilograms < 0)
						sb.AppendLine($"RequiredPowderItem[{i}].QuantityInKilograms cannot be less than 0 on '{Name}'");
					i++;
				}
			}


			// WARNINGS
			if (KeroseneLitersRequired != null && KeroseneLitersRequired > 0)
			{
				//				sbwarn.AppendLine($"KeroseneLitersRequired is obsolete, please use LiquidRequired in future blueprints '{Name}'");
			}

			if (GunpowderKGRequired != null && GunpowderKGRequired > 0)
			{
				sbwarn.AppendLine($"GunpowderKGRequired will be deprecated, please use PowderRequired in future blueprints '{Name}'");
			}
			if (RequiredLiquid != null && RequiredLiquid.Count > 0)
			{
				sbwarn.AppendLine($"RequiredLiquid is not implemented due to Hinterland being lazy devs '{Name}'");
			}

			if (sbwarn.Length > 0)
			{
				MelonLoader.MelonLogger.Warning("\n" + sbwarn.ToString().Trim());
			}

			// is valid ?
			if (sb.Length > 0)
			{
				MelonLoader.MelonLogger.Error("\n" + sb.ToString().Trim());
				return false;
			}

			return true;

		}
		#endregion


		internal BlueprintData GetBlueprintData()
		{

			if (GunpowderKGRequired != null && GunpowderKGRequired > 0)
			{
				RequiredPowder.Add(new ModRequiredPowder() { Powder = "POWDER_Gunpowder", QuantityInKilograms = GunpowderKGRequired ?? 0 });
			}
			//if (KeroseneLitersRequired!= null && KeroseneLitersRequired > 0)
			//{
			//	RequiredLiquid.Add(new ModRequiredLiquid(){ Liquid = "LIQUID_Kerosene", VolumeInLitres = KeroseneLitersRequired ?? 0 });
			//}


			BlueprintData bp = ScriptableObject.CreateInstance<BlueprintData>();

			bp.name = "BP_" + Name;

			bp.m_KeroseneLitersRequired = KeroseneLitersRequired ?? 0;
			//bp.m_GunpowderKGRequired = GunpowderKGRequired ?? 0;

			bp.m_RequiredGear = Utils.GetRequiredGearItems(RequiredGear);
			bp.m_RequiredPowder = Utils.GetRequiredPowder(RequiredPowder);
			//bp.m_RequiredLiquid = Utils.GetRequiredLiquid(RequiredLiquid);

			bp.m_CraftedResult = Addressables.LoadAssetAsync<GameObject>(CraftedResult).WaitForCompletion().GetComponent<GearItem>();
			bp.m_CraftedResultCount = CraftedResultCount;

			bp.m_DurationMinutes = DurationMinutes;
			bp.m_CraftingAudio = Utils.MakeAudioEvent(CraftingAudio);
			//bp.m_CraftingIcon = new AssetReferenceTexture2D(null);

			bp.m_RequiresLight = RequiresLight;
			bp.m_RequiresLitFire = RequiresLitFire;
			bp.m_RequiredCraftingLocation = Utils.TranslateEnumValue<CraftingLocation, CraftingLocation>(RequiredCraftingLocation);
			bp.m_AppliedSkill = Utils.TranslateEnumValue<SkillType, SkillType>(AppliedSkill);
			bp.m_ImprovedSkill = Utils.TranslateEnumValue<SkillType, SkillType>(ImprovedSkill);

			bp.m_Locked = false;
			bp.m_AppearsInStoryOnly = false;

//			bp.m_proxy = bp.game;
			bp.m_CanIncreaseRepairSkill = false;

			return bp;
		}

	}
}