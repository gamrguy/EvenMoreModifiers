using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace EvenMoreModifiers
{
	public class EMMItem : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool CloneNewInstances => true;
		
		public static int MAX_EFFECTS = 4;   // Maximum effects that can be rolled. More can be added through hax.
		public static float EFFECT_CHANCE = 0.5f; // Chance to roll an effect, default 50%
		public static float LUCK_EFFECT_CHANCE = 0.1f / 3; // Increase in chance for rolling effects, default 0.03333% each
		public static float LUCK_POWER_CAP = 1.2f;  // Maximum value that luck can increase effect power by, default 20%

		public List<Effect> effects;  // Effects on this item
		public bool hasRolled;    //Whether this item has rolled for effects

		// Calculates the current power level of effects on this item. Used for determining rarity.
		private float EffectSummary()
		{
			float result = 0;
			effects.ForEach((e) => result += e.Power/e.BasePower);
			return result;
		}

		private void RollEffects(Item item, EMMPlayer player, string context)
		{
			hasRolled = true;

			var rand = new WeightedRandom<Effect>();

			// Add all applicable effects to the random
			foreach (var e in EMMMod.effectRegistry)
				if (e.Value.CanRoll(item, context))
					rand.Add(e.Value, e.Value.Weight);

			if (rand.elements.Count == 0)
				return;

			if (effects == null)
				effects = new List<Effect>();
			else if (effects != null)
				effects.Clear();
			else
				return;

			// Add all effects
			for (int i = 0; i < MAX_EFFECTS; i++)
			{
				if (rand.elements.Count > 0 && Main.rand.NextFloat() < EFFECT_CHANCE + player.luck * LUCK_EFFECT_CHANCE)
				{
					Effect r = rand.Get();
					Effect e = r.Clone();
					e.Apply(item, player);
					effects.Add(e);
					rand.elements.Remove(new Tuple<Effect, double>(r, r.Weight));
					rand.needsRefresh = true;
				}
				else break;
			}
		}

		public override TagCompound Save(Item item)
		{
			var tag = new TagCompound();
			tag.Add("HasRolled", hasRolled);

			var effectTag = new TagCompound();
			effects.ForEach((e) => effectTag.Add($"{e.mod.Name}:{e.Name}", e.Save(item)));
			tag.Add("Effects", effectTag);

			return tag;
		}

		public override void Load(Item item, TagCompound tag)
		{
			hasRolled = tag.GetBool("HasRolled");
			foreach (var kv in tag.GetCompound("Effects"))
			{
				Effect e;
				if (EMMMod.effectRegistry.TryGetValue(kv.Key, out e))
				{
					e = e.Clone();
					e.Load(item, kv.Value as TagCompound);
					effects.Add(e);
				}
			}
		}

		public override bool NeedsSaving(Item item) => hasRolled;

		public override GlobalItem Clone(Item item, Item itemClone)
		{
			var clone = (EMMItem)base.Clone(item, itemClone);
			if (effects != null)
				clone.effects = effects.Select(e => e.Clone()).ToList();

			return clone;
		}

		public override void NetSend(Item item, BinaryWriter writer)
		{
			writer.Write(hasRolled);
			writer.Write(effects.Count);
			foreach(var e in effects)
			{
				writer.Write($"{e.mod.Name}:{e.Name}");
				writer.Write((double)e.Power);
				writer.Write((double)e.Scale);
				e.NetSend(item, writer);
			}
		}

		public override void NetReceive(Item item, BinaryReader reader)
		{
			hasRolled = reader.ReadBoolean();
			int count = reader.ReadInt32();
			for(int i=0; i<count; i++)
			{
				Effect e = EMMMod.GetEMMEffect(reader.ReadString()).Clone();
				if (e == null) return;
				e.Power = (float)reader.ReadDouble();
				e.Scale = (float)reader.ReadDouble();
				e.NetReceive(item, reader);
				e.ApplyItem(item);
			}
		}

		public override void SetDefaults(Item item)
		{
			if (effects == null) effects = new List<Effect>();
			effects.ForEach((e) => e.SetDefaults(item));
		}

		public override bool AltFunctionUse(Item item, Player player)
		{
			bool result = false;
			effects.ForEach((e) => result |= e.AltFunctionUse(item, player));
			return result;
		}

		public override bool CanEquipAccessory(Item item, Player player, int slot)
		{
			bool result = false;
			effects.ForEach((e) => result |= e.CanEquipAccessory(item, player, slot));
			return result;
		}

		public override bool? CanHitNPC(Item item, Player player, NPC target)
		{
			bool? result = null;
			foreach (var e in effects)
			{
				bool? canHit = e.CanHitNPC(item, player, target);
				if (canHit.HasValue && !canHit.Value) return false;
				if (canHit.HasValue) result = canHit.Value;
			}
			return result;
		}

		public override bool CanHitPvp(Item item, Player player, Player target)
		{
			bool result = true;
			effects.ForEach((e) => result &= e.CanHitPvp(item, player, target));
			return result;
		}

		public override void HoldItem(Item item, Player player)
		{
			effects.ForEach((e) => e.HoldItem(item, player));
		}

		public override bool HoldItemFrame(Item item, Player player)
		{
			bool result = false;
			effects.ForEach((e) => result |= e.HoldItemFrame(item, player));
			return result;
		}

		public override void Update(Item item, ref float gravity, ref float maxFallSpeed)
		{
			foreach (var e in effects)
				e.Update(item, ref gravity, ref maxFallSpeed);
		}

		public override void UpdateInventory(Item item, Player player)
		{
			effects.ForEach((e) => e.UpdateInventory(item, player));
		}

		public override void UpdateAccessory(Item item, Player player, bool hideVisual)
		{
			effects.ForEach((e) => e.UpdateAccessory(item, player, hideVisual));
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			if (effects.Count() > 0)
			{
				var rarity = EMMMod.GetRarity(EffectSummary());
				tooltips.AddTooltipLine(mod, "RarityLine", $"[{rarity.Name}]", rarity.Color);
				effects.ForEach((e) => e.ModifyTooltips(item, tooltips));
			}
		}

		public override bool OnPickup(Item item, Player player)
		{
			if (!hasRolled) RollEffects(item, player.GetModPlayer<EMMPlayer>(), "Pickup");

			bool result = true;
			effects.ForEach((e) => result &= e.OnPickup(item, player));
			return result;
		}

		public override bool NewPreReforge(Item item)
		{
			bool result = true;
			effects.ForEach((e) => result &= e.NewPreReforge(item));
			return result;
		}

		public override void PostReforge(Item item)
		{
			if (!hasRolled || effects.Count == 0) RollEffects(item, Main.player[Main.myPlayer].GetModPlayer<EMMPlayer>(), "Reforge");

			effects.ForEach((e) => e.PostReforge(item));
		}

		public override void GetWeaponCrit(Item item, Player player, ref int crit)
		{
			foreach (var e in effects)
				e.GetWeaponCrit(item, player, ref crit);
		}

		public override void GetWeaponDamage(Item item, Player player, ref int damage)
		{
			foreach(var e in effects)
				e.GetWeaponDamage(item, player, ref damage);
		}

		public override void GetWeaponKnockback(Item item, Player player, ref float knockback)
		{
			foreach (var e in effects)
				e.GetWeaponKnockback(item, player, ref knockback);
		}
		
		public override bool ConsumeAmmo(Item item, Player player)
		{
			bool result = true;
			effects.ForEach((e) => result &= e.ConsumeAmmo(item, player));
			return result;
		}

		public override void MeleeEffects(Item item, Player player, Rectangle hitbox)
		{
			effects.ForEach((e) => e.MeleeEffects(item, player, hitbox));
		}

		public override void ModifyHitNPC(Item item, Player player, NPC target, ref int damage, ref float knockback, ref bool crit)
		{
			foreach (var e in effects)
				e.ModifyHitNPC(item, player, target, ref damage, ref knockback, ref crit);
		}

		public override void ModifyHitPvp(Item item, Player player, Player target, ref int damage, ref bool crit)
		{
			foreach (var e in effects)
				e.ModifyHitPvp(item, player, target, ref damage, ref crit);
		}

		public override void OnHitNPC(Item item, Player player, NPC target, int damage, float knockback, bool crit)
		{
			effects.ForEach((e) => e.OnHitNPC(item, player, target, damage, knockback, crit));
		}

		public override void OnHitPvp(Item item, Player player, Player target, int damage, bool crit)
		{
			effects.ForEach((e) => e.OnHitPvp(item, player, target, damage, crit));
		}

		public override float UseTimeMultiplier(Item item, Player player)
		{
			float result = 1f;
			effects.ForEach((e) => result *= e.UseTimeMultiplier(item, player));
			return result;
		}
	}
}
