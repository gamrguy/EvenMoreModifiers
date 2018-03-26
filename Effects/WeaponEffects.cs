using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace EvenMoreModifiers.Effects
{
	public abstract class WeaponEffect : Effect
	{
		public override float MinScale => 1f/BasePower;

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
			tooltips.AddTooltipLine(mod, Name, $"+{Power}% damage", Color.IndianRed);
		}

		public override void GetWeaponDamage(Item item, Player player, ref int damage)
		{
			int oldDamage = damage;
			damage = (int)Math.Round(damage * (1 + Power / 100));
			if (damage - oldDamage == 0) damage++;
			Power = (float)Math.Round(((float)damage / oldDamage - 1) * 100, 0);
		}
	}

	public class CritPlusEffect : WeaponEffect
	{
		public override float BasePower => 10f;
		public override int AutoRound => 0;

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddTooltipLine(mod, Name, $"+{Power}% crit chance", Color.Crimson);
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

		public override bool CanRoll(Item item, string context)
		{
			return base.CanRoll(item, context) && !item.channel;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddTooltipLine(mod, Name, $"+{Power}% speed", Color.Cyan);
		}

		public override float UseTimeMultiplier(Item item, Player player)
		{
			return 1 + Power / 100;
		}
	}

	public class KnockbackPlusEffect : WeaponEffect
	{
		public override float BasePower => 20f;
		public override int AutoRound => 0;

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddTooltipLine(mod, Name, $"+{Power}% knockback", Color.Gray);
		}

		public override void GetWeaponKnockback(Item item, Player player, ref float knockback)
		{
			knockback *= 1 + Power / 100;
		}
	}

	public class VelocityPlusEffect : WeaponEffect
	{
		public override float BasePower => 20f;
		public override int AutoRound => 0;

		public override bool CanRoll(Item item, string context)
		{
			return base.CanRoll(item, context) && item.shootSpeed > 0;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddTooltipLine(mod, Name, $"+{Power}% velocity", Color.SteelBlue);
		}

		public override void ApplyItem(Item item)
		{
			item.shootSpeed *= 1 + Power / 100;
		}
	}

	public class ManaReduceEffect : WeaponEffect
	{
		public override float BasePower => 15f;
		public override int AutoRound => 0;

		public override bool CanRoll(Item item, string context)
		{
			return base.CanRoll(item, context) && item.mana > 1;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddTooltipLine(mod, Name, $"-{Power}% mana cost", Color.MediumPurple);
		}

		public override void ApplyItem(Item item)
		{
			int preMana = item.mana;
			item.mana = (int)Math.Floor(item.mana * (1 - Power / 100));
			if (item.mana <= 0) item.mana = 1;
			Power = (float)Math.Round((1 - (float)item.mana / preMana) * 100);
		}
	}

	public class AmmoReduceEffect : WeaponEffect
	{
		public override float BasePower => 20f;
		public override int AutoRound => 0;

		public override bool CanRoll(Item item, string context)
		{
			return base.CanRoll(item, context) && item.useAmmo > 0;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddTooltipLine(mod, Name, $"{Power}% chance to not consume ammo", Color.PaleGoldenrod);
		}

		public override bool ConsumeAmmoPlayer(Player player, Item weapon, Item ammo)
		{
			return Main.rand.NextFloat() > Power / 100;
		}
	}

	public class DamageWithManaCostEffect : DamagePlusEffect
	{
		public override float MinScale => 16f/30;
		public override float BasePower => 30f;

		public override bool CanRoll(Item item, string context)
		{
			return base.CanRoll(item, context) && item.mana == 0;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddTooltipLine(mod, Name, $"+{Power}% damage, but added mana cost", Color.Coral);
		}

		public override void ApplyItem(Item item)
		{
			item.mana += Math.Max((int)(25 * (item.useTime / 60.0)), 1);
		}
	}

	public class DamagePlusDayEffect : DamagePlusEffect
	{
		public override float BasePower => 15f;

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddTooltipLine(mod, Name, $"+{Power}% damage during the day", Color.Coral);
		}

		public override void GetWeaponDamage(Item item, Player player, ref int damage)
		{
			if (Main.dayTime) base.GetWeaponDamage(item, player, ref damage);
		}
	}

	public class DamagePlusNightEffect : DamagePlusEffect
	{
		public override float BasePower => 15f;

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddTooltipLine(mod, Name, $"+{Power}% damage during the night", Color.BlueViolet);
		}

		public override void GetWeaponDamage(Item item, Player player, ref int damage)
		{
			if (!Main.dayTime) base.GetWeaponDamage(item, player, ref damage);
		}
	}

	public class CursedDamageEffect : DamagePlusEffect
	{
		public override float MinScale => 16f / 30;
		public override float BasePower => 30f;

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddTooltipLine(mod, Name, $"+{Power}% damage, but cursed", Color.Coral);
		}

		public override void HoldItem(Item item, Player player)
		{
			EffectPlayer.PlayerInfo(player).holdingCursed = true;
		}
	}

	public abstract class DebuffEffect : WeaponEffect
	{
		public override float BasePower => 50f;
		public override int AutoRound => 0;

		public abstract string DebuffName { get; }
		public abstract int DebuffType { get; }
		public abstract int DebuffTime { get; }
		public abstract Color DebuffColor { get; }
		
		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddTooltipLine(mod, Name, $"+{Power}% chance to inflict {DebuffName} for {Math.Round((float)DebuffTime/60, 1)}s", DebuffColor);
		}

		public override void HoldItem(Item item, Player player)
		{
			EffectPlayer.PlayerInfo(player).debuffChances.Add(new DebuffChance(DebuffType, DebuffTime, Power/100));
		}
	}

	public class PoisonDebuffEffect : DebuffEffect
	{
		public override string DebuffName => "Poisoned";
		public override int DebuffType => Terraria.ID.BuffID.Poisoned;
		public override int DebuffTime => 480;
		public override Color DebuffColor => Color.Green;
	}

	public class FireDebuffEffect : DebuffEffect
	{
		public override string DebuffName => "On Fire!";
		public override int DebuffType => Terraria.ID.BuffID.OnFire;
		public override int DebuffTime => 300;
		public override Color DebuffColor => Color.OrangeRed;
	}

	public class FrostburnDebuffEffect : DebuffEffect
	{
		public override string DebuffName => "Frostburn";
		public override int DebuffType => Terraria.ID.BuffID.Frostburn;
		public override int DebuffTime => 240;
		public override Color DebuffColor => Color.MediumBlue;
	}

	public class ConfuseDebuffEffect : DebuffEffect
	{
		public override string DebuffName => "Confusion";
		public override int DebuffType => Terraria.ID.BuffID.Confused;
		public override int DebuffTime => 120;
		public override Color DebuffColor => Color.Pink;
	}

	public class CursedInfernoDebuffEffect : DebuffEffect
	{
		public override string DebuffName => "Cursed Inferno";
		public override int DebuffType => Terraria.ID.BuffID.CursedInferno;
		public override int DebuffTime => 180;
		public override Color DebuffColor => Color.GreenYellow;
	}

	public class IchorDebuffEffect : DebuffEffect
	{
		public override float BasePower => 20f;

		public override string DebuffName => "Ichor";
		public override int DebuffType => Terraria.ID.BuffID.Ichor;
		public override int DebuffTime => 180;
		public override Color DebuffColor => Color.YellowGreen;
	}
}
