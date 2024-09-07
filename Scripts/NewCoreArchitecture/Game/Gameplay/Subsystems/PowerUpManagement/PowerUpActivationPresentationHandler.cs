using Match3.Game.Gameplay.Core;
using System;
using System.Collections.Generic;

namespace Match3.Game.Gameplay.SubSystems.PowerUpManagement
{
    public interface PowerUpActivationPresentationHandler : PresentationHandler
    {
        void HandleHammer(CellStack target, Action onCompleted);

        void HandleBroom(List<CellStack> horizontals, List<CellStack> verticals, Action<CellStack> onCompletedOnStack, Action onCompleted);

        void HandleHand(CellStack origin, CellStack destination, Action onCompleted);
    }
}