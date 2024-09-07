using System;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.ExplosionManagement;
using Match3.Game.Gameplay.SubSystems.HintingAndShuffling;
using Match3.Game.Gameplay.SubSystems.LevelEnding;
using Match3.Game.Gameplay.SubSystems.PowerUpManagement;
using Match3.Game.Gameplay.Swapping;
using Match3.Game.Gameplay.Tiles;
using Match3.Presentation.Gameplay;
using Match3.Presentation.Gameplay.LogicPresentationHandlers;
using Match3.Presentation.Gameplay.PowerUpActivation;
using UnityEngine;

namespace Match3.Game
{
    public class GameplayTutorialManager : Service, EventListener
    {

        global::Game gameManager;

        int levelIndex;

        CellStackBoard cellStackBoard;
        GameBoard gameBoard;
        HintingSystem hintingSystem;
        //bool showAutoGuide;

        public GameplayTutorialManager()
        {
            ServiceLocator.Find<EventManager>().Register(this);
            gameManager = Base.gameManager;
        }

        public void OnEvent(GameEvent evt, object sender)
        {
            if (Base.gameManager.CurrentState is CampaignGameplayState == false)
                return;
            if (evt is LevelPresentationInitializedEvent iEvt)
            {
                levelIndex = iEvt.levelIndex;
                gameBoard = iEvt.gpc.GameBoard();
                cellStackBoard = iEvt.gpc.GameBoard().CellStackBoard();
                hintingSystem = iEvt.gpc.GetSystem<HintingSystem>();
                CheckTutorial();
            }
            else if (evt is ExplosiveActivatedEvent || evt is SwapSuccessfulEvent || evt is PowerUpSelectedEvent || evt is PowerUpActivatedEvent)
                CheckTutorial();
            else if (evt is LevelEndedEvent)
            {
                Clear();
            }
        }

        private void Clear()
        {
            levelIndex = -1;
            cellStackBoard = null;
            hintingSystem = null;
            gameBoard = null;
        }

        private void CheckTutorial()
        {
            switch (levelIndex)
            {
                case 0:
                    if (gameManager.tutorialManager.CheckAndHideTutorial(5))
                    {
                        ResumeShowingMainHint();
                        gameManager.tutorialManager.CheckThenShowTutorial(6, .5f, null);
                    }
                    else
                    {
                        if (gameManager.tutorialManager.CheckThenShowTutorial(5, 1, null))
                        {
                            StopShowingMainHint();
                            //showAutoGuide = false;
                            ShowMoveHint(5, 3, Direction.Left);
                            //guidePresenter.SetShowingGuide(cellPresenters[3, 5], Dir.Left);
                        }
                    }
                    break;
                case 1:
                    if (gameManager.tutorialManager.CheckAndHideTutorial(16))
                    {
                        ResumeShowingMainHint();
                        //showAutoGuide = true;
                        gameManager.tutorialManager.CheckThenShowTutorial(17, .5f, null);
                    }
                    else if (gameManager.tutorialManager.CheckAndHideTutorial(7))
                    {
                        if (gameManager.tutorialManager.CheckThenShowTutorial(16, .5f, null))
                            ShowMoveHint(3, 4, Direction.Up);
                        // guidePresenter.SetShowingGuide(cellPresenters[4, 3], Dir.Up);
                    }
                    else
                    {
                        if (gameManager.tutorialManager.CheckThenShowTutorial(7, 1, null))
                        {
                            StopShowingMainHint();
                            //showAutoGuide = false;
                            ShowMoveHint(2, 3, Direction.Right);
                            //guidePresenter.SetShowingGuide(cellPresenters[3, 2], Dir.Right);
                        }
                    }
                    break;
                case 2:
                    if (gameManager.tutorialManager.CheckAndHideTutorial(20))
                    {
                        ResumeShowingMainHint();
                        //showAutoGuide = true;
                        gameManager.tutorialManager.CheckThenShowTutorial(21, .5f, null);
                    }
                    else if (gameManager.tutorialManager.CheckAndHideTutorial(19))
                    {
                        if (gameManager.tutorialManager.CheckThenShowTutorial(20, .5f, null))
                            ShowActivatingHint(6, 6);
                        // guidePresenter.SetShowingGuide(cellPresenters[6, 6], Dir.None);
                    }
                    else if (gameManager.tutorialManager.CheckAndHideTutorial(8))
                    {
                        if (gameManager.tutorialManager.CheckThenShowTutorial(19, .5f, null))
                            ShowMoveHint(7, 6, Direction.Left);
                        // guidePresenter.SetShowingGuide(cellPresenters[6, 7], Dir.Left);
                    }
                    else if (gameManager.tutorialManager.CheckAndHideTutorial(18))
                    {
                        if (gameManager.tutorialManager.CheckThenShowTutorial(8, .5f, null))
                            ShowMoveHint(4, 3, Direction.Left);
                        // guidePresenter.SetShowingGuide(cellPresenters[3, 4], Dir.Left);
                    }
                    else
                    {
                        if (gameManager.tutorialManager.CheckThenShowTutorial(18, 1, null))
                        {
                            StopShowingMainHint();
                            //showAutoGuide = false;
                            ShowMoveHint(9, 0, Direction.Down);
                            // guidePresenter.SetShowingGuide(cellPresenters[0, 9], Dir.Down);
                        }
                    }
                    break;
                case 3:
                    if (gameManager.tutorialManager.CheckAndHideTutorial(24))
                    {
                        ResumeShowingMainHint();
                        //showAutoGuide = true;
                        gameManager.tutorialManager.CheckThenShowTutorial(14, 1, null);
                    }
                    else if (gameManager.tutorialManager.CheckAndHideTutorial(23))
                    {
                        if (gameManager.tutorialManager.CheckThenShowTutorial(24, .5f, null))
                            ShowActivatingHint(2, 4);
                        ;// guidePresenter.SetShowingGuide(cellPresenters[4, 2], Dir.None);
                    }
                    else
                    {
                        if (gameManager.tutorialManager.CheckThenShowTutorial(23, 1, null))
                        {
                            StopShowingMainHint();
                            //showAutoGuide = false;
                            ShowMoveHint(3, 3, Direction.Left);

                            ;// guidePresenter.SetShowingGuide(cellPresenters[3, 3], Dir.Left);
                        }
                    }
                    break;
                case 4:
                    if (gameManager.tutorialManager.CheckAndHideTutorial(9))
                    {
                        ResumeShowingMainHint();
                        //showAutoGuide = true;
                        gameManager.tutorialManager.CheckThenShowTutorial(25, 1, null);
                    }
                    else
                    {
                        if (gameManager.tutorialManager.CheckThenShowTutorial(9, 1, null))
                        {
                            StopShowingMainHint();
                            //showAutoGuide = false;
                            ShowMoveHint(9, 6, Direction.Down);

                            // guidePresenter.SetShowingGuide(cellPresenters[6, 9], Dir.Down);
                        }
                    }
                    break;
                case 5:
                    gameManager.tutorialManager.CheckThenShowTutorial(26, 1, null);
                    break;
                case 6:
                    //showAutoGuide = false;
                    if (gameManager.tutorialManager.CheckAndHideTutorial(67))
                        ResumeShowingMainHint();
                    else
                    {
                        if (gameManager.tutorialManager.CheckAndHideTutorial(36))
                        {
                            ResumeShowingMainHint();
                            gameManager.tutorialManager.CheckThenShowTutorial(67, .25f, null);
                        }
                        else if (gameManager.tutorialManager.CheckThenShowTutorial(36, 1, null))
                            StopShowingMainHint();
                    }
                    break;
                case 8:
                    gameManager.tutorialManager.CheckThenShowTutorial(10, 1, null);
                    break;
                case 10:
                    gameManager.tutorialManager.CheckThenShowTutorial(11, 1, null);
                    break;
                case 11:
                    gameManager.tutorialManager.CheckThenShowTutorial(15, 1, null);
                    break;
                case 17:
                    gameManager.tutorialManager.CheckThenShowTutorial(12, 1, null);
                    break;
                case 25:
                    gameManager.tutorialManager.CheckThenShowTutorial(13, 1, null);
                    break;
                case 27:
                    if (gameManager.tutorialManager.CheckAndHideTutorial(68))
                        ResumeShowingMainHint();
                    else
                    {
                        if (gameManager.tutorialManager.CheckAndHideTutorial(37))
                        {
                            ResumeShowingMainHint();
                            gameManager.tutorialManager.CheckThenShowTutorial(68, .25f, null);
                        }
                        else if (gameManager.tutorialManager.CheckThenShowTutorial(37, 1, null))
                            StopShowingMainHint();
                    }
                    break;
                case 33:
                    if (gameManager.tutorialManager.CheckAndHideTutorial(69))
                        ResumeShowingMainHint();
                    else
                    {
                        if (gameManager.tutorialManager.CheckAndHideTutorial(38))
                        {
                            ResumeShowingMainHint();
                            gameManager.tutorialManager.CheckThenShowTutorial(69, .25f, null);
                        }
                        else if (gameManager.tutorialManager.CheckThenShowTutorial(38, 1, null))
                            StopShowingMainHint();
                    }
                    break;
                case 43:
                    if (gameManager.tutorialManager.CheckAndHideTutorial(45))
                        ResumeShowingMainHint();
                    else
                    {
                        if (gameManager.tutorialManager.CheckThenShowTutorial(45, 1, null))
                        {
                            StopShowingMainHint();
                            ShowMoveHint(5, 4, Direction.Down);
                        }
                    }
                    break;
                case 50:
                    gameManager.tutorialManager.CheckThenShowTutorial(31, 1, null);
                    break;
                case 60:
                    gameManager.tutorialManager.CheckThenShowTutorial(32, 1, null);
                    break;
                case 70:
                    gameManager.tutorialManager.CheckThenShowTutorial(33, 1, null);
                    break;
                case 78:
                    gameManager.tutorialManager.CheckThenShowTutorial(87, 1, null);
                    break;
                case 94:
                    gameManager.tutorialManager.CheckThenShowTutorial(39, 1, null);
                    break;
                case 100:
                    gameManager.tutorialManager.CheckThenShowTutorial(35, 1, null);
                    break;
                case 160:
                    gameManager.tutorialManager.CheckThenShowTutorial(40, 1, null);
                    break;
                case 189:
                    gameManager.tutorialManager.CheckThenShowTutorial(41, 1, null);
                    break;
                case 220:
                    gameManager.tutorialManager.CheckThenShowTutorial(42, 1, null);
                    break;
                case 235:
                    gameManager.tutorialManager.CheckThenShowTutorial(43, 1, null);
                    break;
                case 285:
                    gameManager.tutorialManager.CheckThenShowTutorial(44, 1, null);
                    break;
                case 320:
                    gameManager.tutorialManager.CheckThenShowTutorial(46, 1, null);
                    break;
                case 260:
                    gameManager.tutorialManager.CheckThenShowTutorial(47, 1, null);
                    break;
                case 375:
                    gameManager.tutorialManager.CheckThenShowTutorial(48, 1, null);
                    break;
                case 475:
                    gameManager.tutorialManager.CheckThenShowTutorial(49, 1, null);
                    break;
                case 506:
                    gameManager.tutorialManager.CheckThenShowTutorial(50, 1, null);
                    break;
                case 525:
                    gameManager.tutorialManager.CheckThenShowTutorial(51, 1, null);
                    break;
                case 575:
                    gameManager.tutorialManager.CheckThenShowTutorial(62, 1, null);
                    break;
                case 625:
                    gameManager.tutorialManager.CheckThenShowTutorial(63, 1, null);
                    break;
                case 675:
                    if (gameManager.tutorialManager.CheckAndHideTutorial(52))
                    {
                        gameManager.tutorialManager.CheckThenShowTutorial(53, .5f, null);
                        ResumeShowingMainHint();
                    }
                    else if (gameManager.tutorialManager.CheckThenShowTutorial(52, 1, null))
                        StopShowingMainHint();
                    break;
                case 725:
                    gameManager.tutorialManager.CheckThenShowTutorial(64, 1, null);
                    break;
                case 775:
                    gameManager.tutorialManager.CheckThenShowTutorial(65, 1, null);
                    break;
                case 850:
                    gameManager.tutorialManager.CheckThenShowTutorial(66, 1, null);
                    break;
                case 1150:
                    if (gameManager.tutorialManager.CheckAndHideTutorial(71))
                        ResumeShowingMainHint();
                    else if (gameManager.tutorialManager.CheckAndHideTutorial(70))
                    {
                        if (gameManager.tutorialManager.CheckThenShowTutorial(71, .5f, null))
                            ShowMoveHint(5, 2, Direction.Down);
                        // guidePresenter.SetShowingGuide(cellPresenters[4, 3], Dir.Up);
                    }
                    else
                    {
                        if (gameManager.tutorialManager.CheckThenShowTutorial(70, 1, null))
                        {
                            StopShowingMainHint();
                            //showAutoGuide = false;
                            ShowMoveHint(5, 1, Direction.Down);
                            //guidePresenter.SetShowingGuide(cellPresenters[3, 2], Dir.Right);
                        }
                    }
                    break;
                case 1400:
                    gameManager.tutorialManager.CheckThenShowTutorial(84, 1, null);
                    break;
                case 1825:
                    gameManager.tutorialManager.CheckThenShowTutorial(85, 1, null);
                    break;
                case 2100:
                    gameManager.tutorialManager.CheckThenShowTutorial(86, 1, null);
                    break;
                case 2800:
                    gameManager.tutorialManager.CheckThenShowTutorial(97, 1, () => gameManager.tutorialManager.CheckThenShowTutorial(98, .25f, null));
                    break;
                case 3100:
                    gameManager.tutorialManager.CheckThenShowTutorial(103, 1, null);
                    break;
                default:
                    if (levelIndex > 475 &&  gameManager.tutorialManager.IsTutorialShowed(83) == false && BoardHas<Padlock>(gameBoard) == true)
                        gameManager.tutorialManager.CheckThenShowTutorial(83, 1, null);
                    break;
            }
        }

        private bool BoardHas<T>(GameBoard gameBoard) where T : Tile
        {
            foreach (var cellStack in gameBoard.ArrbitrayCellStackArray())
                if (QueryUtilities.HasTileOnTop<T>(cellStack))
                    return true;
            return false;
        }

        private void StopShowingMainHint()
        {
            hintingSystem.SetManualHint(new EmptyHint());
        }

        private void ResumeShowingMainHint()
        {
            hintingSystem.SetManualHint(null);
        }

        private void ShowActivatingHint(int x, int y)
        {
            hintingSystem.SetManualHint(new ActivationHint(cellStackBoard[x, y]));
        }

        private void ShowMoveHint(int x, int y, Direction dirction)
        {
            var currentCellStack = cellStackBoard[x, y];
            var targetCellStack = cellStackBoard.DirectionalElementOf(new Vector2Int(x, y), dirction);
            hintingSystem.SetManualHint(new MoveHint(currentCellStack, targetCellStack));
            //hintingPresentationHandler.ShowHint();
        }
    }
}