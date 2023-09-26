using MelonLoader;
using UnityEngine;
using Il2CppTLD.AddressableAssets;
using AsmResolver.DotNet;

namespace CraftingRevisions
{
	internal class CraftingRevisionsMod : MelonMod
	{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		internal static CraftingRevisionsMod instance;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

		public CraftingRevisionsMod()
		{
			instance = this;
		}

		public override void OnInitializeMelon()
		{
			Settings.instance.AddToModSettings("Crafting Revisions");
		}
	}
}
