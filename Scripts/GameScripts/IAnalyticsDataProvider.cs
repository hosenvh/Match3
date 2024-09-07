using Match3.Foundation.Base.MoneyHandling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAnalyticsProvider
{
    void SendAnalytics(AnalyticsDataBase analyticsDataBase);

}