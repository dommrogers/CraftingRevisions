using Il2Cpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Il2CppTLD.Gear;

namespace CraftingRevisions
{
	internal class Utils
	{

		private static readonly Dictionary<string, uint> eventIds = new();


		internal static Il2CppAK.Wwise.Event? MakeAudioEvent(string? eventName)
		{
			if (string.IsNullOrEmpty(eventName) || GetAKEventIdFromString(eventName) == 0)
			{
				Il2CppAK.Wwise.Event emptyEvent = new();
				emptyEvent.WwiseObjectReference = ScriptableObject.CreateInstance<WwiseEventReference>();
				emptyEvent.WwiseObjectReference.objectName = "NULL_WWISEEVENT";
				emptyEvent.WwiseObjectReference.id = GetAKEventIdFromString("NULL_WWISEEVENT");
				return emptyEvent;
			}

			Il2CppAK.Wwise.Event newEvent = new();
			newEvent.WwiseObjectReference = ScriptableObject.CreateInstance<WwiseEventReference>();
			newEvent.WwiseObjectReference.objectName = eventName;
			newEvent.WwiseObjectReference.id = GetAKEventIdFromString(eventName);
			return newEvent;
		}
		private static uint GetAKEventIdFromString(string eventName)
		{
			if (eventIds.Count == 0)
			{
				Type type = typeof(Il2CppAK.EVENTS);
				foreach (PropertyInfo prop in type.GetProperties(BindingFlags.Static | BindingFlags.Public))
				{
					string key = prop.Name.ToLowerInvariant();
					uint value = (uint)prop.GetValue(null)!;
					eventIds.Add(key, value);
				}
			}

			eventIds.TryGetValue(eventName.ToLowerInvariant(), out uint id);
			return id;
		}

		internal static ToolsItem[] GetToolsItems(List<string> items)
		{
			var list = new List<ToolsItem>();
			foreach (string item in items)
			{
				ToolsItem ti = GetToolsItem(item);
				list.Add(ti);
			}

			return list.ToArray();
		}

		internal static ToolsItem GetToolsItem(string item)
		{
			ToolsItem ti = Addressables.LoadAssetAsync<GameObject>(item).WaitForCompletion().GetComponent<ToolsItem>();
			return ti;

		}

			internal static Il2CppTLD.Gear.BlueprintData.RequiredGearItem[] GetRequiredGearItems(List<ModRequiredGearItem> RequiredGear)
		{
			var list = new List<Il2CppTLD.Gear.BlueprintData.RequiredGearItem>();
			foreach (ModRequiredGearItem mrgi in RequiredGear)
			{
				Il2CppTLD.Gear.BlueprintData.RequiredGearItem rgi = new();
				rgi.m_Item = Addressables.LoadAssetAsync<GameObject>(mrgi.Item).WaitForCompletion().GetComponent<GearItem>();
				rgi.m_Count = mrgi.Count;
				list.Add(rgi);
			}

			return list.ToArray();
		}

		internal static Il2CppTLD.Gear.BlueprintData.RequiredLiquid[] GetRequiredLiquid(List<ModRequiredLiquid> RequiredLiquid)
		{
			var list = new List<Il2CppTLD.Gear.BlueprintData.RequiredLiquid>();
			foreach (ModRequiredLiquid mrl in RequiredLiquid)
			{
				Il2CppTLD.Gear.BlueprintData.RequiredLiquid rl = new();
				rl.m_Liquid = Addressables.LoadAssetAsync<LiquidType>(mrl.Liquid).WaitForCompletion();
				rl.m_VolumeInLitres = mrl.VolumeInLitres;
				list.Add(rl);
			}

			return list.ToArray();
		}

		internal static Il2CppTLD.Gear.BlueprintData.RequiredPowder[] GetRequiredPowder(List<ModRequiredPowder> RequiredPowder)
		{
			var list = new List<Il2CppTLD.Gear.BlueprintData.RequiredPowder>();
			foreach (ModRequiredPowder mrp in RequiredPowder)
			{
				Il2CppTLD.Gear.BlueprintData.RequiredPowder rp = new();
				rp.m_Powder = Addressables.LoadAssetAsync<PowderType>(mrp.Powder).WaitForCompletion();
				rp.m_QuantityInKilograms = mrp.QuantityInKilograms;
				list.Add(rp);
			}

			return list.ToArray();
		}

		internal static class EnumValues<T> where T : struct, Enum
		{
			private static readonly T[] values = Enum.GetValues<T>();
			public static bool Contains(T value) => values.Contains(value);
		}

		internal static T TranslateEnumValue<T, E>(E value) where T : Enum where E : Enum
		{
			return (T)Enum.Parse(typeof(T), Enum.GetName(typeof(E), value));
		}

	}
}
