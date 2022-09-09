using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StarlightRiver.Core;
using StarlightRiver.Core.Loaders;
using StarlightRiver.Content.Dusts;
using StarlightRiver.Content.Buffs;
using StarlightRiver.Content.Items.Vitric;
using StarlightRiver.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.Enums;
using Terraria.ModLoader;
using System;
using System.Linq;
using System.Collections.Generic;
using Terraria.Graphics.Effects;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace StarlightRiver.Content.Dusts
{
	public class BloodMetaballDust : ModDust
	{

		public override string Texture => AssetDirectory.Assets + "Invisible";

		public override void OnSpawn(Dust dust)
		{
			dust.noLight = true;
		}

		public override bool Update(Dust dust)
		{
			float speed = 1.4f;
			dust.position += dust.velocity * speed;

			if (dust.customData == null)
				dust.customData = new List<Vector2>();

			var list = dust.customData as List<Vector2>;

			list.Add(dust.position);

			if (list.Count > 6)
				list.RemoveAt(0);

			if (dust.noGravity)
				dust.velocity = new Vector2(0, -speed);
			else
			{
				dust.velocity.Y += 0.2f * speed;

				if (dust.position.X > 16 && dust.position.Y > 16)
				{
					var tile = Main.tile[(int)dust.position.X / 16, (int)dust.position.Y / 16];

					if (tile.HasTile && tile.BlockType == BlockType.Solid && Main.tileSolid[tile.TileType])
					{
						dust.scale *= 1.03f;
						dust.velocity *= -0.5f;
					}
				}
			}

			dust.rotation = dust.velocity.ToRotation() + 1.57f;
			dust.scale *= 0.96f;

			if (dust.noGravity)
				dust.scale *= 0.96f;

			if (dust.scale < 0.05f)
				dust.active = false;

			return false;
		}
	}

	public class BloodMetaballDustLight : BloodMetaballDust { }
}
