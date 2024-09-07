using System.Collections.Generic;
using Match3.Game.MapPet.Cat;
using UnityEngine;

namespace Match3.Game.MapPet.Shared
{
    public class PetBehaviourController : MonoBehaviour
    {

        // ------------------------------------------- Public Fields ------------------------------------------- \\ 

        public PetPathFollower pathFollower;
        public Animator animator;

        [Space(10)] public PetPath[] catPaths;

        [Space(10)] public float changeSpeedDurationTime;

        [Space(10)] public List<PetBehaviour> startBehaviours;


        // ------------------------------------------- Private Fields ------------------------------------------- \\

        protected PetBehaviour currentBehaviour;

        private static readonly int MoveParameter = Animator.StringToHash("Move");

        private float currentAnimatorMoveParameterValue = 0;
        private float targetAnimatorMoveParameterValue = 0;
        private float deltaAnimatorMoveBlendValue = 0;

        private float deltaSpeedChanging = 0;
        private float targetMoveSpeed = 0;

        private float changeMoodTime = 0;

        private bool moveSpeedChanged = false;


        // ====================================================================================================== \\


        protected virtual void Start()
        {
            pathFollower.SetPath(catPaths.RandomOne(catPaths[0]).Path);
            pathFollower.MovePetToRandomPosition();
            SetBehaviour(startBehaviours.RandomOne());
        }

        void Update()
        {
            if (moveSpeedChanged)
            {
                currentAnimatorMoveParameterValue = Mathf.MoveTowards(currentAnimatorMoveParameterValue,
                    targetAnimatorMoveParameterValue,
                    (Time.deltaTime / changeSpeedDurationTime) * deltaAnimatorMoveBlendValue);
                animator.SetFloat(MoveParameter, currentAnimatorMoveParameterValue);

                pathFollower.SetSpeed(Mathf.MoveTowards(pathFollower.MoveSpeed, targetMoveSpeed,
                    (Time.deltaTime / changeSpeedDurationTime) * deltaSpeedChanging));

                if (currentAnimatorMoveParameterValue == targetAnimatorMoveParameterValue)
                    moveSpeedChanged = false;
            }

            if (changeMoodTime > 0)
            {
                changeMoodTime -= Time.deltaTime;
                if (changeMoodTime <= 0)
                {
                    SetBehaviour(GetNextBehaviour(currentBehaviour));
                }
            }
        }


        // ----------------------------------- Set Behaviour ----------------------------------- \\


        public virtual void SetBehaviour(PetBehaviour behaviour)
        {
            currentBehaviour = behaviour;

            if (!string.IsNullOrEmpty(behaviour.animatorTriggerParameter) &&
                behaviour.animatorTriggerParameter != "None")
                animator.SetTrigger(behaviour.animatorTriggerParameter);

            changeMoodTime = behaviour.actTime.GetRandomInRange();
            ChangeSpeed(behaviour.moveSpeed, behaviour.animatorMoveBlendValue);
        }


        protected virtual PetBehaviour GetNextBehaviour(PetBehaviour behaviour)
        {
            return behaviour.availableBehaviours.RandomOne();
        }


        // ------------------------------------- Speed Operation ------------------------------------- \\

        private void ChangeSpeed(float targetMoveSpeed, float targetAnimatorBlendValue)
        {
            if (currentAnimatorMoveParameterValue == targetMoveSpeed) return;

            this.targetMoveSpeed = targetMoveSpeed;
            targetAnimatorMoveParameterValue = targetAnimatorBlendValue;

            deltaAnimatorMoveBlendValue = Mathf.Abs(animator.GetFloat(MoveParameter) - targetAnimatorBlendValue);
            deltaSpeedChanging = Mathf.Abs(pathFollower.MoveSpeed - targetMoveSpeed);

            moveSpeedChanged = true;
        }
    }
}