using Match3.Foundation.Base.ServiceLocating;
using System.Collections.Generic;


namespace Match3.Presentation
{
    public enum PresentationElement
    {
        News,
        Settings_MoreGames,
        Settings_Gift,
        Settings_Support,
        Settings_Credit,
        LanguageSelection,
        Settings_Merch
    }

    // NOTE: This is a very crude system. It should be reevaluated, redesigned and improved by time.
    public class PresentationElementActivationStateCenter : Service
    {

        HashSet<PresentationElement> deactiveElements = new HashSet<PresentationElement>();

        public void DeactiveElement(PresentationElement element)
        {
            deactiveElements.Add(element);
        }

        public bool IsActive(PresentationElement element)
        {
            return deactiveElements.Contains(element) == false;
        }
    }
}