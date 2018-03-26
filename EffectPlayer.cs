using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace EvenMoreModifiers
{
	/// <summary>
	/// Holds player-entity data and handles it
	/// </summary>
	public class EffectPlayer : ModPlayer
	{
		public static EffectPlayer PlayerInfo(Player player) => player.GetModPlayer<EffectPlayer>();

		// Globals for modifiers
		public bool holdingCursed;    // Whether currently holding a cursed item (take 1 damage per second)
		public float dodgeChance;     // Dodge chance (TODO: Implement this)

		// List of current debuff chances
		public List<DebuffChance> debuffChances = new List<DebuffChance>();

		public override void ResetEffects()
		{
			dodgeChance = 0;
			debuffChances.Clear();
		}

		public override void UpdateBadLifeRegen()
		{
			if (holdingCursed)
			{
				if (player.lifeRegen > 0)
				{
					player.lifeRegen = 0;
				}
				player.lifeRegen -= 2;
				player.lifeRegenTime = 0;
			}
			holdingCursed = false;
		}

		public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
		{
			Main.NewText(debuffChances.Count);
			debuffChances.ForEach((x) =>
			{
				if (Main.rand.NextFloat() < x.BuffChance)
					target.AddBuff(x.BuffType, x.BuffTime);
			});
		}

		public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			debuffChances.ForEach((x) =>
			{
				if (Main.rand.NextFloat() < x.BuffChance)
					target.AddBuff(x.BuffType, x.BuffTime);
			});
		}
	}
}
