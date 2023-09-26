using Il2Cpp;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CraftingRevisions
{
	internal sealed class ModUserRecipeBlueprintData
	{
		/// <summary>
		/// optional name, used for debugging
		/// </summary>
		public string? Name { get; set; } = null;
		public List<ModRequiredGearItem> RequiredGear { get; set; } = new();
		public List<ModRequiredPowder> RequiredPowder { get; set; } = new();
		public List<ModRequiredLiquid> RequiredLiquid { get; set; } = new();
		public string? CraftedResult { get; set; } = null;
		public int CraftedResultCount { get; set; } = 0;
		public int DurationMinutes { get; set; } = 0;
		public string? CraftingAudio { get; set; } = null;

	}
}