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

	public class DamagePlusEffect : WeaponEffect
	{
		public override float BasePower => 10f;
		public override int AutoRound => 0;

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddTooltipLine(mod, Name, $"+{Power}% damage", Color.LimeGreen);
		}

		public override void GetWeaponDamage(Item item, Player player, ref int damage)
		{
			damage = (int)Math.Ceiling(damage * (1 + Power / 100));
		}
	}

	public class CritPlusEffect : WeaponEffect
	{
		public override float BasePower => 10f;
		public override int AutoRound => 0;

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddTooltipLine(mod, Name, $"+{Power}% crit chance", Color.LimeGreen);
		}

		public override void GetWeaponCrit(Item item, Player player, ref int crit)
		{
			crit += (int)Power;
		}
	}

	public class SpeedPlusEffect : WeaponEffect
	{
		public override float BasePower => 10f;
		public override int AutoRound => 0;

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddTooltipLine(mod, Name, $"+{Power}% speed", Color.LimeGreen);
		}

		public override float UseTimeMultiplier(Item item, Player player)
		{
			return 1 + Power / 100;
		}
	}

	public class KnockbackPlusEffect : WeaponEffect
	{
		public override float MinScale => 0.05f;
		public override float BasePower => 20f;
		public override int AutoRound => 0;

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddTooltipLine(mod, Name, $"+{Power}% knockback", Color.LimeGreen);
		}

		public override void GetWeaponKnockback(Item item, Player player, ref float knockback)
		{
			knockback *= 1 + Power / 100;
		}
	}

	public class VelocityPlusEffect : WeaponEffect
	{
		public override float MinScale => 0.05f;
		public override float BasePower => 20f;
		public override int AutoRound => 0;

		public override bool CanRoll(Item item, string context)
		{
			return base.CanRoll(item, context) && item.shootSpeed > 0;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddTooltipLine(mod, Name, $"+{Power}% velocity", Color.LimeGreen);
		}

		public override void ApplyItem(Item item)
		{
			item.shootSpeed *= 1 + Power / 100;
		}
	}
}
