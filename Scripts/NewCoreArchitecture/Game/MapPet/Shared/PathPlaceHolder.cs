using PathCreation;
using UnityEngine;


namespace Match3.Game.MapPet.Shared
{
    [ExecuteInEditMode]
    public class PathPlaceHolder : MonoBehaviour
    {
        [SerializeField] private PathCreator pathCreator = default;
        [SerializeField] [Range(0, 1)] private float position = default;


        private void OnEnable()
        {
            transform.position = pathCreator.path.GetPointAtDistance(position, EndOfPathInstruction.Stop);
        }

        private void OnValidate()
        {
            transform.position =
                pathCreator.path.GetPointAtDistance(position * pathCreator.path.length, EndOfPathInstruction.Stop);
        }
        
        
        public float GetPositionOnPath()
        {
            return position * pathCreator.path.length;
        }
        
    }
}
