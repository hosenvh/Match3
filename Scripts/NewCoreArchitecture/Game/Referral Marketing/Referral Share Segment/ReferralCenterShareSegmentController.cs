using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.ReferralMarketing.Segments;

namespace Match3.Game.ReferralMarketing
{
    
    public class ReferralCenterShareSegmentController: EventListener
    {

        private readonly ShareSegmentDataStorage dataStorage;
        private readonly ReferralCenter referralCenter;
        
        public ShareSegment[] ShareSegments { private set; get;}

        public ReferralCenterShareSegmentController(ReferralCenter referralCenter)
        {
            this.referralCenter = referralCenter;
            dataStorage = new ShareSegmentDataStorage();
            ServiceLocator.Find<EventManager>().Register(this);
        }

        public void SetShareSegments(ShareSegment[] segments)
        {
            ShareSegments = segments;
            foreach (var shareSegment in ShareSegments)
            {
                shareSegment.Initialize(dataStorage, referralCenter);
            }
        }
        
        public T GetSegment<T>() where T : ShareSegment
        {
            foreach (var segment in ShareSegments)
            {
                if (segment is T shareSegment)
                    return shareSegment;
            }

            return null;
        }

        public void OnEvent(GameEvent evt, object sender)
        {
            foreach (var shareSegment in ShareSegments)
            {
                shareSegment.UpdateInternalState(evt);
            }
        }
    }
    

}