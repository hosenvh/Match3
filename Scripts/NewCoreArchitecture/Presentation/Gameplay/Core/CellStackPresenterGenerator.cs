using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.SubSystems.General;
using Match3.Presentation.Gameplay.Core;
using UnityEngine;

namespace Match3.Presentation.Gameplay
{
    public class CellStackPresenterGenerator : MonoBehaviour, EventListener
    {
        BoardPresenterNew boardPresenter;


        public void Setup(BoardPresenterNew boardPresenter)
        {
            this.boardPresenter = boardPresenter;
            ServiceLocator.Find<EventManager>().Register(this);
        }

        void OnDestroy()
        {
            ServiceLocator.Find<EventManager>().UnRegister(this);
        }

        public void OnEvent(GameEvent evt, object sender)
        {

            switch (evt)
            {
                case CellGeneratedEvent cEvt:
                    boardPresenter.CreateCellPresenterFor(cEvt.cell, cEvt.cellStack);
                    break;
                case CellReorderedEvent cEvt:
                    cEvt.cellStack.GetComponent<CellStackPresenter>().UpdateCellOrderings();
                    break;
            }


        }
    }
}