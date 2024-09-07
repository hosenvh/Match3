using System;
using System.Collections.Generic;
using Match3.Game.Gameplay.SubSystems.LevelEnding;
using UnityEngine;
using UnityEngine.UI;


namespace Match3.Presentation.Gameplay
{

    public class LevelDifficultyUIController : MonoBehaviour
    {
        [SerializeField] private Image levelGoalPanelImage;
        [SerializeField] private Image powerUpsPanelImage;
        [SerializeField] private LevelUIPack challengeLevelUiPack;
        [SerializeField] private List<LevelUIPack> levelUiPacks;
        
        
        public void Setup(BoardConfig boardConfig)
        {
            var difficultyLevel = boardConfig.levelConfig.difficultyLevel;
            var isChallengeLevelActive = boardConfig.IsChallengeLevelActive();
            
            if (isChallengeLevelActive)
            {
                levelGoalPanelImage.sprite = challengeLevelUiPack.levelGoalPanelSprite;
                powerUpsPanelImage.sprite = challengeLevelUiPack.powerUpPanelSprite;
            }
            else
            {
                foreach (var levelUiPack in levelUiPacks)
                {
                    if (levelUiPack.difficultyLevel == difficultyLevel)
                    {
                        levelGoalPanelImage.sprite = levelUiPack.levelGoalPanelSprite;
                        powerUpsPanelImage.sprite = levelUiPack.powerUpPanelSprite;
                        break;
                    }
                }
            }
        }
        
        
        [Serializable]
        public class LevelUIPack
        {
            public DifficultyLevel difficultyLevel;
            public Sprite levelGoalPanelSprite;
            public Sprite powerUpPanelSprite;
        }
        
        
    }

}