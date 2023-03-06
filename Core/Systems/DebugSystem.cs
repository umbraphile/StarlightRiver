using StarlightRiver.Core.Loaders;
using System.Linq;
using System.Reflection;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader.Core;
using static Terraria.ModLoader.Core.TmodFile;

namespace StarlightRiver.Core.Systems
{
	internal class DebugSystem : ModSystem
	{
		int timer = 0;

		public override void Load()
		{
			On.Terraria.Main.Update += DoUpdate;
			On.Terraria.Main.DrawInterface += DrawDebugMenu;
		}

		private void DrawDebugMenu(On.Terraria.Main.orig_DrawInterface orig, Main self, GameTime gameTime)
		{
			orig(self, gameTime);

			if (!StarlightRiver.debugMode || Main.playerInventory)
				return;

			string menu = "Debug mode options:\n " +
				"Y: Hold to speed up game\n " +
				"U: Hold to slow down game\n " +
				"P: Press to change difficulty\n " +
				"L: Reload shaders";

			Main.spriteBatch.Begin();
			Utils.DrawBorderString(Main.spriteBatch, menu, new Vector2(32, 120), new Color(230, 230, 255));
			Main.spriteBatch.End();
		}

		private void DoUpdate(On.Terraria.Main.orig_Update orig, Main self, GameTime gameTime)
		{
			if (Main.LocalPlayer.position == Vector2.Zero || float.IsNaN(Main.LocalPlayer.position.X) || float.IsNaN(Main.LocalPlayer.position.Y))
				Main.LocalPlayer.position = new Vector2(Main.spawnTileX * 16, Main.spawnTileY * 16);

			if (!StarlightRiver.debugMode)
			{
				orig(self, gameTime);
				return;
			}

			if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Y)) //Boss Speed Up Key
			{
				for (int k = 0; k < 8; k++)
				{
					orig(self, gameTime);
				}
			}

			if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.U)) //Boss Slow Down Key
			{
				if (timer % 2 == 0)
					orig(self, gameTime);

				timer++;

				return;
			}

			if (Main.oldKeyState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.P) && Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.P)) //Difficulty toggle key
			{
				if (!Main.expertMode)
				{
					Main.GameMode = GameModeID.Expert;
					Main.NewText("The game is now in expert mode.", new Color(255, 150, 0));
				}
				else if (!Main.masterMode)
				{
					Main.GameMode = GameModeID.Master;
					Main.NewText("The game is now in master mode.", new Color(255, 0, 0));
				}
				else
				{
					Main.GameMode = GameModeID.Normal;
					Main.NewText("The game is now in normal mode.", new Color(180, 180, 255));
				}
			}

			if (Main.oldKeyState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.L) && Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.L)) //Shader reload key
			{
				MethodInfo info = typeof(Mod).GetProperty("File", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true);
				var file = (TmodFile)info.Invoke(StarlightRiver.Instance, null);

				System.Collections.Generic.IEnumerable<FileEntry> shaders = file.Where(n => n.Name.StartsWith("Effects/") && n.Name.EndsWith(".xnb"));

				foreach (FileEntry entry in shaders)
				{
					string name = entry.Name.Replace(".xnb", "").Replace("Effects/", "");
					string path = entry.Name.Replace(".xnb", "");

					Filters.Scene[name].Deactivate();

					ShaderLoader.LoadShader(name, path);
				}
			}

			orig(self, gameTime);
		}
	}
}
