using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Terraria.IO;
using Terraria.ObjectData;
using Terraria.DataStructures;

using CollateralCascade.Content.Items;

namespace CollateralCascade.Content.Tiles
{
    public class XenianCrystalBlock : ModTile
    {
		public override void SetStaticDefaults()
		{
			TileID.Sets.Ore[Type] = true;
			Main.tileSpelunker[Type] = true;
			Main.tileOreFinderPriority[Type] = 450;
			Main.tileShine2[Type] = true;
			Main.tileShine[Type] = 975;
			Main.tileMergeDirt[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileLighted[Type] = true;

			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Xenian Shards");
			AddMapEntry(new Color(255, 176, 5), name);

			DustType = 84;
			ItemDrop = ModContent.ItemType<Items.Placeable.XenianCrystal>();
			HitSound = SoundID.Tink;
			MineResist = 4f;
			MinPick = 150;
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
			r = 0.5f;
			g = 0.325f;
			b = 0.01f;
		}

	}

	public class AssembledXenianCrystalBlock : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileObsidianKill[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
			TileObjectData.newTile.Origin = new Point16(1, 2);
			TileObjectData.newTile.DrawYOffset = 3;
			Main.tileSolid[Type] = false;
			Main.tileBlockLight[Type] = true;
			Main.tileLighted[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;
			TileObjectData.addTile(Type);


			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Xenian Crystal");
			AddMapEntry(new Color(255, 200, 35), name);

			DustType = 84;
			ItemDrop = ModContent.ItemType<Items.Placeable.XenianCrystal>();
			HitSound = SoundID.Tink;
			MineResist = 10f;
			MinPick = 150;
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			r = 1f;
			g = 0.65f;
			b = 0.02f;
		}

		public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;
			player.noThrow = 2;
			player.cursorItemIconEnabled = true;
			player.cursorItemIconID = ModContent.ItemType<SpectroRay>();

			if (player.HeldItem.type == ModContent.ItemType<SpectroRay>())
			{
				float baseChance = 100;

				if (!Main.hardMode)
					baseChance *= 0.5f;

				if (Main.bloodMoon)
					baseChance *= 0.75f;

				if (Main.expertMode)
					baseChance *= 0.75f;

				if (Main.masterMode)
					baseChance *= 0.5f;

				if (Main.windSpeedCurrent > 0)
					baseChance *= 2;

				if (Main.windSpeedCurrent < 0)
					baseChance *= 0.5f;

				baseChance += Main.numClouds;

				baseChance *= Main.numStars / 100;

				int chance = (int)baseChance;

				float displayChance = MathF.Round((100f / baseChance) * 10000f) / 10000f;

				player.cursorItemIconText = "Right click to analyze.\nPotential risk of resonance cascade: " + displayChance + "%.";
			}
			else
			{
				player.cursorItemIconText = "You need a Anti-Mass Spectro-Ray to analyze this crystal.";
			}
        }

        public override bool RightClick(int i, int j)
		{
			Player player = Main.LocalPlayer;
			if (player.HeldItem.type == ModContent.ItemType<SpectroRay>())
            {
				float baseChance = 100;

				if (!Main.hardMode)
					baseChance *= 0.5f;

				if (Main.bloodMoon)
					baseChance *= 0.75f;

				if (Main.expertMode)
					baseChance *= 0.75f;

				if (Main.masterMode)
					baseChance *= 0.5f;

				if (Main.windSpeedCurrent > 0)
					baseChance *= 2;

				if (Main.windSpeedCurrent < 0)
					baseChance *= 0.5f;

				baseChance += Main.numClouds;

				baseChance *= Main.numStars / 100;

				int chance = (int)baseChance;

				Main.NewText("As you analyzed the crystal, it got dimmer.\n(You can no longer analyze it.)");
				ItemDrop = -1;
				WorldGen.KillTile(i, j, false, false, true);
				WorldGen.PlaceTile(i, j, ModContent.TileType<AssembledXenianCrystalBlock_Exhausted>(), false, true);
				ItemDrop = ModContent.ItemType<Items.Placeable.XenianCrystal>();

				if (Main.rand.NextBool(chance))
                {
					//resonance cascade stuff
					Main.NewText("You feel the world shake as rifts begin opening across the world...");
					Main.windSpeedCurrent = 0.8f;
                }
				else
                {
					switch(Main.rand.Next(2))
                    {
						case 0:
							//cool thing happens
							break;
						case 1:
							//cool thing happens
							break;
						default:
							//nothing happens
							Main.NewText("An error has occured.");
							break;
                    }
				}
				return true;
            }

			return false;
		}

	}

	public class AssembledXenianCrystalBlock_Exhausted : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileObsidianKill[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
			TileObjectData.newTile.Origin = new Point16(1, 2);
			TileObjectData.newTile.DrawYOffset = 3;
			Main.tileSolid[Type] = false;
			Main.tileBlockLight[Type] = true;
			Main.tileLighted[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;
			TileObjectData.addTile(Type);


			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Exhausted Xenian Crystal");
			AddMapEntry(new Color(225, 205, 156), name);

			ItemDrop = ModContent.ItemType<Items.ExhaustedXenianCrystal>();
			HitSound = SoundID.Tink;
			MineResist = 10f;
			MinPick = 150;
		}
	}

	public class ExampleOreSystem : ModSystem
	{
		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
		{
			int ShiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Shinies"));

			if (ShiniesIndex != -1)
			{
				tasks.Insert(ShiniesIndex + 1, new XenianCrystalPass("Xenian Crystals", 237.4298f));
			}
		}
	}
	public class XenianCrystalPass : GenPass
	{
		public XenianCrystalPass(string name, float loadWeight) : base(name, loadWeight)
		{
		}

		protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = "Forming xenian shards";

			for (int k = 0; k < (int)(Main.maxTilesX * Main.maxTilesY * 6E-05); k++)
			{
				int x = WorldGen.genRand.Next(0, Main.maxTilesX);
				int y = WorldGen.genRand.Next((int)WorldGen.worldSurfaceLow, Main.maxTilesY);

				WorldGen.TileRunner(x, y, WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(2, 6), ModContent.TileType<XenianCrystalBlock>());
			}
		}
	}
}