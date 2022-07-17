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

namespace CollateralCascade.Content.Items
{
    public class HeadcrabBeak : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Headcrab Beak");
            Tooltip.SetDefault("'She's debeaked and completely harmless.'");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99;
        }

        public override void SetDefaults()
        {
            Item.maxStack = 999;
            Item.width = 12;
            Item.height = 12;
            Item.value = 10;
        }

        public override void AddRecipes()
        { }
    }

    public class SpectroRay : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Anti-Mass Spectro-Ray");
            Tooltip.SetDefault("Used to analyze xenian crystals\n'It's probably not a problem, probably.'");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Thrust;
            Item.useTurn = true;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.autoReuse = false;

            Item.maxStack = 1;
            Item.width = 12;
            Item.height = 12;
            Item.value = 1000;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.DirtBlock, 1);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }

    public class ExhaustedXenianCrystal : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Xenian Shard");
            Tooltip.SetDefault("Exhausted");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99;
        }

        public override void SetDefaults()
        {
            Item.maxStack = 999;
            Item.width = 24;
            Item.height = 24;
            Item.value = 5;
        }

        public override void AddRecipes()
        { }
    }
}