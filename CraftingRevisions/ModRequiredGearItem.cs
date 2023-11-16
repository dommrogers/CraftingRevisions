using Il2CppTLD.Gear;

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
        /// Count of required units 
        /// </summary>
        public float Quantity { get; set; } = 0.0f;
    }
}
