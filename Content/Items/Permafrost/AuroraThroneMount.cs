﻿using Microsoft.Xna.Framework;
using Terraria.Graphics.Effects;
using StarlightRiver.Core;
using StarlightRiver.Core.Systems.CombatMountSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;

namespace StarlightRiver.Content.Items.Permafrost
{
	internal class AuroraThroneMount : CombatMount
	{
		List<Projectile> buffedMinions = new List<Projectile>();

		public override string PrimaryIconTexture => AssetDirectory.PermafrostItem + "AuroraThroneMountPrimary";
		public override string SecondaryIconTexture => AssetDirectory.PermafrostItem + "AuroraThroneMountSecondary";

		public override void SetDefaults()
		{
			primarySpeedCoefficient = 26;
			primaryCooldownCoefficient = 20;
			secondaryCooldownCoefficient = 600;
			secondarySpeedCoefficient = 120;
			damageCoefficient = 42;
			autoReuse = true;
		}

		public override void PostUpdate(Player player)
		{
			var mp = player.GetModPlayer<CombatMountPlayer>();
			var progress = 1 - Math.Max(0, (mp.mountingTime - 15) / 15f);

			if (progress < 1)
			{
				for (int k = 0; k < 2; k++)
				{
					var pos = player.Center + new Vector2(0, 58 - (int)(progress * 58));
					Dust.NewDustPerfect(pos + Vector2.UnitX * Main.rand.NextFloat(-20, 20), ModContent.DustType<Dusts.Cinder>(), Main.rand.NextVector2Circular(1, 1), 0, new Color(255, 255, 200), 0.5f);
				}
			}

			if(Main.rand.NextBool(2))
				Dust.NewDustPerfect(player.Center + new Vector2(-6 * player.direction + Main.rand.NextFloat(-20, 20), 0), ModContent.DustType<Dusts.Glow>(), Vector2.UnitY * Main.rand.NextFloat(1, 2), 0, Main.DiscoColor, 0.25f);

			player.Hitbox.Offset(new Point(0, -32));

			player.fullRotation = -0.2f * player.direction + player.velocity.X * -0.05f;
			player.fullRotationOrigin = new Vector2(player.width / 2, player.height);

			player.gfxOffY = (float)Math.Sin(Main.GameUpdateCount * 0.1f) * 4f;

			if (player.velocity.Y > 0)
				player.velocity.Y *= 0.9f;

			Lighting.AddLight(player.Center, Color.Lerp(Color.White, Main.DiscoColor, 0.5f).ToVector3());
		}

		public override void OnStartPrimaryAction(Player player)
		{

		}

		public override void PrimaryAction(int timer, Player player)
		{
			for (int k = 0; k < 4; k++)
			{
				int check = (int)(k / 4f * MaxPrimaryTime);

				if (timer == check)
				{
					var vel = Vector2.Normalize(Main.MouseWorld - player.Center).RotatedByRandom(0.5f) * Main.rand.NextFloat(6, 9);
					Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, vel, ModContent.ProjectileType<AuroraThroneMountWhip>(), damageCoefficient / 4, 10, player.whoAmI, Main.rand.Next(80, 120), Main.rand.NextBool() ? 1 : -1);
				}
			}
		}

		public override void OnStartSecondaryAction(Player player)
		{
			foreach (Projectile proj in Main.projectile.Where(n => n.active && n.owner == player.whoAmI && n.minion))
			{
				proj.extraUpdates++;
				buffedMinions.Add(proj);
			}
		}

		public override void SecondaryAction(int timer, Player player)
		{
			var animTime = secondarySpeedCoefficient / 4f;
			var time = Math.Max(0, (timer - animTime * 3) / (animTime));

			if (time > 0)
				Filters.Scene.Activate("Shockwave", player.Center).GetShader().UseProgress(2f).UseIntensity(100 - time * 100).UseDirection(new Vector2(0.1f - time * 0.1f, 0.02f - time * 0.02f));
			else
				Filters.Scene.Deactivate("Shockwave");

			if (timer == 1)
			{
				foreach (Projectile proj in buffedMinions)
				{
					proj.extraUpdates--;

					if (proj.extraUpdates < 0)
						proj.extraUpdates = 0;
				}

				buffedMinions.Clear();
			}
		}
	}

	internal class AuroraThroneMountData : ModMount
	{
		public override string Texture => AssetDirectory.PermafrostItem + "AuroraThroneMount";

		public override void SetMount(Player player, ref bool skipDust)
		{
			skipDust = true;
		}

		public override void Dismount(Player player, ref bool skipDust)
		{
			skipDust = true;
		}

		public override bool Draw(List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow)
		{
			SetStaticDefaults();

			var tex = ModContent.Request<Texture2D>(Texture).Value;
			var tex2 = ModContent.Request<Texture2D>(Texture + "Glow").Value;
			var tex3 = ModContent.Request<Texture2D>(AssetDirectory.SquidBoss + "PortalGlow").Value;
			var mp = drawPlayer.GetModPlayer<CombatMountPlayer>();
			var progress = 1 - Math.Max(0, (mp.mountingTime - 15) / 15f);

			float sin = (float)Math.Sin(Main.GameUpdateCount * 0.1f);
			var frameNumber = (int)(Main.GameUpdateCount * 0.15f) % 6;
			var pos = drawPlayer.Center - Main.screenPosition + new Vector2(-12 * drawPlayer.direction, 76 - (int)(progress * 68));
			var source = new Rectangle(0, frameNumber * 58 + 58 - (int)(progress * 58), 64, (int)(progress * 58));
			var source2 = new Rectangle(0, frameNumber * 58 + 58 - (int)(progress * 58), 64, 2);

			if (mp.mountingTime <= 0)
				pos.Y += drawPlayer.gfxOffY;	

			var color = Main.DiscoColor;
			color.A = 0;

			var glowRot = 3.14f + 0.2f * drawPlayer.direction;
			playerDrawData.Add(new DrawData(tex3, pos, null, color * (0.25f + 0.05f * sin), glowRot, tex3.Size() / 2, 0.32f + 0.025f * sin, spriteEffects, 0));

			float rot = 0.2f * drawPlayer.direction;
			playerDrawData.Add(new DrawData(tex, pos, source, drawColor, rot, new Vector2(32, 58), 1, spriteEffects, 0));
			playerDrawData.Add(new DrawData(tex2, pos, source, Main.DiscoColor, rot, new Vector2(32, 58), 1, spriteEffects, 0));

			if (progress < 1)
				playerDrawData.Add(new DrawData(tex2, pos, source2, Color.White, rot, new Vector2(32, 58), 1, spriteEffects, 0));

			Main.NewText(drawPlayer.Hitbox);

			return false;
		}

		public override void SetStaticDefaults()
		{
			MountData.jumpHeight = 6;
			MountData.acceleration = 0.35f;
			MountData.jumpSpeed = 10f;
			MountData.blockExtraJumps = false;
			MountData.heightBoost = 0;
			MountData.runSpeed = 5f;

			// Frame data and player offsets
			MountData.totalFrames = 1;
			MountData.playerYOffsets = Enumerable.Repeat(26, MountData.totalFrames).ToArray();
			MountData.xOffset = 13;
			MountData.yOffset = -62;
			MountData.playerHeadOffset = 22;
			MountData.bodyFrame = 3;
			// Standing
			MountData.standingFrameCount = 1;
			MountData.standingFrameDelay = 12;
			MountData.standingFrameStart = 0;
			// Running
			MountData.runningFrameCount = 1;
			MountData.runningFrameDelay = 12;
			MountData.runningFrameStart = 0;
			// Flying
			MountData.flyingFrameCount = 0;
			MountData.flyingFrameDelay = 0;
			MountData.flyingFrameStart = 0;
			// In-air
			MountData.inAirFrameCount = 1;
			MountData.inAirFrameDelay = 12;
			MountData.inAirFrameStart = 0;
			// Idle
			MountData.idleFrameCount = 1;
			MountData.idleFrameDelay = 12;
			MountData.idleFrameStart = 0;
			MountData.idleFrameLoop = true;
			// Swim
			MountData.swimFrameCount = MountData.inAirFrameCount;
			MountData.swimFrameDelay = MountData.inAirFrameDelay;
			MountData.swimFrameStart = MountData.inAirFrameStart;

			if (!Main.dedServ)
			{
				MountData.textureWidth = MountData.backTexture.Width() + 20;
				MountData.textureHeight = MountData.backTexture.Height();
			}
		}
	}

	internal class AuroraThroneMountItem : CombatMountItem
	{
		public override int MountType => ModContent.MountType<AuroraThroneMountData>();

		public override Type CombatMountType => typeof(AuroraThroneMount);

		public override string Texture => AssetDirectory.PermafrostItem + Name;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Aurora Crown");
			Tooltip.SetDefault("Summons an aurora throne combat mount\nLashes out with whip-like apendages\nRight click to summon explosive auroralings");
		}
	}
}
