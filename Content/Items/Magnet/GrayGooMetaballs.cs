﻿using StarlightRiver.Core.Loaders;
using StarlightRiver.Core.Systems.LightingSystem;
using StarlightRiver.Core.Systems.MetaballSystem;
using System.Linq;
using Terraria.Graphics.Effects;

namespace StarlightRiver.Content.Items.Magnet
{
	internal class GrayGooMetaballs : MetaballActor
	{
		public static bool visible;

		public int DustType => ModContent.DustType<GrayGooDust>();

		public int DustType2 => ModContent.DustType<GrayGooSplashDust>();

		public Color InteriorColor => Color.Gray;

		public override Color OutlineColor => Color.DarkGray;

		public override bool OverEnemies => true;

		public override bool Active => visible;

		public override void DrawShapes(SpriteBatch spriteBatch)
		{
			Effect borderNoise = ShaderLoader.GetShader("BorderNoise").Value;

			if (borderNoise is null)
				return;

			Texture2D tex = Assets.Masks.GlowVerySoft.Value;
			Texture2D harshTex = Assets.Masks.GlowHarsh.Value;

			borderNoise.Parameters["offset"].SetValue((float)Main.time / 100f);

			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
			borderNoise.CurrentTechnique.Passes[0].Apply();

			foreach (Dust dust in Main.dust)
			{
				if (dust.active && dust.type == DustType)
				{
					borderNoise.Parameters["offset"].SetValue(dust.rotation);
					spriteBatch.Draw(tex, (dust.position - Main.screenPosition) / 2, null, Color.White * 0.9f, dust.rotation, tex.Size() / 2, dust.scale * 0.25f, SpriteEffects.None, 0);
				}

				if (dust.active && dust.type == DustType2)
				{
					borderNoise.Parameters["offset"].SetValue(dust.rotation);
					for (int i = 0; i < 5; i++)
					{
						spriteBatch.Draw(harshTex, (dust.position - Main.screenPosition) / 2, null, Color.White, dust.rotation, harshTex.Size() / 2, dust.scale * 0.25f, SpriteEffects.None, 0);
					}
				}
			}

			spriteBatch.End();
			spriteBatch.Begin();
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Texture2D target)
		{
			if (GrayGooProj.NPCTarget == null)
				return false;

			Effect effect = ShaderLoader.GetShader("GrayGooShader").Value;

			if (effect != null)
			{
				effect.Parameters["NPCTarget"].SetValue(GrayGooProj.NPCTarget.RenderTarget);
				effect.Parameters["threshhold"].SetValue(0.95f);
				effect.Parameters["screenSize"].SetValue(Main.ScreenSize.ToVector2() * 2);
				effect.Parameters["time"].SetValue((float)Main.timeForVisualEffects * 0.1f);
				effect.Parameters["min"].SetValue(0f);
				effect.Parameters["max"].SetValue(1f);

				effect.Parameters["noisiness"].SetValue(143.578348f);

				effect.Parameters["eyeThreshhold"].SetValue(0.1f);
				effect.Parameters["eyeColor"].SetValue(new Vector4(0, 1, 1, 1));
				effect.Parameters["eyeChangeRate"].SetValue(0.1f);

				spriteBatch.End();
				spriteBatch.Begin(default, default, Main.DefaultSamplerState, default, RasterizerState.CullNone, effect);

				spriteBatch.Draw(Target.RenderTarget, position: Vector2.Zero, color: Color.White);

				spriteBatch.End();
				spriteBatch.Begin();
			}

			return false;
		}

		public override bool PostDraw(SpriteBatch spriteBatch, Texture2D target)
		{
			var sourceRect = new Rectangle(0, 0, target.Width, target.Height);
			LightingBufferRenderer.DrawWithLighting(target, Vector2.Zero, sourceRect, InteriorColor, 0, Vector2.Zero, 2f);

			visible = false;
			return false;
		}
	}
}