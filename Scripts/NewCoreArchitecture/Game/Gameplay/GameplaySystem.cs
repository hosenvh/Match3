using Match3.Foundation.Base.ComponentSystem;
using System;

namespace Match3.Game.Gameplay
{
    // NOTE: This is currently use for documentation only.
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true)]
    public class AfterAttribute : System.Attribute
    {
        public AfterAttribute(Type type)
        {

        }
    }

    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true)]
    public class BeforeAttribute : System.Attribute
    {
        public BeforeAttribute(Type type)
        {

        }
    }

   

    public interface IGameplaySystem
    {
        void Start();

        void OnActivated();

        void OnDeactivated();

        void Update(float dt);

        T GetFrameData<T>() where T : BlackBoardData;


        T GetSessionData<T>() where T : BlackBoardData;

    }

    public abstract class GameplaySystem : IGameplaySystem
    {
        protected GameplayController gameplayController;

        protected GameplaySystem(GameplayController gameplayController)
        {
            this.gameplayController = gameplayController;
        }

        public virtual void Start() { }

        public virtual void OnActivated(){ }

        public virtual void OnDeactivated() { }

        public virtual void Reset() { }


        public abstract void Update(float dt);


        public T GetFrameData<T>() where T : BlackBoardData
        {
            return gameplayController.FrameCenteredBlackBoard().GetComponent<T>();
        }

        public T GetSessionData<T>() where T : BlackBoardData
        {
            return gameplayController.SessionCenteredBlackBoard().GetComponent<T>();
        }

    }
}