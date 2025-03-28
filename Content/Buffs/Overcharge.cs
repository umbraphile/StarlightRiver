﻿using StarlightRiver.Helpers;
using System.Collections.Generic;
using System.Linq;
using static Terraria.ModLoader.ModContent;

namespace StarlightRiver.Content.Buffs
{
	class Overcharge : SmartBuff
	{
		public Overcharge() : base("Overcharge", "Reduces defense by 75%", true) { }

		public override string Texture => AssetDirectory.Buffs + "Overcharge";

		public override void Load()
		{
			StarlightNPC.ModifyIncomingHitEvent += ReduceDefense;
		}

		private void ReduceDefense(NPC npc, ref NPC.HitModifiers modifiers)
		{
			if (Inflicted(npc))
				modifiers.Defense.Base /= 4;
		}

		public override void Update(Player Player, ref int buffIndex)
		{
			Player.statDefense /= 4;

			if (Main.rand.NextBool(10))
			{
				Vector2 pos = Player.Center + Vector2.One.RotatedByRandom(6.28f) * Main.rand.NextFloat(Player.width);
				DustHelper.SpawnElectricityPattern(pos, pos + Vector2.One.RotatedByRandom(6.28f) * Main.rand.Next(5, 10), DustType<Content.Dusts.Electric>(), 0.8f, 3, default, 0.25f);
			}

			return;
		}

		public override void Update(NPC NPC, ref int buffIndex)
		{
			if (Main.rand.NextBool(10))
			{
				Vector2 pos = NPC.Center + Vector2.One.RotatedByRandom(6.28f) * Main.rand.NextFloat(NPC.width);
				DustHelper.SpawnElectricityPattern(pos, pos + Vector2.One.RotatedByRandom(6.28f) * Main.rand.Next(5, 10), DustType<Content.Dusts.Electric>(), 0.8f, 3, default, 0.25f);
			}

			return;
		}
	}
}