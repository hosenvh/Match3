using Match3.Game.Gameplay;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Factories;
using Match3.Game.Gameplay.HitManagement;
using Match3.Game.Gameplay.Matching;
using Match3.Game.Gameplay.Physics;
using Match3.Game.Gameplay.Tiles;
using Match3.Game.Gameplay.Tiles.Explosives;
using Match3.Game.Gameplay.Swapping;
using Match3.Game.Gameplay.RainbowMechanic;
using Match3.Game.Gameplay.SubSystems.PowerUpManagement;
using Match3.Game.Gameplay.SubSystems.HoneyMechanic;
using Match3.Game.Gameplay.SubSystems.ButterflyMechanic;
using Match3.Game.Gameplay.SubSystems.VacuumCleanerMechanic;
using Match3.Game.Gameplay.SubSystems.IvyMechanic;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Match3.Main
{
    public class MainTileFactory : TileFactory
    {
        public TileStack CreateTileStack()
        {
            var stack = new TileStack();
            stack.AddComponent(new LockState());
            stack.AddComponent(new TileStackPhysicalState());
            return stack;
        }

        public Tile CreateChainTile(int level)
        {
            var tile = new Chain(level);
            tile.AddComponent(new TilePhysicalProperties(isAffectedByGravity: false));
            tile.AddComponent(new TileMatchingProperties(allowsMatchFallThrough: true));
            tile.AddComponent(new TileUserInteractionProperties(isSwappable: false));
            tile.AddComponent(new TileRainbowProperties(matchingRule: TileRainbowProperties.MatchingRule.None));
            tile.AddComponent(new TilePowerUpProperties(isHammerTarget: true));
            tile.AddComponent(new DefaultTileHitProperties(
                acceptsDirectHit: true,
                acceptsSideHit: false, 
                suppressesHitToSideHit: true,
                propagatesHitToCell: true));
            tile.AddComponent(new TileHoneyMechanicProperties(canBeTakenOverByHoney: false));
            tile.AddComponent(new TileButterflyMechanicProperties(canButterflyBeGeneratedOn: false));
            tile.AddComponent(new VacuumCleanerMechanicProperties(vacuumCleanerMustHitOnlyOnce: false));
            tile.AddComponent(new IvyMechanicTileProperties(canBeTakenOverByIvy: false));

            return tile;
        }


        public Tile CreateRockTile(int level)
        {
            var tile = new Rock(level);
            tile.AddComponent(new TilePhysicalProperties(isAffectedByGravity: false));
            tile.AddComponent(new TileMatchingProperties(allowsMatchFallThrough: false));
            tile.AddComponent(new TileUserInteractionProperties(isSwappable: false));
            tile.AddComponent(new TileRainbowProperties(matchingRule: TileRainbowProperties.MatchingRule.None));
            tile.AddComponent(new TilePowerUpProperties(isHammerTarget: true));
            tile.AddComponent(new RockHitProperties(tile));
            tile.AddComponent(new TileHoneyMechanicProperties(canBeTakenOverByHoney: false));
            tile.AddComponent(new TileButterflyMechanicProperties(canButterflyBeGeneratedOn: false));
            tile.AddComponent(new VacuumCleanerMechanicProperties(vacuumCleanerMustHitOnlyOnce: false));
            tile.AddComponent(new IvyMechanicTileProperties(canBeTakenOverByIvy: false));

            return tile;
        }


        public Tile CreateGardenTile()
        {
            var tile = new Garden();
            tile.AddComponent(new TilePhysicalProperties(isAffectedByGravity: false));
            tile.AddComponent(new TileMatchingProperties(allowsMatchFallThrough: false));
            tile.AddComponent(new TileUserInteractionProperties(isSwappable: false));
            tile.AddComponent(new TileRainbowProperties(matchingRule: TileRainbowProperties.MatchingRule.None));
            tile.AddComponent(new TilePowerUpProperties(isHammerTarget: true));
            tile.AddComponent(new DefaultTileHitProperties(
                acceptsDirectHit: true,
                acceptsSideHit: true,
                suppressesHitToSideHit: true,
                propagatesHitToCell: false));
            tile.AddComponent(new TileHoneyMechanicProperties(canBeTakenOverByHoney: false));
            tile.AddComponent(new TileButterflyMechanicProperties(canButterflyBeGeneratedOn: false));
            tile.AddComponent(new VacuumCleanerMechanicProperties(vacuumCleanerMustHitOnlyOnce: true));
            tile.AddComponent(new IvyMechanicTileProperties(canBeTakenOverByIvy: false));

            return tile;
        }

        public Tile CreateChickenNestTile()
        {
            var tile = new ChickenNest();
            tile.AddComponent(new TilePhysicalProperties(isAffectedByGravity: true));
            tile.AddComponent(new TileMatchingProperties(allowsMatchFallThrough: false));
            tile.AddComponent(new TileUserInteractionProperties(isSwappable: true));
            tile.AddComponent(new TileRainbowProperties(matchingRule: TileRainbowProperties.MatchingRule.None));
            tile.AddComponent(new TilePowerUpProperties(isHammerTarget: true));
            tile.AddComponent(new DefaultTileHitProperties(
               acceptsDirectHit: true,
               acceptsSideHit: true,
               suppressesHitToSideHit: true,
               propagatesHitToCell: false));
            tile.AddComponent(new TileHoneyMechanicProperties(canBeTakenOverByHoney: false));
            tile.AddComponent(new TileButterflyMechanicProperties(canButterflyBeGeneratedOn: false));
            tile.AddComponent(new VacuumCleanerMechanicProperties(vacuumCleanerMustHitOnlyOnce: true));
            tile.AddComponent(new IvyMechanicTileProperties(canBeTakenOverByIvy: false));

            return tile;
        }

        public Tile CreateBalloonTiedTile(Vector2Int relationalDirectionFromItsMainBalloonTile)
        {
            var tile = new BalloonTiedTile(relationalDirectionFromItsMainBalloonTile);
            tile.AddComponent(new TilePhysicalProperties(isAffectedByGravity: false));
            tile.AddComponent(new TileMatchingProperties(allowsMatchFallThrough: true));
            tile.AddComponent(new TileUserInteractionProperties(isSwappable: false));
            tile.AddComponent(new TileRainbowProperties(matchingRule: TileRainbowProperties.MatchingRule.None));
            tile.AddComponent(new TilePowerUpProperties(isHammerTarget: true));
            tile.AddComponent(new DefaultTileHitProperties(
                                  acceptsDirectHit: true,
                                  acceptsSideHit: false,
                                  suppressesHitToSideHit: true,
                                  propagatesHitToCell: true));
            tile.AddComponent(new TileHoneyMechanicProperties(canBeTakenOverByHoney: false));
            tile.AddComponent(new TileButterflyMechanicProperties(canButterflyBeGeneratedOn: false));
            tile.AddComponent(new VacuumCleanerMechanicProperties(vacuumCleanerMustHitOnlyOnce: false));
            tile.AddComponent(new IvyMechanicTileProperties(canBeTakenOverByIvy: false));

            return tile;
        }

        public Tile CreateBalloonMainTile()
        {
            var tile = new BalloonMainTile();
            tile.AddComponent(new TilePhysicalProperties(isAffectedByGravity: false));
            tile.AddComponent(new TileMatchingProperties(allowsMatchFallThrough: false));
            tile.AddComponent(new TileUserInteractionProperties(isSwappable: false));
            tile.AddComponent(new TileRainbowProperties(matchingRule: TileRainbowProperties.MatchingRule.None));
            tile.AddComponent(new TilePowerUpProperties(isHammerTarget: false));
            tile.AddComponent(new DefaultTileHitProperties(
                                  acceptsDirectHit: false,
                                  acceptsSideHit: false,
                                  suppressesHitToSideHit: true,
                                  propagatesHitToCell: false));
            tile.AddComponent(new TileHoneyMechanicProperties(canBeTakenOverByHoney: false));
            tile.AddComponent(new TileButterflyMechanicProperties(canButterflyBeGeneratedOn: false));
            tile.AddComponent(new VacuumCleanerMechanicProperties(vacuumCleanerMustHitOnlyOnce: true));
            tile.AddComponent(new IvyMechanicTileProperties(canBeTakenOverByIvy: false));

            return tile;
        }

        public Tile CreateBoxTile(int level)
        {
            var tile = new Box(level);
            AddBoxComponents(tile);
            return tile;
        }

        public Tile CreateColoredBoxTile(int level, TileColor color)
        {
            var tile = new ColoredBox(level, color);
            AddBoxComponents(tile);
            return tile;
        }

        void AddBoxComponents(Tile tile)
        {
            tile.AddComponent(new TilePhysicalProperties(isAffectedByGravity: false));
            tile.AddComponent(new TileMatchingProperties(allowsMatchFallThrough: false));
            tile.AddComponent(new TileUserInteractionProperties(isSwappable: false));
            tile.AddComponent(new TileRainbowProperties(matchingRule: TileRainbowProperties.MatchingRule.None));
            tile.AddComponent(new TilePowerUpProperties(isHammerTarget: true));
            tile.AddComponent(new DefaultTileHitProperties(
                acceptsDirectHit: true,
                acceptsSideHit: true,
                suppressesHitToSideHit: true,
                propagatesHitToCell: false));
            tile.AddComponent(new TileHoneyMechanicProperties(canBeTakenOverByHoney: false));
            tile.AddComponent(new TileButterflyMechanicProperties(canButterflyBeGeneratedOn: false));
            tile.AddComponent(new VacuumCleanerMechanicProperties(vacuumCleanerMustHitOnlyOnce: false));
            tile.AddComponent(new IvyMechanicTileProperties(canBeTakenOverByIvy: false));
        }

        public Tile CreateHoneyTile()
        {
            var tile = new Honey();
            tile.AddComponent(new TilePhysicalProperties(isAffectedByGravity: false));
            tile.AddComponent(new TileMatchingProperties(allowsMatchFallThrough: false));
            tile.AddComponent(new TileUserInteractionProperties(isSwappable: false));
            tile.AddComponent(new TileRainbowProperties(matchingRule: TileRainbowProperties.MatchingRule.None));
            tile.AddComponent(new TilePowerUpProperties(isHammerTarget: true));
            tile.AddComponent(new DefaultTileHitProperties(
                acceptsDirectHit: true,
                acceptsSideHit: true,
                suppressesHitToSideHit: true,
                propagatesHitToCell: false));
            tile.AddComponent(new TileHoneyMechanicProperties(canBeTakenOverByHoney: false));
            tile.AddComponent(new TileButterflyMechanicProperties(canButterflyBeGeneratedOn: false));
            tile.AddComponent(new VacuumCleanerMechanicProperties(vacuumCleanerMustHitOnlyOnce: false));
            tile.AddComponent(new IvyMechanicTileProperties(canBeTakenOverByIvy: false));

            return tile;
        }


        public Tile CreateHardenedHoneyTile()
        {
            var tile = new HardenedHoney();

            tile.AddComponents(
                new TilePhysicalProperties(isAffectedByGravity: false),
                new TileMatchingProperties(allowsMatchFallThrough: false),
                new TileUserInteractionProperties(isSwappable: false),
                new TileRainbowProperties(matchingRule: TileRainbowProperties.MatchingRule.None),
                new TilePowerUpProperties(isHammerTarget: true),
                new DefaultTileHitProperties(
                    acceptsDirectHit: true,
                    acceptsSideHit: true,
                    suppressesHitToSideHit: true,
                    propagatesHitToCell: false),
                new TileHoneyMechanicProperties(canBeTakenOverByHoney: false),
                new TileButterflyMechanicProperties(canButterflyBeGeneratedOn: false),
                new VacuumCleanerMechanicProperties(vacuumCleanerMustHitOnlyOnce: false),
                new IvyMechanicTileProperties(canBeTakenOverByIvy: false));

            return tile;
        }

        public Tile CreateCleanColoredBeadTile(TileColor color)
        {
            return CreateColoredBeadTile(color, ColoredBead.DirtinessState.Clean);
        }

        public Tile CreateColoredBeadTile(TileColor color, ColoredBead.DirtinessState dirtinessState)
        {
            var tile = new ColoredBead(dirtinessState);
            tile.AddComponent(new TilePhysicalProperties(isAffectedByGravity: true));
            tile.AddComponent(new TileMatchingProperties(allowsMatchFallThrough: false));
            tile.AddComponent(new TileUserInteractionProperties(isSwappable: true));
            tile.AddComponent(new TileRainbowProperties(matchingRule: TileRainbowProperties.MatchingRule.MatchWithRainbowFindOtherMatchesBySameColor));
            tile.AddComponent(new TilePowerUpProperties(isHammerTarget: true));
            tile.AddComponent(new DefaultTileHitProperties(
                acceptsDirectHit: true,
                acceptsSideHit: false,
                suppressesHitToSideHit: false,
                propagatesHitToCell: true));
            tile.AddComponent(new TileHoneyMechanicProperties(canBeTakenOverByHoney: true));
            tile.AddComponent(new TileColorComponent(color: color));
            tile.AddComponent(new TileButterflyMechanicProperties(canButterflyBeGeneratedOn: true));
            tile.AddComponent(new VacuumCleanerMechanicProperties(vacuumCleanerMustHitOnlyOnce: false));
            tile.AddComponent(new IvyMechanicTileProperties(canBeTakenOverByIvy: true));

            return tile;
        }

        public Tile CreateLemonadeTile()
        {
            var tile = new Lemonade();
            tile.AddComponent(new TilePhysicalProperties(isAffectedByGravity: true));
            tile.AddComponent(new TileMatchingProperties(allowsMatchFallThrough: false));
            tile.AddComponent(new TileUserInteractionProperties(isSwappable: true));
            tile.AddComponent(new TileRainbowProperties(matchingRule: TileRainbowProperties.MatchingRule.None));
            tile.AddComponent(new TilePowerUpProperties(isHammerTarget: false));
            tile.AddComponent(new DefaultTileHitProperties(
                acceptsDirectHit: true, 
                acceptsSideHit: false, 
                suppressesHitToSideHit:true,
                propagatesHitToCell: true));
            tile.AddComponent(new TileHoneyMechanicProperties(canBeTakenOverByHoney: false));
            tile.AddComponent(new TileButterflyMechanicProperties(canButterflyBeGeneratedOn: false));
            tile.AddComponent(new VacuumCleanerMechanicProperties(vacuumCleanerMustHitOnlyOnce: true));
            tile.AddComponent(new IvyMechanicTileProperties(canBeTakenOverByIvy: true));

            return tile;
        }

        public Tile CreateChickTile()
        {
            var tile = new Chick();

            tile.AddComponent(new TilePhysicalProperties(isAffectedByGravity: true));
            tile.AddComponent(new TileMatchingProperties(allowsMatchFallThrough: false));
            tile.AddComponent(new TileUserInteractionProperties(isSwappable: true));
            tile.AddComponent(new TileRainbowProperties(matchingRule: TileRainbowProperties.MatchingRule.MatchWithRainbowFindOtherMatchesBySameType));
            tile.AddComponent(new TilePowerUpProperties(isHammerTarget: true));
            tile.AddComponent(new DefaultTileHitProperties(
               acceptsDirectHit: true,
               acceptsSideHit: true,
               suppressesHitToSideHit: true,
               propagatesHitToCell: false));
            tile.AddComponent(new TileHoneyMechanicProperties(canBeTakenOverByHoney: false));
            tile.AddComponent(new TileButterflyMechanicProperties(canButterflyBeGeneratedOn: true));
            tile.AddComponent(new VacuumCleanerMechanicProperties(vacuumCleanerMustHitOnlyOnce: false));
            tile.AddComponent(new IvyMechanicTileProperties(canBeTakenOverByIvy: true));

            return tile;
        }

        public Tile CreateNutTile(int initialLevel)
        {
            var tile = new Nut(initialLevel);
            tile.AddComponent(new TilePhysicalProperties(isAffectedByGravity: true));
            tile.AddComponent(new TileMatchingProperties(allowsMatchFallThrough: false));
            tile.AddComponent(new TileUserInteractionProperties(isSwappable: true));
            tile.AddComponent(new TileRainbowProperties(matchingRule: TileRainbowProperties.MatchingRule.MatchWithRainbowFindOtherMatchesBySameType));
            tile.AddComponent(new TilePowerUpProperties(isHammerTarget: true));
            tile.AddComponent(new DefaultTileHitProperties(
                acceptsDirectHit: true,
                acceptsSideHit: true,
                suppressesHitToSideHit: true,
                propagatesHitToCell: false));
            tile.AddComponent(new TileHoneyMechanicProperties(canBeTakenOverByHoney: false));
            tile.AddComponent(new TileButterflyMechanicProperties(canButterflyBeGeneratedOn: true));
            tile.AddComponent(new VacuumCleanerMechanicProperties(vacuumCleanerMustHitOnlyOnce: false));
            tile.AddComponent(new IvyMechanicTileProperties(canBeTakenOverByIvy: true));

            return tile;
        }

        public Tile CreateExtraMoveTile(int moveAmount)
        {
            var tile = new ExtraMove(moveAmount);
            tile.AddComponent(new TilePhysicalProperties(isAffectedByGravity: true));
            tile.AddComponent(new TileMatchingProperties(allowsMatchFallThrough: false));
            tile.AddComponent(new TileUserInteractionProperties(isSwappable: true));
            tile.AddComponent(new TileRainbowProperties(matchingRule: TileRainbowProperties.MatchingRule.None));
            tile.AddComponent(new TilePowerUpProperties(isHammerTarget: true));
            tile.AddComponent(new DefaultTileHitProperties(
                acceptsDirectHit: true,
                acceptsSideHit: true,
                suppressesHitToSideHit: true,
                propagatesHitToCell: true));
            tile.AddComponent(new TileHoneyMechanicProperties(canBeTakenOverByHoney: false));
            tile.AddComponent(new TileButterflyMechanicProperties(canButterflyBeGeneratedOn: false));
            tile.AddComponent(new VacuumCleanerMechanicProperties(vacuumCleanerMustHitOnlyOnce: false));
            tile.AddComponent(new IvyMechanicTileProperties(canBeTakenOverByIvy: true));

            return tile;
        }


        public Tile CreateIceTile(int level)
        {
            var tile = new Ice(level);
            tile.AddComponent(new TilePhysicalProperties(isAffectedByGravity: true));
            tile.AddComponent(new TileMatchingProperties(allowsMatchFallThrough: true));
            tile.AddComponent(new TileUserInteractionProperties(isSwappable: false));
            tile.AddComponent(new TileRainbowProperties(matchingRule: TileRainbowProperties.MatchingRule.None));
            tile.AddComponent(new TilePowerUpProperties(isHammerTarget: true));
            tile.AddComponent(new DefaultTileHitProperties(
                acceptsDirectHit: true,
                acceptsSideHit: false,
                suppressesHitToSideHit: true,
                propagatesHitToCell: false));
            tile.AddComponent(new TileHoneyMechanicProperties(canBeTakenOverByHoney: false));
            tile.AddComponent(new TileButterflyMechanicProperties(canButterflyBeGeneratedOn: false));
            tile.AddComponent(new VacuumCleanerMechanicProperties(vacuumCleanerMustHitOnlyOnce: false));
            tile.AddComponent(new IvyMechanicTileProperties(canBeTakenOverByIvy: false));

            return tile;
        }

        public Tile CreateSandTile(int level)
        {
            var tile = new EmptySand(level);
            tile.AddComponent(new TilePhysicalProperties(isAffectedByGravity: true));
            tile.AddComponent(new TileMatchingProperties(allowsMatchFallThrough: false));
            tile.AddComponent(new TileUserInteractionProperties(isSwappable: false));
            tile.AddComponent(new TileRainbowProperties(matchingRule: TileRainbowProperties.MatchingRule.None));
            tile.AddComponent(new TilePowerUpProperties(isHammerTarget: true));
            tile.AddComponent(new DefaultTileHitProperties(
                acceptsDirectHit: true,
                acceptsSideHit: true,
                suppressesHitToSideHit: true,
                propagatesHitToCell: false));
            tile.AddComponent(new TileHoneyMechanicProperties(canBeTakenOverByHoney: false));
            tile.AddComponent(new TileButterflyMechanicProperties(canButterflyBeGeneratedOn: false));
            tile.AddComponent(new VacuumCleanerMechanicProperties(vacuumCleanerMustHitOnlyOnce: false));
            tile.AddComponent(new IvyMechanicTileProperties(canBeTakenOverByIvy: false));

            return tile;
        }

        public Tile CreateSandWithGemTile(int level)
        {
            var tile = new SandWithGem(level);
            tile.AddComponent(new TilePhysicalProperties(isAffectedByGravity: true));
            tile.AddComponent(new TileMatchingProperties(allowsMatchFallThrough: false));
            tile.AddComponent(new TileUserInteractionProperties(isSwappable: false));
            tile.AddComponent(new TileRainbowProperties(matchingRule: TileRainbowProperties.MatchingRule.None));
            tile.AddComponent(new TilePowerUpProperties(isHammerTarget: true));
            tile.AddComponent(new DefaultTileHitProperties(
                acceptsDirectHit: true,
                acceptsSideHit: true,
                suppressesHitToSideHit: true,
                propagatesHitToCell: false));
            tile.AddComponent(new TileHoneyMechanicProperties(canBeTakenOverByHoney: false));
            tile.AddComponent(new TileButterflyMechanicProperties(canButterflyBeGeneratedOn: false));
            tile.AddComponent(new VacuumCleanerMechanicProperties(vacuumCleanerMustHitOnlyOnce: false));
            tile.AddComponent(new IvyMechanicTileProperties(canBeTakenOverByIvy: false));

            return tile;
        }


        public Tile CreateFireflyJar(int level)
        {
            var tile = new FireflyJar(level);
            tile.AddComponent(new TilePhysicalProperties(isAffectedByGravity: false));
            tile.AddComponent(new TileMatchingProperties(allowsMatchFallThrough: false));
            tile.AddComponent(new TileUserInteractionProperties(isSwappable: false));
            tile.AddComponent(new TileRainbowProperties(matchingRule: TileRainbowProperties.MatchingRule.None));
            tile.AddComponent(new TilePowerUpProperties(isHammerTarget: true));
            tile.AddComponent(new DefaultTileHitProperties(
                acceptsDirectHit: true,
                acceptsSideHit: true,
                suppressesHitToSideHit: true,
                propagatesHitToCell: false));
            tile.AddComponent(new TileHoneyMechanicProperties(canBeTakenOverByHoney: false));
            tile.AddComponent(new TileButterflyMechanicProperties(canButterflyBeGeneratedOn: false));
            tile.AddComponent(new VacuumCleanerMechanicProperties(vacuumCleanerMustHitOnlyOnce: true));
            tile.AddComponent(new IvyMechanicTileProperties(canBeTakenOverByIvy: false));

            return tile;
        }

        public Tile CreateButterflyTile(TileColor color)
        {
            var tile = new Butterfly();
            tile.AddComponent(new TilePhysicalProperties(isAffectedByGravity: true));
            tile.AddComponent(new TileMatchingProperties(allowsMatchFallThrough: false));
            tile.AddComponent(new TileUserInteractionProperties(isSwappable: true));
            tile.AddComponent(new TileRainbowProperties(matchingRule: TileRainbowProperties.MatchingRule.MatchWithRainbowFindOtherMatchesBySameColor));
            tile.AddComponent(new TilePowerUpProperties(isHammerTarget: true));
            tile.AddComponent(new DefaultTileHitProperties(
                acceptsDirectHit: true,
                acceptsSideHit: false,
                suppressesHitToSideHit: false,
                propagatesHitToCell: true));
            tile.AddComponent(new TileHoneyMechanicProperties(canBeTakenOverByHoney: false));
            tile.AddComponent(new TileButterflyMechanicProperties(canButterflyBeGeneratedOn: false));
            tile.AddComponent(new TileColorComponent(color));
            tile.AddComponent(new VacuumCleanerMechanicProperties(vacuumCleanerMustHitOnlyOnce: false));
            tile.AddComponent(new IvyMechanicTileProperties(canBeTakenOverByIvy: false));

            return tile;
        }

        public Tile CreateRocketBoxTile(TileColor color)
        {
            var tile = new RocketBox();
            tile.AddComponent(new TilePhysicalProperties(isAffectedByGravity: true));
            tile.AddComponent(new TileMatchingProperties(allowsMatchFallThrough: false));
            tile.AddComponent(new TileUserInteractionProperties(isSwappable: true));
            tile.AddComponent(new TileRainbowProperties(TileRainbowProperties.MatchingRule.MatchWithRainbowFindOtherMatchesBySameColor));
            tile.AddComponent(new TilePowerUpProperties(isHammerTarget: true));
            tile.AddComponent(new DefaultTileHitProperties(
                acceptsDirectHit: true,
                acceptsSideHit: false,
                suppressesHitToSideHit: false,
                propagatesHitToCell: true));
            tile.AddComponent(new TileHoneyMechanicProperties(canBeTakenOverByHoney: false));
            tile.AddComponent(new TileButterflyMechanicProperties(canButterflyBeGeneratedOn: false));
            tile.AddComponent(new TileColorComponent(color));
            tile.AddComponent(new VacuumCleanerMechanicProperties(vacuumCleanerMustHitOnlyOnce: false));
            tile.AddComponent(new IvyMechanicTileProperties(canBeTakenOverByIvy: true));

            return tile;
        }

        public Tile CreateRainbowTile()
        {
            var tile = new Rainbow();
            tile.AddComponent(new TilePhysicalProperties(isAffectedByGravity: true));
            tile.AddComponent(new TileMatchingProperties(allowsMatchFallThrough: false));
            tile.AddComponent(new TileUserInteractionProperties(isSwappable: true));
            tile.AddComponent(new TileRainbowProperties(matchingRule: TileRainbowProperties.MatchingRule.MatchWithRainbowNoOtherMatches));
            tile.AddComponent(new TilePowerUpProperties(isHammerTarget: true));
            tile.AddComponent(new DefaultTileHitProperties(
                acceptsDirectHit: false,
                acceptsSideHit: false,
                suppressesHitToSideHit: true,
                propagatesHitToCell: true));
            tile.AddComponent(new TileHoneyMechanicProperties(canBeTakenOverByHoney: true));
            tile.AddComponent(new TileButterflyMechanicProperties(canButterflyBeGeneratedOn: false));
            tile.AddComponent(new VacuumCleanerMechanicProperties(vacuumCleanerMustHitOnlyOnce: false));
            tile.AddComponent(new IvyMechanicTileProperties(canBeTakenOverByIvy: true));

            return tile;
        }

        public Tile CreateRocketTile()
        {
            var tile = new Rocket();
            AddDefaultExplosiveProperties(tile);
            return tile;
        }

        public Tile CreateBombTile()
        {
            var tile = new Bomb();
            AddDefaultExplosiveProperties(tile);
            return tile;
        }

        public Tile CreateDynamiteTile()
        {
            var tile = new Dynamite();
            AddDefaultExplosiveProperties(tile);
            return tile;
        }

        public Tile CreateTNTBarrelTile()
        {
            var tile = new TNTBarrel();
            AddDefaultExplosiveProperties(tile);
            return tile;
        }

        private void AddDefaultExplosiveProperties(ExplosiveTile tile)
        {
            tile.AddComponent(new TilePhysicalProperties(isAffectedByGravity: true));
            tile.AddComponent(new TileMatchingProperties(allowsMatchFallThrough: false));
            tile.AddComponent(new TileUserInteractionProperties(isSwappable: true));
            tile.AddComponent(new TileRainbowProperties(matchingRule: TileRainbowProperties.MatchingRule.None));
            tile.AddComponent(new TilePowerUpProperties(isHammerTarget: true));
            tile.AddComponent(new DefaultTileHitProperties(
                acceptsDirectHit: true,
                acceptsSideHit: false,
                suppressesHitToSideHit: true,
                propagatesHitToCell: true));
            tile.AddComponent(new TileHoneyMechanicProperties(canBeTakenOverByHoney: true));
            tile.AddComponent(new TileButterflyMechanicProperties(canButterflyBeGeneratedOn: false));
            tile.AddComponent(new VacuumCleanerMechanicProperties(vacuumCleanerMustHitOnlyOnce: false));
            tile.AddComponent(new IvyMechanicTileProperties(canBeTakenOverByIvy: true));

        }

        public Tile CreateVacuumCleaner(TileColor targetColor, int targetAmount, Direction direction, int priority)
        {
            var tile = new VacuumCleaner(targetColor, targetAmount, direction, priority);
            tile.AddComponent(new TilePhysicalProperties(isAffectedByGravity: false));
            tile.AddComponent(new TileMatchingProperties(allowsMatchFallThrough: false));
            tile.AddComponent(new TileUserInteractionProperties(isSwappable: false));
            tile.AddComponent(new TileRainbowProperties(matchingRule: TileRainbowProperties.MatchingRule.None));
            tile.AddComponent(new TilePowerUpProperties(isHammerTarget: false));
            tile.AddComponent(new DefaultTileHitProperties(
                acceptsDirectHit: false,
                acceptsSideHit: false,
                suppressesHitToSideHit: true,
                propagatesHitToCell: false));
            tile.AddComponent(new TileHoneyMechanicProperties(canBeTakenOverByHoney: false));
            tile.AddComponent(new TileButterflyMechanicProperties(canButterflyBeGeneratedOn: false));
            tile.AddComponent(new VacuumCleanerMechanicProperties(vacuumCleanerMustHitOnlyOnce: true));
            tile.AddComponent(new IvyMechanicTileProperties(canBeTakenOverByIvy: false));

            return tile;
        }

        public Tile CreateHoneyJarTile()
        {
            var tile = new HoneyJar();
            tile.AddComponent(new TilePhysicalProperties(isAffectedByGravity: true));
            tile.AddComponent(new TileMatchingProperties(allowsMatchFallThrough: false));
            tile.AddComponent(new TileUserInteractionProperties(isSwappable: true));
            tile.AddComponent(new TileRainbowProperties(matchingRule: TileRainbowProperties.MatchingRule.None));
            tile.AddComponent(new TilePowerUpProperties(isHammerTarget: true));
            tile.AddComponent(new DefaultTileHitProperties(
                acceptsDirectHit: true,
                acceptsSideHit: true,
                suppressesHitToSideHit: true,
                propagatesHitToCell: false));
            tile.AddComponent(new TileHoneyMechanicProperties(canBeTakenOverByHoney: false));
            tile.AddComponent(new TileButterflyMechanicProperties(canButterflyBeGeneratedOn: false));
            tile.AddComponent(new VacuumCleanerMechanicProperties(vacuumCleanerMustHitOnlyOnce: false));
            tile.AddComponent(new IvyMechanicTileProperties(canBeTakenOverByIvy: true));

            return tile;
        }


        public Tile CreateHoneycomb()
        {
            var tile = new Honeycomb();
            tile.AddComponent(new TilePhysicalProperties(isAffectedByGravity: false));
            tile.AddComponent(new TileMatchingProperties(allowsMatchFallThrough: false));
            tile.AddComponent(new TileUserInteractionProperties(isSwappable: false));
            tile.AddComponent(new TileRainbowProperties(matchingRule: TileRainbowProperties.MatchingRule.None));
            tile.AddComponent(new TilePowerUpProperties(isHammerTarget: false));
            tile.AddComponent(new DefaultTileHitProperties(
                acceptsDirectHit: false,
                acceptsSideHit: false,
                suppressesHitToSideHit: true,
                propagatesHitToCell: false));
            tile.AddComponent(new TileHoneyMechanicProperties(canBeTakenOverByHoney: false));
            tile.AddComponent(new TileButterflyMechanicProperties(canButterflyBeGeneratedOn: false));
            tile.AddComponent(new VacuumCleanerMechanicProperties(vacuumCleanerMustHitOnlyOnce: false));
            tile.AddComponent(new IvyMechanicTileProperties(canBeTakenOverByIvy: false));

            return tile;
        }

        public Tile CreatePadlock(Padlock.Status status)
        {
            var tile = new Padlock(status);
            tile.AddComponent(new TilePhysicalProperties(isAffectedByGravity: false));
            tile.AddComponent(new TileMatchingProperties(allowsMatchFallThrough: false));
            tile.AddComponent(new TileUserInteractionProperties(isSwappable: false));
            tile.AddComponent(new TileRainbowProperties(matchingRule: TileRainbowProperties.MatchingRule.None));
            tile.AddComponent(new TilePowerUpProperties(isHammerTarget: true));
            tile.AddComponent(new DefaultTileHitProperties(
                acceptsDirectHit: true,
                acceptsSideHit: true,
                suppressesHitToSideHit: true,
                propagatesHitToCell: false));
            tile.AddComponent(new TileHoneyMechanicProperties(canBeTakenOverByHoney: false));
            tile.AddComponent(new TileButterflyMechanicProperties(canButterflyBeGeneratedOn: false));
            tile.AddComponent(new VacuumCleanerMechanicProperties(vacuumCleanerMustHitOnlyOnce: true));
            tile.AddComponent(new IvyMechanicTileProperties(canBeTakenOverByIvy: false));

            // TODO:: Add Vacuum cleaner mechanic component.

            return tile;
        }

        public Tile CreateIvyBushTile()
        {
            var tile = new IvyBush();
            tile.AddComponent(new TilePhysicalProperties(isAffectedByGravity: false));
            tile.AddComponent(new TileMatchingProperties(allowsMatchFallThrough: true));
            tile.AddComponent(new TileUserInteractionProperties(isSwappable: false));
            tile.AddComponent(new TileRainbowProperties(matchingRule: TileRainbowProperties.MatchingRule.None));
            tile.AddComponent(new TilePowerUpProperties(isHammerTarget: true));
            tile.AddComponent(new DefaultTileHitProperties(
                acceptsDirectHit: true,
                acceptsSideHit: false,
                suppressesHitToSideHit: true,
                propagatesHitToCell: false));
            tile.AddComponent(new TileHoneyMechanicProperties(canBeTakenOverByHoney: false));
            tile.AddComponent(new TileButterflyMechanicProperties(canButterflyBeGeneratedOn: false));
            tile.AddComponent(new VacuumCleanerMechanicProperties(vacuumCleanerMustHitOnlyOnce: true));
            tile.AddComponent(new IvyMechanicTileProperties(canBeTakenOverByIvy: false));

            return tile;
        }

        public Tile CreateGasCylinder(TileColor color, int startCountdown)
        {
            var tile = new GasCylinder(startCountdown);
            tile.AddComponent(new TilePhysicalProperties(isAffectedByGravity: true));
            tile.AddComponent(new TileMatchingProperties(allowsMatchFallThrough: false));
            tile.AddComponent(new TileUserInteractionProperties(isSwappable: true));
            tile.AddComponent(new TileRainbowProperties(matchingRule: TileRainbowProperties.MatchingRule.MatchWithRainbowFindOtherMatchesBySameColor));
            tile.AddComponent(new TilePowerUpProperties(isHammerTarget: true));
            tile.AddComponent(new DefaultTileHitProperties(
                acceptsDirectHit: true,
                acceptsSideHit: false,
                suppressesHitToSideHit: false,
                propagatesHitToCell: true));
            tile.AddComponent(new TileHoneyMechanicProperties(canBeTakenOverByHoney: false));
            tile.AddComponent(new TileButterflyMechanicProperties(canButterflyBeGeneratedOn: false));
            tile.AddComponent(new VacuumCleanerMechanicProperties(vacuumCleanerMustHitOnlyOnce: true));
            tile.AddComponent(new IvyMechanicTileProperties(canBeTakenOverByIvy: false));

            tile.AddComponent(new TileColorComponent(color));

            return tile;
        }

        public Tile CreateBeachBallTile(HashSet<TileColor> colors)
        {
            var tile = new BeachBallMainTile(colors);
            tile.AddComponent(new TilePhysicalProperties(isAffectedByGravity: false));
            tile.AddComponent(new TileMatchingProperties(allowsMatchFallThrough: false));
            tile.AddComponent(new TileUserInteractionProperties(isSwappable: false));
            tile.AddComponent(new TileRainbowProperties(matchingRule: TileRainbowProperties.MatchingRule.None));
            tile.AddComponent(new TilePowerUpProperties(isHammerTarget: false));
            tile.AddComponent(new DefaultTileHitProperties(
                acceptsDirectHit: false,
                acceptsSideHit: true,
                suppressesHitToSideHit: true,
                propagatesHitToCell: false));
            tile.AddComponent(new TileHoneyMechanicProperties(canBeTakenOverByHoney: false));
            tile.AddComponent(new TileButterflyMechanicProperties(canButterflyBeGeneratedOn: false));
            tile.AddComponent(new VacuumCleanerMechanicProperties(vacuumCleanerMustHitOnlyOnce: true));
            tile.AddComponent(new IvyMechanicTileProperties(canBeTakenOverByIvy: false));


            return tile;
        }

        public Tile CreateTableClothMainTile(Size size, TableClothMainTile.TargetHandler firstTarget, TableClothMainTile.TargetHandler secondTarget)
        {
            var tile = new TableClothMainTile(size, firstTarget, secondTarget);
            tile.AddComponent(new TilePhysicalProperties(isAffectedByGravity: false));
            tile.AddComponent(new TileMatchingProperties(allowsMatchFallThrough: false));
            tile.AddComponent(new TileUserInteractionProperties(isSwappable: false));
            tile.AddComponent(new TileRainbowProperties(matchingRule: TileRainbowProperties.MatchingRule.None));
            tile.AddComponent(new TilePowerUpProperties(isHammerTarget: false));
            tile.AddComponent(new DefaultTileHitProperties(
                acceptsDirectHit: false,
                acceptsSideHit: false,
                suppressesHitToSideHit: true,
                propagatesHitToCell: false));
            tile.AddComponent(new TileHoneyMechanicProperties(canBeTakenOverByHoney: false));
            tile.AddComponent(new TileButterflyMechanicProperties(canButterflyBeGeneratedOn: false));
            tile.AddComponent(new VacuumCleanerMechanicProperties(vacuumCleanerMustHitOnlyOnce: true));
            tile.AddComponent(new IvyMechanicTileProperties(canBeTakenOverByIvy: false));


            return tile;

        }

        public Tile CreateFishStatueTile(int level)
        {
            var tile = new FishStatue(level);
            tile.AddComponent(new TilePhysicalProperties(isAffectedByGravity: false));
            tile.AddComponent(new TileMatchingProperties(allowsMatchFallThrough: false));
            tile.AddComponent(new TileUserInteractionProperties(isSwappable: false));
            tile.AddComponent(new TileRainbowProperties(matchingRule: TileRainbowProperties.MatchingRule.None));
            tile.AddComponent(new TilePowerUpProperties(isHammerTarget: true));
            tile.AddComponent(new DefaultTileHitProperties(
                acceptsDirectHit: true,
                acceptsSideHit: false,
                suppressesHitToSideHit: true,
                propagatesHitToCell: false));
            tile.AddComponent(new TileHoneyMechanicProperties(canBeTakenOverByHoney: false));
            tile.AddComponent(new TileButterflyMechanicProperties(canButterflyBeGeneratedOn: false));
            tile.AddComponent(new VacuumCleanerMechanicProperties(vacuumCleanerMustHitOnlyOnce: false));
            tile.AddComponent(new IvyMechanicTileProperties(canBeTakenOverByIvy: false));


            return tile;
        }


        public Tile CreateBuoyantTile(TileColor color)
        {
            var tile = new Buoyant(color);
            tile.AddComponents(
                new TilePhysicalProperties(isAffectedByGravity: false),
                new TileMatchingProperties(allowsMatchFallThrough: false),
                new TileUserInteractionProperties(isSwappable: false),
                new TileRainbowProperties(matchingRule: TileRainbowProperties.MatchingRule.None),
                new TilePowerUpProperties(isHammerTarget: true),
                new DefaultTileHitProperties(
                    acceptsDirectHit: true,
                    acceptsSideHit: true,
                    suppressesHitToSideHit: true,
                    propagatesHitToCell: false),
                new TileHoneyMechanicProperties(canBeTakenOverByHoney: false),
                new TileButterflyMechanicProperties(canButterflyBeGeneratedOn: false),
                new VacuumCleanerMechanicProperties(vacuumCleanerMustHitOnlyOnce: false),
                new IvyMechanicTileProperties(canBeTakenOverByIvy: false));


            return tile;
        }

        public Tile CreateDuckTile (IEnumerable<Type> childTilesTypes)
        {
            var tile = new Duck(childTilesTypes);
            tile.AddComponents(
                new TilePhysicalProperties(isAffectedByGravity: false),
                new TileMatchingProperties(allowsMatchFallThrough: false),
                new TileUserInteractionProperties(isSwappable: false),
                new TileRainbowProperties(matchingRule: TileRainbowProperties.MatchingRule.None),
                new TilePowerUpProperties(isHammerTarget: true),
                new DefaultTileHitProperties(acceptsDirectHit: true, acceptsSideHit: true, suppressesHitToSideHit: true, propagatesHitToCell: false),
                new TileHoneyMechanicProperties(canBeTakenOverByHoney: false),
                new TileButterflyMechanicProperties(canButterflyBeGeneratedOn: false),
                new VacuumCleanerMechanicProperties(vacuumCleanerMustHitOnlyOnce: false),
                new IvyMechanicTileProperties(canBeTakenOverByIvy: false));

            return tile;
        }

        public Tile CreateGrassSack()
        {
            var tile = new GrassSackMainTile();
            tile.AddComponents(
                new TilePhysicalProperties(isAffectedByGravity: false),
                new TileMatchingProperties(allowsMatchFallThrough: false),
                new TileUserInteractionProperties(isSwappable: false),

                new TileRainbowProperties(matchingRule: TileRainbowProperties.MatchingRule.None),
                new TilePowerUpProperties(isHammerTarget: true),
                new DefaultTileHitProperties(
                    acceptsDirectHit: true,
                    acceptsSideHit: true,
                    suppressesHitToSideHit: true,
                    propagatesHitToCell: false),
                new TileHoneyMechanicProperties(canBeTakenOverByHoney: false),
                new TileButterflyMechanicProperties(canButterflyBeGeneratedOn: false),
                new VacuumCleanerMechanicProperties(vacuumCleanerMustHitOnlyOnce: false),
                new IvyMechanicTileProperties(canBeTakenOverByIvy: false));
            return tile;
        }

        public Tile CreateJamJarTile()
        {
            var tile = new JamJar();
            tile.AddComponents(
                new TilePhysicalProperties(isAffectedByGravity: true),
                new TileMatchingProperties(allowsMatchFallThrough: false),
                new TileUserInteractionProperties(isSwappable: true),
                new TileRainbowProperties(matchingRule: TileRainbowProperties.MatchingRule.None),
                new TilePowerUpProperties(isHammerTarget: true),
                new DefaultTileHitProperties(
                    acceptsDirectHit: true,
                    acceptsSideHit: true,
                    suppressesHitToSideHit: true,
                    propagatesHitToCell: false),
                new TileHoneyMechanicProperties(canBeTakenOverByHoney: false),
                new TileButterflyMechanicProperties(canButterflyBeGeneratedOn: false),
                new VacuumCleanerMechanicProperties(vacuumCleanerMustHitOnlyOnce: false),
                new IvyMechanicTileProperties(canBeTakenOverByIvy: false));
            return tile;
        }

        public Tile CreateSlaveTile(CompositeTile master)
        {
            var tile = new SlaveTile(master);

            // WARNING: It's a copy by reference. 
            foreach (var comp in master.AllComponents())
                tile.AddComponent(comp);

            return tile;
        }

        public Tile CreateCat()
        {
            var tile = new Cat();
            tile.AddComponents(
                new TilePhysicalProperties(isAffectedByGravity: false),
                new TileMatchingProperties(allowsMatchFallThrough: false),
                new TileUserInteractionProperties(isSwappable: false),
                new TileRainbowProperties(matchingRule: TileRainbowProperties.MatchingRule.None),
                new TilePowerUpProperties(isHammerTarget: false),
                new DefaultTileHitProperties(
                    acceptsDirectHit: false,
                    acceptsSideHit: false,
                    suppressesHitToSideHit: true,
                    propagatesHitToCell: false),
                new TileHoneyMechanicProperties(canBeTakenOverByHoney: false),
                new TileButterflyMechanicProperties(canButterflyBeGeneratedOn: false),
                new VacuumCleanerMechanicProperties(vacuumCleanerMustHitOnlyOnce: false),
                new IvyMechanicTileProperties(canBeTakenOverByIvy: false));
            return tile;
        }

        public Tile CreateCatFood()
        {
            var tile = new CatFood();
            tile.AddComponents(
                new TilePhysicalProperties(isAffectedByGravity: false),
                new TileMatchingProperties(allowsMatchFallThrough: false),
                new TileUserInteractionProperties(isSwappable: false),
                new TileRainbowProperties(matchingRule: TileRainbowProperties.MatchingRule.None),
                new TilePowerUpProperties(isHammerTarget: false),
                new DefaultTileHitProperties(
                    acceptsDirectHit: false,
                    acceptsSideHit: false,
                    suppressesHitToSideHit: false,
                    propagatesHitToCell: false),
                new TileHoneyMechanicProperties(canBeTakenOverByHoney: false),
                new TileButterflyMechanicProperties(canButterflyBeGeneratedOn: false),
                new VacuumCleanerMechanicProperties(vacuumCleanerMustHitOnlyOnce: false),
                new IvyMechanicTileProperties(canBeTakenOverByIvy: false));
            return tile;
        }

        public Tile CreateCatColoredBead(TileColor color)
        {
            var tile = new CatColoredBead();
            tile.AddComponent(new TilePhysicalProperties(isAffectedByGravity: true));
            tile.AddComponent(new TileMatchingProperties(allowsMatchFallThrough: false));
            tile.AddComponent(new TileUserInteractionProperties(isSwappable: true));
            tile.AddComponent(new TileRainbowProperties(matchingRule: TileRainbowProperties.MatchingRule.MatchWithRainbowFindOtherMatchesBySameColor));
            tile.AddComponent(new TilePowerUpProperties(isHammerTarget: true));
            tile.AddComponent(new DefaultTileHitProperties(
                acceptsDirectHit: true,
                acceptsSideHit: false,
                suppressesHitToSideHit: false,
                propagatesHitToCell: true));
            tile.AddComponent(new TileHoneyMechanicProperties(canBeTakenOverByHoney: true));
            tile.AddComponent(new TileColorComponent(color: color));
            tile.AddComponent(new TileButterflyMechanicProperties(canButterflyBeGeneratedOn: true));
            tile.AddComponent(new VacuumCleanerMechanicProperties(vacuumCleanerMustHitOnlyOnce: false));
            tile.AddComponent(new IvyMechanicTileProperties(canBeTakenOverByIvy: true));

            return tile;
        }

        public Tile CreateIceMakerTile()
        {
            var tile = new IceMakerMainTile();
            tile.AddComponent(new TilePhysicalProperties(isAffectedByGravity: false));
            tile.AddComponent(new TileMatchingProperties(allowsMatchFallThrough: false));
            tile.AddComponent(new TileUserInteractionProperties(isSwappable: false));
            tile.AddComponent(new TileRainbowProperties(matchingRule: TileRainbowProperties.MatchingRule.None));
            tile.AddComponent(new TilePowerUpProperties(isHammerTarget: true));
            tile.AddComponent(new DefaultTileHitProperties(
                                                           acceptsDirectHit: true,
                                                           acceptsSideHit: true,
                                                           suppressesHitToSideHit: true,
                                                           propagatesHitToCell: false));
            tile.AddComponent(new TileHoneyMechanicProperties(canBeTakenOverByHoney: false));
            tile.AddComponent(new TileButterflyMechanicProperties(canButterflyBeGeneratedOn: false));
            tile.AddComponent(new VacuumCleanerMechanicProperties(vacuumCleanerMustHitOnlyOnce: true));
            tile.AddComponent(new IvyMechanicTileProperties(canBeTakenOverByIvy: false));

            return tile;
        }

        public Tile CreateCompass()
        {
            var tile = new CompassTile();
            tile.AddComponent(new TilePhysicalProperties(isAffectedByGravity: true));
            tile.AddComponent(new TileMatchingProperties(allowsMatchFallThrough: false));
            tile.AddComponent(new TileUserInteractionProperties(isSwappable: false));
            tile.AddComponent(new TileRainbowProperties(matchingRule: TileRainbowProperties.MatchingRule.None));
            tile.AddComponent(new TilePowerUpProperties(isHammerTarget: true));
            tile.AddComponent(new DefaultTileHitProperties(
                                  acceptsDirectHit: true,
                                  acceptsSideHit: false,
                                  suppressesHitToSideHit: true,
                                  propagatesHitToCell: false));
            tile.AddComponent(new TileHoneyMechanicProperties(canBeTakenOverByHoney: false));
            tile.AddComponent(new TileButterflyMechanicProperties(canButterflyBeGeneratedOn: false));
            tile.AddComponent(new VacuumCleanerMechanicProperties(vacuumCleanerMustHitOnlyOnce: true));
            tile.AddComponent(new IvyMechanicTileProperties(canBeTakenOverByIvy: false));

            return tile;
        }

        public Tile CreateIvySackTile(int level)
        {
            var tile = new IvySackTile(level);
            
            tile.AddComponent(new TilePhysicalProperties(isAffectedByGravity: true));
            tile.AddComponent(new TileMatchingProperties(allowsMatchFallThrough: false));
            tile.AddComponent(new TileUserInteractionProperties(isSwappable: true));
            tile.AddComponent(new TileRainbowProperties(matchingRule: TileRainbowProperties.MatchingRule.None));
            tile.AddComponent(new TilePowerUpProperties(isHammerTarget: true));
            tile.AddComponent(new DefaultTileHitProperties(
                acceptsDirectHit: true,
                acceptsSideHit: true,
                suppressesHitToSideHit: true,
                propagatesHitToCell: false));
            tile.AddComponent(new TileHoneyMechanicProperties(canBeTakenOverByHoney: false));
            tile.AddComponent(new TileButterflyMechanicProperties(canButterflyBeGeneratedOn: false));
            tile.AddComponent(new VacuumCleanerMechanicProperties(vacuumCleanerMustHitOnlyOnce: false));
            tile.AddComponent(new IvyMechanicTileProperties(canBeTakenOverByIvy: true));

            return tile;
        }

        public Tile CreateLouieTile(int level)
        {
            var tile = new Louie(level);

            tile.AddComponents(
                new TilePhysicalProperties(isAffectedByGravity: false),
                new TileMatchingProperties(allowsMatchFallThrough: false),
                new TileUserInteractionProperties(isSwappable: false),
                new TileRainbowProperties(matchingRule: TileRainbowProperties.MatchingRule.None),
                new TilePowerUpProperties(isHammerTarget: true),
                new DefaultTileHitProperties(acceptsDirectHit: true, acceptsSideHit: true, suppressesHitToSideHit: true, propagatesHitToCell: false),
                new TileHoneyMechanicProperties(canBeTakenOverByHoney: false),
                new TileButterflyMechanicProperties(canButterflyBeGeneratedOn: false),
                new VacuumCleanerMechanicProperties(vacuumCleanerMustHitOnlyOnce: false),
                new IvyMechanicTileProperties(canBeTakenOverByIvy: false)
            );

            return tile;
        }

        public Tile CreateCoconutTile(int level)
        {
            var tile = new Coconut(level);

            tile.AddComponents(
                new TilePhysicalProperties(isAffectedByGravity: true),
                new TileMatchingProperties(allowsMatchFallThrough: false),
                new TileUserInteractionProperties(isSwappable: true),
                new TileRainbowProperties(matchingRule: TileRainbowProperties.MatchingRule.None),
                new TilePowerUpProperties(isHammerTarget: true),
                new DefaultTileHitProperties(acceptsDirectHit: true, acceptsSideHit: false, suppressesHitToSideHit: true, propagatesHitToCell: false),
                new TileHoneyMechanicProperties(canBeTakenOverByHoney: false),
                new TileButterflyMechanicProperties(canButterflyBeGeneratedOn: false),
                new VacuumCleanerMechanicProperties(vacuumCleanerMustHitOnlyOnce: false),
                new IvyMechanicTileProperties(canBeTakenOverByIvy: false)
            );

            return tile;
        }
    }
}
