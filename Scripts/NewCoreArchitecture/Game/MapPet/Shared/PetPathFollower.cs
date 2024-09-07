using System.Collections;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;
using UnityEngine.Serialization;


namespace Match3.Game.MapPet.Shared
{

    public class PetPathFollower : MonoBehaviour
    {

        // ------------------------------------------- Public Fields ------------------------------------------- \\ 
        
        [FormerlySerializedAs("catParentTransform")] public Transform parentTransform;

        public float MoveSpeed { get; private set; } = 0;

        // ------------------------------------------- Private Fields ------------------------------------------- \\
        
        private VertexPath path;

        private float traveledDistance = 0;


        // ====================================================================================================== \\
        
        
        public void SetPath(VertexPath path)
        {
            this.path = path;
        }

        public void SetSpeed(float speed)
        {
            MoveSpeed = speed;
        }

        public void MovePetToRandomPosition()
        {
            var pathPos = Random.Range(0, path.length);
            traveledDistance = pathPos;
            parentTransform.position = path.GetPointAtDistance(pathPos, EndOfPathInstruction.Loop);
            parentTransform.rotation = path.GetRotationAtDistance(pathPos, EndOfPathInstruction.Loop);
        }

        public void MovePetToPositionAtPath(float pathPos)
        {
            traveledDistance = pathPos;
            parentTransform.position = path.GetPointAtDistance(pathPos, EndOfPathInstruction.Loop);
            parentTransform.rotation = path.GetRotationAtDistance(pathPos, EndOfPathInstruction.Loop);
        }
        
        void Update()
        {
            if (path==null || parentTransform==null || MoveSpeed == 0) return;
            
            traveledDistance += MoveSpeed * Time.deltaTime;
            parentTransform.position = path.GetPointAtDistance(traveledDistance, EndOfPathInstruction.Loop);
            parentTransform.rotation = path.GetRotationAtDistance(traveledDistance, EndOfPathInstruction.Loop);
        }
        
    }

}


