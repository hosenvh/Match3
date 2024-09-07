using UnityEngine;
using System;
using System.Collections.Generic;
using Match3.CharacterManagement.Character.Base;

namespace Match3
{
    [Serializable]
    public class GameOverServerConfigData
    {
        [SerializeField] private bool isBoardPreviewActivate;

        public bool IsBoardPreviewActivate => isBoardPreviewActivate;
    }

    [Serializable]
    public class GameOverServerCohortConfig : CohortConfigReplacer<GameOverServerConfigData>
    {
    }
}