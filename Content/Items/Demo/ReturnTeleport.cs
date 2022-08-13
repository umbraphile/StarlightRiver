using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using StarlightRiver.Core;
using StarlightRiver.Content.Items.Misc;
using StarlightRiver.Content.Items.Gravedigger;
using StarlightRiver.Content.Abilities;
using StarlightRiver.Content.Items.UndergroundTemple;
using StarlightRiver.Content.Items.Beach;

namespace StarlightRiver.Content.Items.Demo
{
	internal class ReturnTeleport : ModItem
	{
		public override string Texture => "StarlightRiver/Assets/Items/Demo/" + Name;

		public override void SetDefaults()
		{
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Swing;
		}

		public override bool? UseItem(Player player)
		{
			player.Center = new Vector2(Main.spawnTileX, Main.spawnTileY) * 16;
			player.SpawnX = (int)player.Center.X / 16;
			player.SpawnY = (int)player.Center.Y / 16;
			player.noFallDmg = true;

			var newInv = new Item[player.inventory.Length];

			player.statLifeMax = 100;
			player.statManaMax = 20;
			player.GetModPlayer<AbilityHandler>().unlockedAbilities.Clear();

			for (int k = 0; k < player.inventory.Length; k++)
			{
				var i = new Item();

				switch (k)
				{
					case 0:
						i.SetDefaults(ModContent.ItemType<DifficultySwitcher>());
						break;

					case 1:
						i.SetDefaults(ModContent.ItemType<AuroracleTeleport>());
						break;

					case 2:
						i.SetDefaults(ModContent.ItemType<CeirosTeleport>());
						break;
				}

				newInv[k] = i;
			}

			player.inventory = newInv;

			for (int k = 0; k < player.armor.Length; k++)
			{
				var i = new Item();
					i.TurnToAir();

				player.armor[k] = i;
			}

			Item i2 = new Item();
			i2.TurnToAir();

			player.miscEquips[4] = i2;

			return true;
		}
	}
}
