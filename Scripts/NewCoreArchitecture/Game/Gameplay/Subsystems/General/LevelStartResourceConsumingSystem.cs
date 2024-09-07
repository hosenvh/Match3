using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.Input;

namespace Match3.Game.Gameplay.SubSystems.General
{

    public class LevelStartResourceConsumingSystem : GameplaySystem
    {
        public bool IsLevelStartResourcesConsumed { get; private set; }
        private readonly LifeConsumer lifeConsumer;
        private readonly ConsumedBoostersState consumedBoostersState;

        public LevelStartResourceConsumingSystem(GameplayController gameplayController, LifeConsumer lifeConsumer) : base(gameplayController)
        {
            this.lifeConsumer = lifeConsumer;
            this.consumedBoostersState = new ConsumedBoostersState();
        }

        public override void Update(float dt)
        {
            if(GetFrameData<SuccessfulSwapsData>().data.Count > 0 || GetFrameData<SuccessfulUserActivationData>().targets.Count > 0)
            {
                consumedBoostersState.ResetConsumableBoostersStateToNotChosen();
                gameplayController.DeactivateSystem<LevelStartResourceConsumingSystem>();
                lifeConsumer.ConsumeLife();
                IsLevelStartResourcesConsumed = true;
                for (int i = Base.gameManager.profiler.isBoosterSelected.Length - 1; i >= 0; i--)
                {
                    if (Base.gameManager.profiler.isBoosterSelected[i])
                    {
                        consumedBoostersState.SetConsumableBoostersStateToChosen(i);
                        Base.gameManager.profiler.isBoosterSelected[i] = false;
                        Base.gameManager.profiler.BoosterManager.ConsumeBooster(i);
                    }
                    
                    if (Base.gameManager.profiler.BoosterManager.IsInfiniteBoosterAvailable(i))
                        consumedBoostersState.SetConsumableBoostersStateToChosen(i);
                }
                ServiceLocator.Find<EventManager>().Propagate(new LevelStartResourceConsumingEvent(), this);
                ServiceLocator.Find<EventManager>().Propagate(new LevelStartedWithBoosters(consumedBoostersState), this);
                consumedBoostersState.ResetConsumableBoostersStateToNotChosen();
            }
        }
    }
}