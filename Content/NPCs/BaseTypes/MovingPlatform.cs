﻿using System;
using Terraria.GameContent.Bestiary;
using Terraria.ID;

namespace StarlightRiver.Content.NPCs.BaseTypes
{
	internal abstract class MovingPlatform : ModNPC
	{
		Vector2 prevPos;

		public bool beingStoodOn;

		public bool dontCollide = false;

		public virtual bool CanFallThrough => true;

		public virtual void SafeSetDefaults() { }

		public virtual void SafeAI() { }

		public override bool? CanBeHitByProjectile(Projectile Projectile)
		{
			return false;
		}

		public override bool? CanBeHitByItem(Player Player, Item Item)
		{
			return false;
		}

		public override bool CheckActive()
		{
			return false;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("");
		}

		public sealed override void SetDefaults()
		{
			SafeSetDefaults();

			NPC.lifeMax = 10;
			NPC.immortal = true;
			NPC.dontTakeDamage = true;
			NPC.noGravity = true;
			NPC.knockBackResist = 0; //very very important!!
			NPC.aiStyle = -1;
			NPC.damage = 0;
			NPC.netAlways = true;

			for (int k = 0; k < NPC.buffImmune.Length; k++)
			{
				NPC.buffImmune[k] = true;
			}
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			database.Entries.Remove(bestiaryEntry);
		}

		public sealed override void AI()
		{
			SafeAI();

			//TODO: More elegant sync guarantee later perhaps, this should ensure platforms always eventually exist in MP
			if (Main.netMode == NetmodeID.Server && Main.GameUpdateCount % 60 == 0)
				NPC.netUpdate = true;

			if (!dontCollide)
			{
				float yDistTraveled = NPC.position.Y - prevPos.Y;

				if (NPC.velocity != Vector2.Zero && NPC.velocity.Y < -1f && yDistTraveled < NPC.velocity.Y * 1.5 && yDistTraveled > NPC.velocity.Y * 6)
				{
					//this loop outside of the normal moving platform loop in Mechanics is mainly for multiPlayer with some potential for extreme lag situations on fast platforms
					//what is happening is that when terraria skips frames (or lags in mp) they add the NPC velocity multiplied by the skipped frames up to 5x a normal frame until caught up, but only run the ai once
					//so we can end up with frames where the platform skips 5x its normal velocity likely clipping through Players since the platform is thin.
					//to solve this, the collision code takes into account the previous platform position accessed by this AI for the hitbox to cover the whole travel from previous fully processed frame.
					//only handling big upwards y movements since the horizontal skips don't seem as jarring to the user since platforms tend to be wide, and vertical down skips aren't jarring since Player drops onto platform anyway instead of clipping through.
					for (int k = 0; k < Main.maxPlayers; k++)
					{
						Player player = Main.player[k];

						if (!player.active || player.dead || player.GoingDownWithGrapple || CanFallThrough && player.GetModPlayer<StarlightPlayer>().platformTimer > 0)
							continue;

						if (player.position.Y <= NPC.position.Y && player.velocity.Y >= 0 && !player.justJumped)
						{
							var PlayerRect = new Rectangle((int)player.position.X, (int)player.position.Y + player.height, player.width, 1);
							var NPCRect = new Rectangle((int)NPC.position.X, (int)NPC.position.Y, NPC.width, 8 + (player.velocity.Y > 0 ? (int)player.velocity.Y : 0) + (int)Math.Abs(yDistTraveled));

							if (PlayerRect.Intersects(NPCRect))
							{
								player.velocity.Y = 0;
								player.position.Y = NPC.position.Y - player.height + 4;
								player.position += NPC.velocity;
							}
						}
					}
				}
			}

			prevPos = NPC.position;
			beingStoodOn = false;
		}
	}
}