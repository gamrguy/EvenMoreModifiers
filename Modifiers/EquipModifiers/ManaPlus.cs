﻿using Loot.System;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.EquipModifiers
{
	public class ManaPlus : EquipModifier
	{
		public override ModifierTooltipLine[] Description => new[]
		{
			new ModifierTooltipLine { Text = $"+{RoundedPower} max mana", Color =  Color.LimeGreen},
		};

		public override float GetMinMagnitude(Item item) => 1f;
		public override float GetMaxMagnitude(Item item) => 20f;
		
		public override void UpdateEquip(Item item, Player player)
		{
			player.statManaMax2 += (int)RoundedPower;
		}
	}
}
