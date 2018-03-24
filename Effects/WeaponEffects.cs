using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace EvenMoreModifiers.Effects
{
	public class WeaponEffect : Effect
	{
		public override bool CanRoll(Item item, string context)
		{
			return item.damage > 0 && item.maxStack == 1;
		}
	}

	public class DamagePlusEffect : Effect
	{
		public override float BasePower => 10f;
		public override int AutoRound => 0;

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddTooltipLine(mod, Name, $"+{RoundPower(1)}", Color.LimeGreen);
		}

		public override void GetWeaponDamage(Item item, Player player, ref int damage)
		{
			damage = (int)Math.Ceiling(damage * Power / 100);
		}
	}
}
