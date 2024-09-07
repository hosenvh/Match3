using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystems.LevelContinuing;
using Match3.Presentation.TextAdapting;
using PandasCanPlay.HexaWord.Utility;
using SeganX;
using UnityEngine;

namespace Match3.Presentation.Gameplay
{
    public class LevelContinuingDataPreseneter : MonoBehaviour
    {
        [Serializable]
        public struct BoosterPresentationData
        {
            [Type(typeof(Tile), includeAbstracts:false)]
            public string booster;
            public GameObject targetObject;
        }

        public LocalText resumeCostText;
        public TextAdapter additionalMovesText;
        public GameObject boostersContainer;

        public List<BoosterPresentationData> boosterPresentationsData;

        public void Setup(LevelContinuingStage continuingData)
        {
            resumeCostText.SetText(continuingData.neededCoins.ToString());
            additionalMovesText.SetText(string.Format("+{0}",continuingData.additionalMoves));

            boostersContainer.SetActive(continuingData.tilesToCreate.Count > 0);


            foreach (var data in boosterPresentationsData)
                data.targetObject.SetActive(false);

            foreach (var type in continuingData.tilesToCreate)
                FindDataFor(type).targetObject.SetActive(true);

        }

        private BoosterPresentationData FindDataFor(Type type)
        {
            return boosterPresentationsData.Find(d => type.ToString().Equals(d.booster));
        }
    }
}