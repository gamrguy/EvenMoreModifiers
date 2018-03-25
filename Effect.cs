using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace EvenMoreModifiers
{
	public class Effect : GlobalItem
	{
		public virtual float MinScale => 0.1f;
		public virtual float MaxScale => 1.0f;
		public virtual float BasePower => 1.0f;
		public virtual float Weight => 1.0f;
		public virtual int AutoRound => -1;  // -1 == no rounding. Rounds Power to nearest # of fractional digits.
		public virtual bool CustomScaling => false;  // override to true to use own power scaling method

		public new Mod mod;
		public new string Name;

		public float Scale { get; internal set; }
		public float Power { get; internal set; }

		public override bool Autoload(ref string name) => false;

		public void Apply(Item item, EMMPlayer player)
		{
			if (!CustomScaling)
			{
				Scale = Math.Max(MinScale, (Main.rand.NextFloat() - Main.rand.NextFloat() / 5) * MaxScale);
				Scale *= Math.Min(EMMItem.LUCK_POWER_CAP, 1f + player.luck * (Math.Max(0, Main.rand.NextFloat() - Main.rand.NextFloat() / 4)) / 100);
				Power = BasePower * Scale;
			}
			else CustomScale(item, player);

			if (AutoRound > -1)
				Power = (float)Math.Round(Power, AutoRound);

			ApplyItem(item);
		}

		public new Effect Clone()
		{
			var clone = (Effect)MemberwiseClone();
			clone.mod = mod;
			clone.Name = Name;
			clone.Scale = Scale;
			clone.Power = Power;
			Clone(ref clone);
			return clone;
		}

		public override TagCompound Save(Item item)
		{
			var tag = new TagCompound();
			tag.Add("Scale", Scale);
			tag.Add("Power", Power);
			return tag;
		}

		public override void Load(Item item, TagCompound tag)
		{
			Scale = tag.GetFloat("Scale");
			Power = tag.GetFloat("Power");

			ApplyItem(item);
		}

		// EMM hooks
		public virtual void ApplyItem(Item item) { }                     // Change item's stats here
		public virtual bool CanRoll(Item item, string context) => true;  // Whether this effect can be rolled
		public virtual void Clone(ref Effect effect) { }                 // Called when cloning a new Effect
		public virtual void CustomScale(Item item, EMMPlayer player) { } // Called when CustomScaling is true

		// tML hooks - ModPlayer
		public virtual void CatchFish(Player player, Item fishingRod, Item bait, int power, int liquidType, int poolSize, int worldLayer, int questFish, ref int caughtType, ref bool junk) { }
		public virtual void GetFishingLevel(Player player, Item fishingRod, Item bait, ref int fishingLevel) { }
		public virtual bool ConsumeAmmoPlayer(Player player, Item weapon, Item ammo) => true;
	}
}
