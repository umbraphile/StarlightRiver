﻿using StarlightRiver.Content.Biomes;
using StarlightRiver.Content.Buffs;
using StarlightRiver.Content.Tiles.Crimson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace StarlightRiver.Content.Bosses.BrainRedux
{
	internal class TheThinker : ModNPC
	{
		public static readonly List<TheThinker> toRender = new();

		public bool active = false;
		public List<Point16> tilesChanged = new();
		public Vector2 home;

		public ref float ExtraRadius => ref NPC.ai[0];

		public override string Texture => AssetDirectory.BrainRedux + Name;

		public override void Load()
		{
			GraymatterBiome.onDrawHallucinationMap += DrawAura;
			GraymatterBiome.onDrawOverHallucinationMap += DrawMe;
		}

		public override void SetDefaults()
		{
			NPC.noGravity = true;
			NPC.aiStyle = -1;
			NPC.width = 128;
			NPC.height = 128;
			NPC.damage = 10;
			NPC.lifeMax = 1000;
			NPC.knockBackResist = 0f;
			NPC.friendly = false;
			NPC.noTileCollide = true;

			toRender.Add(this);
		}

		public override void AI()
		{
			if (home == default)
				home = NPC.Center;

			GraymatterBiome.forceGrayMatter = true;

			Lighting.AddLight(NPC.Center, new Vector3(1f, 0.2f, 0.2f));

			if (ExtraRadius > 0)
				ExtraRadius -= 0.5f;

			NPC.life = NPC.lifeMax;

			for(int k = 0; k < Main.maxPlayers; k++)
			{
				var player = Main.player[k];

				if (Vector2.DistanceSquared(player.Center, NPC.Center) <= Math.Pow((200 + ExtraRadius), 2))
					player.AddBuff(ModContent.BuffType<CrimsonHallucination>(), 10);
			}
		
			if(active && (NPC.crimsonBoss < 0 || !Main.npc[NPC.crimsonBoss].active))
			{
				ResetArena();
			}
		}

		public override bool CheckActive()
		{
			return false;
		}

		public override bool NeedSaving()
		{
			return true;
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			ExtraRadius += hit.Damage * 0.5f;
		}

		public void CreateArena()
		{
			for(int x = -60; x <= 60; x++)
			{
				for(int y = -60; y <= 60; y++)
				{
					var off = new Vector2(x, y);
					var dist = off.LengthSquared();

					if (dist <= Math.Pow(50, 2))
					{
						var tile = Main.tile[(int)home.X / 16 + x, (int)home.Y / 16 + y];

						tile.LiquidAmount = 0;

						if (tile.HasTile && !tile.IsActuated)
						{
							tile.IsActuated = true;							
							tilesChanged.Add(new Point16(x, y));
						}
					}

					if (dist > Math.Pow(50, 2) && dist <= Math.Pow(60, 2))
					{
						var tile = Main.tile[(int)home.X / 16 + x, (int)home.Y / 16 + y];

						if (!tile.HasTile)
						{
							tile.HasTile = true;
							tile.TileType = (ushort)ModContent.TileType<BrainBlocker>();
							tile.Slope = Terraria.ID.SlopeType.Solid;
							WorldGen.TileFrame((int)home.X / 16 + x, (int)home.Y / 16 + y);
							tilesChanged.Add(new Point16(x, y));
						}
					}
				}
			}

			for(int k = 0; k < 10; k++)
			{
				float off = 1 - (k / 9f) * 2f;
				Vector2 pos = NPC.Center + Vector2.UnitY * off * 750;
				int i = NPC.NewNPC(null, (int)pos.X, (int)pos.Y, ModContent.NPCType<BrainPlatform>());

				float a = off * 750f;
				float h = 750f;

				Main.npc[i].width = (int)((float)Math.Sqrt(Math.Pow(h, 2) - Math.Pow(a, 2)) * 2);
				Main.npc[i].Center = pos;
			}

			active = true;
		}

		public void ResetArena()
		{
			foreach(Point16 point in tilesChanged)
			{
				var tile = Main.tile[(int)home.X / 16 + point.X, (int)home.Y / 16 + point.Y];

				if (tile.IsActuated)
					tile.IsActuated = false;

				if (tile.TileType == ModContent.TileType<BrainBlocker>())
					tile.HasTile = false;
			}

			foreach(NPC npc in Main.npc.Where(n => n.active && n.type == ModContent.NPCType<BrainPlatform>()))
			{
				npc.active = false;
			}

			tilesChanged.Clear();
			active = false;
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			return Helpers.Helper.CheckCircularCollision(NPC.Center, 64, target.Hitbox);
		}

		public override void SaveData(TagCompound tag)
		{
			tag.Add("active", active);
			tag.Add("tiles", tilesChanged);
			tag.Add("home", home);
		}

		public override void LoadData(TagCompound tag)
		{
			active = tag.GetBool("active");
			tilesChanged = tag.GetList<Point16>("tiles") as List<Point16>;
			home = tag.Get<Vector2>("home");
		}

		private void DrawAura(SpriteBatch sb)
		{
			Texture2D glow = ModContent.Request<Texture2D>("StarlightRiver/Assets/Keys/GlowAlpha").Value;
			var color = Color.White;
			color.A = 0;

			foreach (TheThinker thinker in toRender)
			{
				for (int k = 0; k < 8; k++)
				{
					sb.Draw(glow, thinker.NPC.Center - Main.screenPosition, null, color, 0, glow.Size() / 2f, (200 + thinker.ExtraRadius) * 4 / glow.Width, 0, 0);
				}
				//sb.Draw(glow, thinker.NPC.Center - Main.screenPosition, null, color, 0, glow.Size() / 2f, 6f, 0, 0);
				//sb.Draw(glow, thinker.NPC.Center - Main.screenPosition, null, color, 0, glow.Size() / 2f, 6f, 0, 0);
			}

			toRender.RemoveAll(n => n is null || !n.NPC.active);
		}

		private void DrawMe(SpriteBatch sb)
		{
			Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

			foreach (TheThinker thinker in toRender)
			{
				sb.Draw(tex, thinker.NPC.Center - Main.screenPosition, null, Color.White, 0, tex.Size() / 2f, 1, 0, 0);
			}
		}
	}
}
