using System;
using Match3.Foundation.Base.EventManagement;
using Match3.Utility.GolmoradLogging;
using UnityEngine;


namespace Match3.Overlay.Analytics
{
    public abstract class AnalyticsHandler
    {
        public void OnEvent(GameEvent evt)
        {
            try
            {
                Handle(evt);
            }
            catch (Exception e)
            {
                DebugPro.LogException<AnalyticsLogTag>(message: $"Error in {GetType().Name}:\n{e}");
            }
        }

        protected abstract void Handle(GameEvent evt);
    }
}