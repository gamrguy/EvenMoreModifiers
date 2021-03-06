﻿using System;
using Loot.System;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.EquipModifiers
{
	public class WingTimePlus : EquipModifier
	{
		public override ModifierTooltipLine[] Description => new[]
		{
			new ModifierTooltipLine { Text = $"+{Math.Round(RoundedPower/60, 2)}s flight time", Color =  Color.LimeGreen},
		};

		public override float GetMinMagnitude(Item item) => 1f;
		public override float GetMaxMagnitude(Item item) => 60f;
		
		public override void UpdateEquip(Item item, Player player)
		{
			player.wingTimeMax += (int)RoundedPower;
		}
	}
}
