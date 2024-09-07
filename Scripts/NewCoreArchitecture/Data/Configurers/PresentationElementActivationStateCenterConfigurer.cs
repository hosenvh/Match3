using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Unity.Configuration;
using Match3.Presentation;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Data.Configuration
{
    [CreateAssetMenu(menuName = "Match3/Configurations/PresentationElementActivationStateCenterConfigurer")]
    public class PresentationElementActivationStateCenterConfigurer : ScriptableConfiguration, Configurer<PresentationElementActivationStateCenter>
    {
        [SerializeField] private List<PresentationElement> deactivedPresentationElements = default;

        public void Configure(PresentationElementActivationStateCenter entity)
        {
            foreach (var element in deactivedPresentationElements)
                entity.DeactiveElement(element);
        }

        public override void RegisterSelf(ConfigurationManager manager)
        {
            manager.Register(this);
        }
    }
}