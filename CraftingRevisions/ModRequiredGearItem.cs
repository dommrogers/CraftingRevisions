using Il2Cpp;
using Il2CppTLD.Gear;
using System.Text.Json.Serialization;

namespace CraftingRevisions
{
	internal sealed class ModRequiredGearItem
	{
		/// <summary>
		/// String value of the gear item
		/// </summary>
		public string? Item { get; set; } = null;
		/// <summary>
		/// Count of how many are required
		/// </summary>
		public int Count { get; set; } = 0;

		/// <summary>
		/// Count of how many are required
		/// </summary>
		public float Quantity { get; set; } = 0f;

		/// <summary>
		/// Unit type (Count/Kilograms)
		/// </summary>
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public BlueprintData.RequiredGearItem.Units Units { get; set; } = BlueprintData.RequiredGearItem.Units.Count;

	}
}
