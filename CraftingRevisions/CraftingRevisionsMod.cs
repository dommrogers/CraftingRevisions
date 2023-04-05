using MelonLoader;
using UnityEngine;
using Il2CppTLD.AddressableAssets;
namespace CraftingRevisions
{
	internal class CraftingRevisionsMod : MelonMod
	{
		public override void OnInitializeMelon()
		{
			Settings.instance.AddToModSettings("Crafting Revisions");
		}
	}
}
