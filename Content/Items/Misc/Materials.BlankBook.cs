using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using StarlightRiver.Core;
using Terraria.ID;

namespace StarlightRiver.Content.Items.Misc
{
	internal class BlankBookSword : ModItem
	{
		int riddleProgress = 0;

		(int, string)[] riddle = new (int, string)[]
		{
			(ItemID.Muramasa, "Test"),
			(ItemID.WoodenSword, "Test2"),
			(ItemID.BladeofGrass, "Test3"),
			(ItemID.BeeKeeper, "Test4"),
		};

		public override string Texture => AssetDirectory.MiscItem + "BlankBook";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dusty Tome");
			Tooltip.SetDefault("An empty book which could be written with martial techniques");
		}

		public override void SetDefaults()
		{
			Item.width = 32;
			Item.height = 32;
			Item.rare = ItemRarityID.Blue;
			Item.value = 50;
		}

		public override void UpdateInventory(Player player)
		{
			if (player.itemAnimation == 1)
			{
				if (player.HeldItem.type == riddle[riddleProgress].Item1)
					riddleProgress++;
				else
					riddleProgress = 0;
			}

			if (riddleProgress >= 4)
				Item.SetDefaults(ModContent.ItemType<SwordBook>());
		}
	}
}
