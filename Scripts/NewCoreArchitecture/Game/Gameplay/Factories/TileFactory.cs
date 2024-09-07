using System.Collections.Generic;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Tiles;
using System;
using UnityEngine;


namespace Match3.Game.Gameplay.Factories
{
    // TODO: Try to create an extendable factory.
    public interface TileFactory : Service
    {
        TileStack CreateTileStack();
        Tile CreateBoxTile(int level);
        Tile CreateColoredBoxTile(int level, TileColor color);
        Tile CreateCleanColoredBeadTile(TileColor color);
        Tile CreateColoredBeadTile(TileColor color, ColoredBead.DirtinessState dirtinessState);

        Tile CreateChainTile(int level);
        Tile CreateLemonadeTile();
        Tile CreateChickTile();
        Tile CreateNutTile(int initialLevel);
        Tile CreateIceTile(int level);
        Tile CreateRockTile(int level);
        Tile CreateHoneyTile();
        Tile CreateHardenedHoneyTile();
        Tile CreateChickenNestTile();
        Tile CreateBalloonTiedTile(Vector2Int relationDirectionFromItsMainBalloonTile);
        Tile CreateBalloonMainTile();
        Tile CreateGardenTile();

        Tile CreateSandTile(int level);
        Tile CreateSandWithGemTile(int level);

        Tile CreateExtraMoveTile(int moveAmount);

        Tile CreateRainbowTile();

        Tile CreateRocketTile();
        Tile CreateBombTile();
        Tile CreateDynamiteTile();
        Tile CreateTNTBarrelTile();

        Tile CreateFireflyJar(int level);
        Tile CreateButterflyTile(TileColor color);
        Tile CreateRocketBoxTile(TileColor color);

        Tile CreateVacuumCleaner(TileColor targetColor, int targetAmount, Direction direction, int priority);
        Tile CreateHoneyJarTile();
        Tile CreateHoneycomb();
        Tile CreatePadlock(Padlock.Status status);
        Tile CreateIvyBushTile();
        Tile CreateGasCylinder(TileColor color, int startCountdown);
        Tile CreateBeachBallTile(HashSet<TileColor> colors);
        Tile CreateSlaveTile(CompositeTile master);

        Tile CreateTableClothMainTile(Size size, TableClothMainTile.TargetHandler firstTarget, TableClothMainTile.TargetHandler secondTarget);
        Tile CreateFishStatueTile(int level);
        Tile CreateBuoyantTile(TileColor color);
        Tile CreateGrassSack();
        Tile CreateJamJarTile();

        Tile CreateCat();
        Tile CreateCatFood();
        Tile CreateCatColoredBead(TileColor color);

        Tile CreateIceMakerTile();

        Tile CreateCompass();
        Tile CreateIvySackTile(int level);
        Tile CreateDuckTile (IEnumerable<Type> childTilesTypes);
        Tile CreateLouieTile(int level);
        Tile CreateCoconutTile(int level);
    }
}
