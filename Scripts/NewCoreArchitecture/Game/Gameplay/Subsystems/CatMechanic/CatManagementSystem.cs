using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.DestructionManagement;
using Match3.Game.Gameplay.HitManagement;
using Match3.Game.Gameplay.SubSystems.General;
using Match3.Game.Gameplay.Swapping;
using Match3.Game.Gameplay.Tiles;
using System;
using System.Collections.Generic;


namespace Match3.Game.Gameplay.SubSystems.CatMechanic
{
    public struct CatManagementKeyType : KeyType
    {

    }

    public interface CatManagementSystemPort : PresentationHandler
    {
        void SwapCat(Cat cat, CellStack target, Action onComplete);
        void Hit(Cat cat, CellStack target, Action onHit, Action onComplete);
        void MoveCatToFood(Cat cat, CatFood catFood, Action onComplete);
    }

    [Before(typeof(DestructionSystem))]
    public class CatManagementSystem : GameplaySystem
    {
        CatManagementSystemPort port;
        GeneralHitActivatingSystem generalHitActivatingSystem;

        Cat managedCat;
        CatPathCell currentPathCell;

        int enqueuedActions = 0;
        bool isExecutingAnAction = false;

        public CatManagementSystem(GameplayController gameplayController) : base(gameplayController)
        {
            port = gameplayController.GetPresentationHandler<CatManagementSystemPort>();

            managedCat = QueryUtilities.FindFirstTileInBoard<Cat>(gameplayController.GameBoard());

            if (managedCat == null)
            {
                gameplayController.DeactivateSystem<CatManagementSystem>();
                return;
            }
            
            currentPathCell = QueryUtilities.FindCell<CatPathCell>(managedCat.Parent().Parent());
        }

        public override void Start()
        {
            generalHitActivatingSystem = gameplayController.GetSystem<GeneralHitActivatingSystem>();
        }

        public override void Update(float dt)
        {
            if (IsActive() == false)
                return;

            if (enqueuedActions > 0 && isExecutingAnAction == false)
                ExecuteAnAction();

        }

        private void ExecuteAnAction()
        {
            var nextCell = currentPathCell.NextCell();

            if (QueryUtilities.IsFullyFree(nextCell.Parent()) == false)
                return;

            enqueuedActions--;
            isExecutingAnAction = true;

            if (IsSwappable(nextCell))
                SwapCat(nextCell, onCompleted: () => isExecutingAnAction = false);

            else if (HasCatFood(nextCell))
                MoveCatToFood(CatFoodIn(nextCell), onCompleted: () => isExecutingAnAction = false);

            else if(CanHit(nextCell))
                Hit(nextCell, onCompleted: () => isExecutingAnAction = false);

            else
            {
                enqueuedActions++;
                isExecutingAnAction = false;
            }
        }

        private void SwapCat(CatPathCell nextCell, Action onCompleted)
        {
            ActionUtilites.FullyLock<CatManagementKeyType>(managedCat.Parent().Parent());
            ActionUtilites.FullyLock<CatManagementKeyType>(nextCell.Parent());

            port.SwapCat(
                managedCat,
                nextCell.Parent(),
                onComplete: () => CompleteCatSwapping(nextCell, onCompleted));
        }

        private void CompleteCatSwapping(CatPathCell nextCell, Action onCompleted)
        {
            ActionUtilites.FullyUnlock(managedCat.Parent().Parent());
            ActionUtilites.FullyUnlock(nextCell.Parent());

            currentPathCell = nextCell; 
            ActionUtilites.SwapTileStacksOf(managedCat.Parent().Parent(), nextCell.Parent());
            onCompleted.Invoke();
        }

        private void MoveCatToFood(CatFood catFood, Action onCompleted)
        {
            ActionUtilites.FullyLock<CatManagementKeyType>(managedCat.Parent().Parent());
            ActionUtilites.FullyLock<CatManagementKeyType>(catFood.Parent().Parent());

            port.MoveCatToFood(
                managedCat,
                catFood,
                () => CompleteCatMovingToFood(catFood, onCompleted));
        }

        private void CompleteCatMovingToFood(CatFood catFood, Action onCompleted)
        {
            ActionUtilites.FullyUnlock(managedCat.Parent().Parent());
            ActionUtilites.FullyUnlock(catFood.Parent().Parent());

            ActionUtilites.SwapTileStacksOf(managedCat.Parent().Parent(), catFood.Parent().Parent());

            ActionUtilites.FullyDestroy(catFood);
            GetFrameData<DestructionData>().tilesToDestroy.Add(catFood);

            managedCat = null;
            onCompleted.Invoke();
        }

        private void Hit(CatPathCell nextCell, Action onCompleted)
        {
            ActionUtilites.FullyLock<CatManagementKeyType>(managedCat.Parent().Parent());
            ActionUtilites.FullyLock<CatManagementKeyType>(nextCell.Parent());

            port.Hit(
                managedCat,
                nextCell.Parent(),
                onHit: () => generalHitActivatingSystem.ActivateHit(nextCell.Parent()),
                onComplete: () => CompleteHitting(nextCell, onCompleted));;
        }

        private void CompleteHitting(CatPathCell nextCell, Action onCompleted)
        {
            ActionUtilites.FullyUnlock(managedCat.Parent().Parent());
            ActionUtilites.FullyUnlock(nextCell.Parent());
            onCompleted.Invoke();
        }

        private bool IsSwappable(CatPathCell catPathCell)
        {
            var cellStack = catPathCell.Parent();
            if (cellStack.HasTileStack() == false)
                return true;

            return cellStack.CurrentTileStack().IsDepleted()
                || cellStack.CurrentTileStack().Top().GetComponent<TileUserInteractionProperties>().isSwappable;
        }

        private bool HasCatFood(CatPathCell catPathCell)
        {
            return QueryUtilities.HasTileOnTop<CatFood>(catPathCell.Parent());
        }

        private CatFood CatFoodIn(CatPathCell nextCell)
        {
            return QueryUtilities.FindTile<CatFood>(nextCell.Parent());
        }

        private bool CanHit(CatPathCell nextCell)
        {
            return nextCell.Parent().HasTileStack();
        }


        public void EnequeAction()
        {
            enqueuedActions++;
        }

        public bool IsActive()
        {
            return managedCat != null;
        }

        public Cat TargetCat()
        {
            return managedCat;
        }

        public CatPathCell CurrentPathCell()
        {
            return currentPathCell;
        }
    }
 }
