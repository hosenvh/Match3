using System;
using System.Collections.Generic;
using Match3.Game.Gameplay.SubSystems.HintingAndShuffling;
using Match3.Game.Gameplay.SubSystemsData.SessionData;


namespace Match3.Game.Gameplay.SubSystems.TileRandomPlacement
{
    public struct TileRandomPlacementKeyType : KeyType
    {
    }

    public class TileRandomPlacementSystem : GameplaySystem
    {
        private readonly List<TileRandomPlacer> tilePlacers = new List<TileRandomPlacer>();
        private TileRandomPlacer activeTilePlacer;

        public TileRandomPlacementSystem(GameplayController gameplayController) : base(gameplayController)
        {
            AddTilePlacer(new DefaultTileRandomPlacer(priority: 1, gameplayController.GetPresentationHandler<DefaultTilePlacerPresentationHandler>())); // TODO: For sure find a bettor place to add this
        }

        public void AddTilePlacer(TileRandomPlacer tilePlacer)
        {
            tilePlacers.Add(tilePlacer);
            tilePlacer.Setup(gameplayController);
        }

        public T GetTilePlacer<T>() where T : TileRandomPlacer
        {
            return (T) tilePlacers.Find(placer => placer is T);
        }

        public override void Start()
        {
            CheckForPlacings();
        }

        public override void Reset()
        {
            CheckForPlacings();
        }

        private void CheckForPlacings()
        {
            if (tilePlacers.Count != 0)
                StartPlacings();
            else
                DeactivateSystem();

            void StartPlacings()
            {
                SortTilePlacers();
                GetReadyForTilePlacings();

                activeTilePlacer = tilePlacers[0];
                StartActiveTilePlacer(
                    activeTilePlacerCompleted: () => StartNextTilePlacers(
                        onNoMoreTilePlacerLeft: FinishTilePlacings));

                void StartActiveTilePlacer(Action activeTilePlacerCompleted)
                {
                    if (activeTilePlacer.AnyTileToPlace())
                        activeTilePlacer.StartTilePlacing(activeTilePlacerCompleted);
                    else
                        activeTilePlacerCompleted.Invoke();
                }

                void StartNextTilePlacers(Action onNoMoreTilePlacerLeft)
                {
                    activeTilePlacer = tilePlacers.NextOf(activeTilePlacer);
                    if (activeTilePlacer != null)
                        StartActiveTilePlacer(activeTilePlacerCompleted: () => StartNextTilePlacers(onNoMoreTilePlacerLeft));
                    else
                        onNoMoreTilePlacerLeft?.Invoke();
                }

                void GetReadyForTilePlacings()
                {
                    gameplayController.GetSystem<HintingSystem>().SetManualHint(new EmptyHint());
                    GetSessionData<InputControlData>().AddLockedBy<TileRandomPlacementKeyType>();
                }

                void FinishTilePlacings()
                {
                    GetSessionData<InputControlData>().RemoveLockedBy<TileRandomPlacementKeyType>();
                    gameplayController.GetSystem<HintingSystem>().SetManualHint(null);

                    DeactivateSystem();
                }

                void SortTilePlacers()
                {
                    tilePlacers.Sort((a, b) => a.Priority < b.Priority ? -1 : 1);
                }
            }

            void DeactivateSystem()
            {
                gameplayController.DeactivateSystem<TileRandomPlacementSystem>();
            }
        }

        public override void Update(float dt)
        {
        }
    }
}