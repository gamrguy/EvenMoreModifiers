using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace EvenMoreModifiers
{
	public class EMMPlayer : ModPlayer
	{
		public float luck;

		private List<Effect> ItemEffects(Item item) => item.GetGlobalItem<EMMItem>().effects;

		public override void CatchFish(Item fishingRod, Item bait, int power, int liquidType, int poolSize, int worldLayer, int questFish, ref int caughtType, ref bool junk)
		{
			foreach (var e in ItemEffects(fishingRod))
				e.CatchFish(player, fishingRod, bait, power, liquidType, poolSize, worldLayer, questFish, ref caughtType, ref junk);
		}

		public override void GetFishingLevel(Item fishingRod, Item bait, ref int fishingLevel)
		{
			foreach (var e in ItemEffects(fishingRod))
				e.GetFishingLevel(player, fishingRod, bait, ref fishingLevel);
		}

		public override bool ConsumeAmmo(Item weapon, Item ammo)
		{
			bool result = true;
			ItemEffects(weapon).ForEach((e) => result &= e.ConsumeAmmoPlayer(player, weapon, ammo));
			return result;
		}
	}
}
