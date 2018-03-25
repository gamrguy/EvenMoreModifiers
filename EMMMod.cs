using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using EvenMoreModifiers.Effects;

/*
 * (c) original version by hiccup
 * reworked and maintained by thegamemaster1234
 * for tmodloader
 */

namespace EvenMoreModifiers
{
	public class EMMMod : Mod
	{
		public static List<EMMRarity> rarities;        // Rarities list
		public static Dictionary<string, Effect> effectRegistry;

		public EMMMod()
		{
			Properties = new ModProperties()
			{
				Autoload = true,
				AutoloadGores = true,
				AutoloadSounds = true
			};
		}

		public override void Load()
		{
			effectRegistry = new Dictionary<string, Effect>();
			AddEffect(this, new DamagePlusEffect());
			AddEffect(this, new CritPlusEffect());
			AddEffect(this, new SpeedPlusEffect());
			AddEffect(this, new VelocityPlusEffect());

			// Rarities are determined by the sum of the effects' ratios of current power to max power.
			// One effect at half strength would be Uncommon. Three would give Rare.
			// An Ascendant item requires an average of about 0.9. Absolute maximum (with Luck) is 4.8, or 1.2 per item.
			rarities = new List<EMMRarity>();
			AddRarity("Common", 0f, Color.White);
			AddRarity("Uncommon", 0.5f, Color.Orange);
			AddRarity("Rare", 1.5f, Color.Yellow);
			AddRarity("Legendary", 2.5f, Color.Red);
			AddRarity("Ascendant", 3.5f, Color.Purple);
		}

		public static void AddRarity(string name, float value, Color color)
		{
			rarities.Add(new EMMRarity(name, value, color));
		}

		public static EMMRarity GetRarity(float value)
		{
			float x = float.MinValue;
			var result = new EMMRarity("Common", 0f, Color.White);
			rarities.ForEach((r) => {
				if(r.Value > x && r.Value <= value)
				{
					x = r.Value;
					result = r;
				}
			});

			return result;
		}

		public static bool AddEffect(Mod mod, Effect effect)
		{
			effect.mod = mod;
			effect.Name = effect.GetType().Name;
			string name = $"{mod.Name}:{effect.Name}";
			if (!effectRegistry.ContainsKey(name))
			{
				effectRegistry.Add(name, effect);
				return true;
			}
			return false;
		}

		public static Effect GetEMMEffect(string name)
		{
			Effect e;
			effectRegistry.TryGetValue(name, out e);
			e = e.Clone();
			return e;
		}

		public override void Unload()
		{
		}

		// @todo: probably write our own handler for packets
		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{

		}
	}
}
