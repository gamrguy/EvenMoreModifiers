using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace EvenMoreModifiers
{
	public struct EMMRarity
	{
		public string Name;
		public float Value;
		public Color Color;

		public EMMRarity(string n, float v, Color c)
		{
			Name = n;
			Value = v;
			Color = c;
		}
	}

	public struct DebuffChance
	{
		public int BuffType;
		public int BuffTime;
		public float BuffChance;

		public DebuffChance(int bType, int bTime, float bChance)
		{
			BuffType = bType;
			BuffTime = bTime;
			BuffChance = bChance;
		}
	}

	public static class TooltipHelper
	{
		public static void AddTooltipLine(this List<TooltipLine> tooltips, Mod mod, string name, string text, Color color)
		{
			tooltips.Add(new TooltipLine(mod, name, text) { overrideColor = color });
		}
	}
}
