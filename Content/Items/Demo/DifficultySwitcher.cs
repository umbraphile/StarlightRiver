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
	internal class DifficultySwitcher : ModItem
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
			if (!Main.expertMode)
				Main.GameMode = GameModeID.Expert;

			else if (!Main.masterMode)
				Main.GameMode = GameModeID.Master;

			else
				Main.GameMode = GameModeID.Normal;

			Main.NewText("Difficulty changed to: " + Main.GameMode);

			return true;
		}
	}
}
