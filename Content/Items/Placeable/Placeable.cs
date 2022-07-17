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
using Terraria.GameContent.Creative;

namespace CollateralCascade.Content.Items.Placeable
{
    public class XenianCrystal : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Xenian Shard");
            //Tooltip.SetDefault("'She's debeaked and completely harmless.'");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99;
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.XenianCrystalBlock>();
            Item.maxStack = 999;
            Item.width = 24;
            Item.height = 24;
            Item.value = 75;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<ExhaustedXenianCrystal>(), 2);
            recipe.AddIngredient(ItemID.SoulofLight, 3);
            recipe.AddTile(TileID.AlchemyTable);
            recipe.Register();
        }
    }

    public class AssembledXenianCrystal : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Xenian Crystal");
            //Tooltip.SetDefault("'She's debeaked and completely harmless.'");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 10;
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.AssembledXenianCrystalBlock>();
            Item.maxStack = 99;
            Item.width = 44;
            Item.height = 44;
            Item.value = 75 * 9;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<XenianCrystal>(), 9);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}