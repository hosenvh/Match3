using System.Collections.Generic;
using DynamicSpecialOfferSpace;


namespace Match3
{

    public class ServerConfigDataValidator
    {
        public bool IsDynamicSpecialOfferConfigValid(DynamicSpecialOfferConfig dynamicSpecialOfferConfig)
        {
            return dynamicSpecialOfferConfig != null && dynamicSpecialOfferConfig.offerShowTime > 0;
        }
    }
    
}


