﻿using System;
using Loot.System;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.WeaponModifiers
{
	public class KnockbackPlus : WeaponModifier
	{
		public override ModifierTooltipLine[] Description => new[]
			{
				new ModifierTooltipLine { Text = $"+{RoundedPower}% knockback", Color = Color.Lime}
			};

		public override float GetMinMagnitude(Item item) => 1f;
		public override float GetMaxMagnitude(Item item) => 20f;

		public override bool CanRoll(ModifierContext ctx) 
			=> base.CanRoll(ctx) && ctx.Item.knockBack > 0;

		public override void GetWeaponKnockback(Item item, Player player, ref float knockback)
		{
			base.GetWeaponKnockback(item, player, ref knockback);
			knockback *= RoundedPower / 100 + 1;
		}
	}
}
