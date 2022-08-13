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
	internal class AuroracleTeleport : ModItem
	{
		public override string Texture => AssetDirectory.Debug;

		public override void SetDefaults()
		{
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Swing;
		}

		public override bool? UseItem(Player player)
		{
			player.Center = StarlightWorld.SquidBossArena.Center.ToVector2() * 16 + new Vector2(0, -80 * 16);
			player.SpawnX = (int)player.Center.X / 16;
			player.SpawnY = (int)player.Center.Y / 16;
			player.noFallDmg = true;

			var newInv = new Item[player.inventory.Length];

			player.statLifeMax = 400;
			player.statManaMax = 200;
			player.GetModPlayer<AbilityHandler>().unlockedAbilities.Clear();

			for (int k = 0; k < player.inventory.Length; k++)
			{
				var i = new Item();

				switch (k)
				{
					case 0:
						i.SetDefaults(ItemID.LightsBane);					
						break;

					case 1:
						i.SetDefaults(ModContent.ItemType<FryingPan>());
						break;

					case 2:
						i.SetDefaults(ItemID.DemonBow);
						break;

					case 3:
						i.SetDefaults(ModContent.ItemType<CoachGun>());
						break;

					case 4:
						i.SetDefaults(ItemID.RubyStaff);
						break;

					case 5:
						i.SetDefaults(ModContent.ItemType<Gluttony>());
						break;

					case 6:
						i.SetDefaults(ItemID.DirtBlock);
						i.stack = 999;
						break;

					case 9:
						i.SetDefaults(ModContent.ItemType<ReturnTeleport>());
						break;

					case 54:
						i.SetDefaults(ItemID.EndlessMusketPouch);
						break;

					case 55:
						i.SetDefaults(ItemID.EndlessQuiver);
						break;

					default:
						i.TurnToAir();
						break;
				}

				newInv[k] = i;
			}

			player.inventory = newInv;

			for(int k = 0; k < player.armor.Length; k++)
			{
				var i = new Item();

				if (k == 0)
					i.SetDefaults(ItemID.GoldHelmet);
				else if (k == 1)
					i.SetDefaults(ItemID.GoldChainmail);
				else if (k == 2)
					i.SetDefaults(ItemID.GoldGreaves);
				else if (k == 3)
					i.SetDefaults(ItemID.CloudinaBalloon);
				else if (k == 4)
					i.SetDefaults(ItemID.HermesBoots);
				else if (k == 5)
					i.SetDefaults(ModContent.ItemType<TempleRune>());
				else if (k == 6)
					i.SetDefaults(ModContent.ItemType<SeaglassRing>());
				else if (k == 7)
					i.SetDefaults(ModContent.ItemType<HolyAmulet>());
				else
					i.TurnToAir();

				player.armor[k] = i;
			}

			Item i2 = new Item();
			i2.SetDefaults(ItemID.GrapplingHook);

			player.miscEquips[4] = i2;

			return true;
		}
	}
}
