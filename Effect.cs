using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace EvenMoreModifiers
{
	public class Effect : GlobalItem
	{
		public virtual float MinScale => 0.1f;
		public virtual float MaxScale => 1.0f;
		public virtual float BasePower => 1.0f;
		public virtual float Weight => 1.0f;
		public virtual int AutoRound => -1;  // -1 == no rounding, 0 = whole number, 1 = 1 decimal place, etc.
		public virtual bool CustomScaling => false;  // override to true to use own power scaling method

		public float Scale { get; internal set; }
		public float Power { get; internal set; }

		public override bool Autoload(ref string name) => false;

		public Effect Clone(EMMPlayer player)
		{
			var clone = (Effect)MemberwiseClone();

			if (!clone.CustomScaling)
			{
				clone.Scale = Math.Max(MinScale, (Main.rand.NextFloat() - Main.rand.NextFloat() / 5) * MaxScale);
				clone.Scale *= Math.Min(EMMItem.LUCK_POWER_CAP, 1f + player.luck * (Math.Max(0, Main.rand.NextFloat() - Main.rand.NextFloat() / 4)) / 100);
				clone.Power = clone.BasePower * clone.Scale;
			}
			else clone.CustomScale(player);

			if (clone.AutoRound > -1)
				clone.Power = (float)Math.Round(clone.Power, clone.AutoRound);

			Clone(ref clone);
			return clone;
		}

		public float RoundPower(int x)
		{
			return (float)Math.Round(Power, x);
		}

		// EMM hooks
		public virtual bool CanRoll(Item item, string context) => true; // Whether this effect can be rolled
		public virtual void Clone(ref Effect effect) { }                // Called when cloning a new Effect
		public virtual void CustomScale(EMMPlayer player) { }           // Called when CustomScaling is true

		// tML hooks - ModPlayer
		public virtual void CatchFish(Player player, Item fishingRod, Item bait, int power, int liquidType, int poolSize, int worldLayer, int questFish, ref int caughtType, ref bool junk) { }
		public virtual void GetFishingLevel(Player player, Item fishingRod, Item bait, ref int fishingLevel) { }
		public virtual bool ConsumeAmmoPlayer(Player player, Item weapon, Item ammo) => true;
	}
}
