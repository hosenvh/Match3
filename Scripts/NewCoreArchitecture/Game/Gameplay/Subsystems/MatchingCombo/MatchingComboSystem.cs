

using Match3.Game.Gameplay.Input;

namespace Match3.Game.Gameplay.MatchingCombo
{

    public interface MatchingComboPresentationHandler : PresentationHandler
    {
        void HandleCombo(int comboCount);
    }

    // NOTE: There is no need for this in gameplay layer. This is mostly used
    // TODO: Try to define this in presentation layer.
    public class MatchingComboSystem : GameplaySystem
    {
        int currentCombo;

        MatchingComboPresentationHandler presentationHandler;


        public MatchingComboSystem(GameplayController gameplayController) : base(gameplayController)
        {
            presentationHandler = gameplayController.GetPresentationHandler<MatchingComboPresentationHandler>();
        }

        public override void Reset()
        {
            currentCombo = 0;
        }


        public override void Update(float dt)
        {
            if (GetFrameData<SuccessfulSwapsData>().data.Count > 0 || GetFrameData<SuccessfulUserActivationData>().targets.Count > 0)
                currentCombo = 0;

            foreach(var matches in GetFrameData<CreatedMatchesData>().data)
            {

                currentCombo++;
            }

            if(GetFrameData<CreatedMatchesData>().data.Count > 0)
                presentationHandler.HandleCombo(currentCombo);


        }
    }
}