using Match3.Foundation.Base.ServiceLocating;
using UnityEngine;


namespace Match3.Presentation
{
    public class PresentationElementActivationHandler : MonoBehaviour
    {
        [SerializeField] private PresentationElement element;

        PresentationElementActivationStateCenter activationStateCenter;

        void Start()
        {
            activationStateCenter = ServiceLocator.Find<PresentationElementActivationStateCenter>();
            Evaluate();
        }

        // TODO: Find a better name
        public void Evaluate()
        {
            if(activationStateCenter == null)
                activationStateCenter = ServiceLocator.Find<PresentationElementActivationStateCenter>();

            this.gameObject.SetActive(activationStateCenter.IsActive(element));
        }
    }
}