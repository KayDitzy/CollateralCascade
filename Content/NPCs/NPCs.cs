using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.GameContent.Personalities;
using Terraria.DataStructures;
using System.Collections.Generic;
using ReLogic.Content;
using Terraria.ModLoader.IO;

using Terraria.ModLoader.Utilities;
using CollateralCascade.Content.Items;

namespace CollateralCascade.Content.NPCs
{
	public class Headcrab : ModNPC
	{
		private enum ActionState
		{
			Idle,
			Notice,
			Attack,
			Fall
		}

		private enum Frame
		{
			Idle,
			Walking1,
			Walking2,
			Walking3,
			Airborne
		}

		public ref float AI_State => ref NPC.ai[0];
		public ref float AI_Timer => ref NPC.ai[1];
		public ref float AI_WalkingTimer => ref NPC.ai[2];
		public ref float AI_WalkingFrame => ref NPC.ai[3];

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 5;

			NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData
			{
				SpecificallyImmuneTo = new int[] {

				}
			});

			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new(0)
			{
				Velocity = 1f
			};
		}

		public override void SetDefaults()
		{
			NPC.width = 30; 
			NPC.height = 30;
			NPC.aiStyle = -1; 
			NPC.damage = 7;
			NPC.defense = 2; 
			NPC.lifeMax = 15; 
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = 15f;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Underground,

				new FlavorTextBestiaryInfoElement("The resonance cascade has resulted in bringing one of Xen's most common creatures, the parasitic Headcrab."),

				new FlavorTextBestiaryInfoElement("Allowing this creature to directly attack the rear of your cranium will not result in a pleasant outcome.")

			});
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			if (NPCID.Sets.NPCBestiaryDrawOffset.TryGetValue(Type, out NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers))
			{
				NPCID.Sets.NPCBestiaryDrawOffset.Remove(Type);
				NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
			}

			return true;
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			//uncomment on release
			//if (Main.hardMode)
				return SpawnCondition.Underground.Chance * 5f;

			return 0;
		}

		public override void AI()
		{
			switch (AI_State)
			{
				case (float)ActionState.Idle:
					Idle();
					break;
				case (float)ActionState.Notice:
					Walk();
					break;
				case (float)ActionState.Attack:
					Lunge();
					break;
				case (float)ActionState.Fall:
					if (NPC.velocity.Y == 0)
					{
						NPC.velocity.X = 0;
						AI_State = (float)ActionState.Idle;
						AI_Timer = 0;
					}

					break;
			}
		}

		public override void FindFrame(int frameHeight)
		{
			NPC.spriteDirection = NPC.direction;

			if (NPC.direction == Main.player[NPC.target].direction && Main.player[NPC.target].headcovered == false)
				NPC.damage = 70;
			else if (NPC.direction == Main.player[NPC.target].direction)
				NPC.damage = 38;
			else
				NPC.damage = 7;

			switch (AI_State)
			{
				case (float)ActionState.Idle:
					NPC.frame.Y = (int)Frame.Idle * frameHeight;
					break;
				case (float)ActionState.Notice:
					if (AI_Timer < 10)
					{
						AI_WalkingTimer++;
						if (AI_WalkingTimer == 15)
						{
							AI_WalkingFrame = 1;
						}
						else if (AI_WalkingTimer == 30)
						{
							AI_WalkingFrame = 2;
						}
						else if (AI_WalkingTimer == 45)
						{
							AI_WalkingFrame = 1;
						}
						else if (AI_WalkingTimer == 60)
						{
							AI_WalkingFrame = 0;
							AI_WalkingTimer = 0;
						}

						NPC.frame.Y = ((int)Frame.Walking1 + (int)AI_WalkingFrame) * frameHeight;
					}
					else
					{
						NPC.frame.Y = (int)Frame.Idle * frameHeight;
					}

					break;
				case (float)ActionState.Attack:
					NPC.frame.Y = (int)Frame.Airborne * frameHeight;
					break;
				case (float)ActionState.Fall:
					NPC.frame.Y = (int)Frame.Idle * frameHeight;
					break;
			}
		}

		public override bool? CanFallThroughPlatforms()
		{
			if (AI_State == (float)ActionState.Fall && NPC.HasValidTarget && Main.player[NPC.target].Top.Y > NPC.Bottom.Y)
			{
				return true;
			}

			return false;
		}

		private void Idle()
		{
			NPC.TargetClosest(true);

			if (NPC.HasValidTarget && Main.player[NPC.target].Distance(NPC.Center) < 500f)
			{
				AI_State = (float)ActionState.Notice;
				AI_Timer = 0;
			}
		}

		private void Walk()
		{
			if (Main.player[NPC.target].Distance(NPC.Center) < 250f)
			{
				AI_Timer++;

				if (AI_Timer >= 40)
				{
					AI_State = (float)ActionState.Attack;
					AI_Timer = 0;
				}
			}
			else
			{
				NPC.TargetClosest(true);
				NPC.velocity = new Vector2(NPC.direction * 1, NPC.velocity.Y);

				if (!NPC.HasValidTarget || Main.player[NPC.target].Distance(NPC.Center) > 500f)
				{
					AI_State = (float)ActionState.Idle;
					AI_Timer = 0;
				}
			}
		}

		private void Lunge()
		{
			AI_Timer++;

			if (AI_Timer == 1)
			{
				float headVelocity = (NPC.direction * 5.5f) * (Main.player[NPC.target].Distance(NPC.Center) / 250f);
				NPC.velocity = new Vector2(headVelocity, -6f);
			}
			else if (AI_Timer > 40)
			{
				AI_State = (float)ActionState.Fall;
				AI_Timer = 0;
			}
			else
            {
				NPC.velocity += new Vector2(NPC.direction * 0.1f, 0);
            }
		}

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (target.dead && NPC.direction == target.direction)
            {
                var entitySource = NPC.GetSource_FromAI();
                var zomb = NPC.NewNPC(entitySource, (int)target.position.X, (int)target.position.Y, ModContent.NPCType<HeadcrabZombie>());
                NPC zombNPC = Main.npc[zomb];
                zombNPC.direction = NPC.direction;
                zombNPC.lifeMax = (int)target.statLifeMax / 2;
                zombNPC.life = zombNPC.lifeMax;
                zombNPC.defense = (int)target.statDefense / 2;
                NPC.position = new Vector2(0, 0);
                NPC.life = 0;
                NPC.despawnEncouraged = true;
            }
        }

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<HeadcrabBeak>(), 3)); // 1% chance to drop Confetti
		}
	}
	public class HeadcrabZombie : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Headcrab Zombie");

			Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.Zombie];

			NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
			{
				Velocity = 1f
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
		}

		public override void SetDefaults()
		{
			NPC.width = 18;
			NPC.height = 40;
			NPC.damage = 14;
			NPC.defense = 6;
			NPC.lifeMax = 50;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath2;
			NPC.value = 60f;
			NPC.knockBackResist = 0.5f;
			NPC.aiStyle = 3;

			AIType = NPCID.Zombie;
			AnimationType = NPCID.Zombie;
			Banner = Item.NPCtoBanner(NPCID.Zombie);
			BannerItem = Item.BannerToItem(Banner);
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			var zombieDropRules = Main.ItemDropsDB.GetRulesForNPCID(NPCID.Zombie, false); // false is important here
			foreach (var zombieDropRule in zombieDropRules)
			{
				npcLoot.Add(zombieDropRule);
			}

			//npcLoot.Add(ItemDropRule.Common(ItemID.Confetti, 100)); // 1% chance to drop Confetti
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			//uncomment on release
			//if (Main.hardMode && !Main.dayTime)
				return SpawnCondition.Underground.Chance * 4f; // Spawn with 1/5th the chance of a regular zombie.

			return 0;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Underground,

				new FlavorTextBestiaryInfoElement("Headcrab zombies, minds paralyzed and controlled by a Headcrab. Underneath, a writhing soul desperate for help.")
			});
		}

		public override void HitEffect(int hitDirection, double damage)
		{
		}

		public override void OnHitPlayer(Player target, int damage, bool crit)
		{
			int timeToAdd = 5 * 60;
		}

		public override bool PreKill()
		{
			if (Main.rand.NextBool(2))
            {
				var entitySource = NPC.GetSource_FromAI();
				NPC.NewNPC(entitySource, (int)NPC.position.X, (int)NPC.position.Y+1, ModContent.NPCType<Headcrab>());
            }

			return true;
		}
	}
	
	public class Houndeye : ModNPC
    {
		private enum ActionState
		{
			Idle,
			Prowl,
			Pursue,
			Attack,
			Fall
		}

		private enum Frame
		{
			Idle,
			Moving1,
			Moving2,
			Moving3,
			Moving4,
			Moving5,
			Moving6,
			Attacking
		}

		public ref float AI_State => ref NPC.ai[0];
		public ref float AI_Timer => ref NPC.ai[1];
		public ref float AI_WalkingTimer => ref NPC.ai[2];
		public ref float AI_WalkingFrame => ref NPC.ai[3];
		public float AI_LastX;
		public float AI_LastY;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 8;

			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new(0)
			{
				Velocity = 1f
			};
		}

		public override void SetDefaults()
		{
			NPC.width = 48;
			NPC.height = 48;
			NPC.aiStyle = -1;
			NPC.damage = 15;
			NPC.defense = 5;
			NPC.lifeMax = 30;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = 15f;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Underground,

				new FlavorTextBestiaryInfoElement("The resonance cascade has resulted in bringing one of Xen's most common creatures, the parasitic Headcrab."),

				new FlavorTextBestiaryInfoElement("Allowing this creature to directly attack the rear of your cranium will not result in a pleasant outcome.")

			});
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			if (NPCID.Sets.NPCBestiaryDrawOffset.TryGetValue(Type, out NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers))
			{
				NPCID.Sets.NPCBestiaryDrawOffset.Remove(Type);
				NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
			}

			return true;
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			//uncomment on release
			//if (Main.hardMode)
			return SpawnCondition.OverworldDay.Chance * 5f;

			return 0;
		}
		public override bool? CanFallThroughPlatforms()
		{
			if (AI_State == (float)ActionState.Fall && NPC.HasValidTarget && Main.player[NPC.target].Top.Y > NPC.Bottom.Y)
			{
				return true;
			}

			return false;
		}

		public override void AI()
		{
			NPC.spriteDirection = NPC.direction;
			switch (AI_State)
			{
				case (float)ActionState.Idle:
					Idle();
					break;
				case (float)ActionState.Prowl:
					Prowl();
					break;
				case (float)ActionState.Pursue:
					Pursue();
					break;
				case (float)ActionState.Attack:
					//Attack();
					break;
				case (float)ActionState.Fall:
					Fall();
					break;
				default:
					Idle();
					break;
			}
		}

		private void Idle()
        {
			//sit around, literally do nothing
			NPC.TargetClosest(false);
			NPC.velocity.X = 0;
			NPC.frame.Y = (int)Frame.Idle * 48;
			AI_Timer++;
			if (AI_Timer > 120)
            {
				NPC.direction *= -1;
				AI_State = (float)ActionState.Prowl;
				AI_Timer = 0;
				AI_WalkingFrame = 0;
			}

			if (NPC.HasValidTarget && Main.player[NPC.target].Distance(NPC.Center) < 500f && AI_Timer > 30)
			{
				AI_State = (float)ActionState.Fall;
				NPC.velocity.Y = -4;
				AI_Timer = 0;
			}
		}

		private void Prowl()
        {
			NPC.TargetClosest(false);
			NPC.frame.Y = ((int)AI_WalkingFrame + 1) * 48;
			NPC.velocity.X = 0.8f * NPC.direction;
			AI_Timer++;

			if (AI_Timer > 240)
			{
				AI_State = (float)ActionState.Idle;
				AI_Timer = 0;
			}
			else
            {
				AI_WalkingTimer++;
				if (AI_WalkingTimer > 9)
                {
					AI_WalkingFrame++;
					if (AI_WalkingFrame > 5)
                    {
						AI_WalkingFrame = 0;
                    }

					AI_WalkingTimer = 0;
                }
            }

			if (NPC.HasValidTarget && Main.player[NPC.target].Distance(NPC.Center) < 500f && AI_Timer > 30)
			{
				AI_State = (float)ActionState.Pursue;
				AI_Timer = 0;
			}
		}

		private void Pursue()
        {
			NPC.TargetClosest(true);
			NPC.frame.Y = ((int)AI_WalkingFrame + 1) * 48;
			NPC.velocity.X = 2f * NPC.direction;
			AI_Timer++;

			if (AI_Timer > 960 || !NPC.HasValidTarget || Main.player[NPC.target].Distance(NPC.Center) > 750f)
			{
				AI_State = (float)ActionState.Prowl;
				AI_Timer = 0;
			}
			else
			{
				AI_WalkingTimer++;
				if (AI_WalkingTimer > 9)
				{
					AI_WalkingFrame++;
					if (AI_WalkingFrame > 5)
					{
						AI_WalkingFrame = 0;
					}

					AI_WalkingTimer = 0;
				}

				/*
				int targetTileX = (int)((NPC.position.X + (48 * NPC.direction)) / 16);
				int targetTileY = (int)((NPC.position.Y + 32) / 16);

				Tile targetTile = Main.tile[targetTileX, targetTileY];
				Tile targetTile2 = Main.tile[targetTileX, targetTileY];
				Tile targetTile3 = Main.tile[targetTileX, targetTileY - 1];

				bool blocked = (targetTile.HasTile || targetTile.IsHalfBlock) || (targetTile2.HasTile || targetTile2.IsHalfBlock) || (targetTile3.HasTile || targetTile3.IsHalfBlock);

				Dust.QuickBox(new Vector2(targetTileX, targetTileY) * 16, new Vector2(targetTileX + 1, targetTileY + 1) * 16, 2, Color.YellowGreen, null);
				*/

				if (AI_LastX == NPC.position.X && NPC.velocity.Y == 0)
				{
					AI_State = (float)ActionState.Fall;
					NPC.velocity.Y = -6;
					AI_Timer = 0;
				}
			}

			AI_LastX = NPC.position.X;
			AI_LastY = NPC.position.Y;
		}

		private void Fall()
        {
			NPC.velocity.X += 0.01f * NPC.direction;
			AI_Timer++;

			if (AI_Timer > 30)
            {
				AI_State = (float)ActionState.Prowl;
				AI_Timer = 0;
				AI_WalkingFrame = 0;
			}
        }
	}
}