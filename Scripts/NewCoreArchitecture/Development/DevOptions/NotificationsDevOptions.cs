using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using Match3.Development.Base.DevelopmentConsole;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game;
using NotificationSpace;
using UnityEngine;
using static Base;
[DevOptionGroup(groupName: "Notification", priority: 34)]
public class NotificationsDevOptions : DevelopmentOptionsDefinition
{
   [DevOption(commandName: "Change Notification Time", color: DevOptionColor.Cyan, shouldAutoClose: true)]
   public static void ChangeNotificationTime(int level,int time)
   {
      var ScenarioNotification=ServiceLocator.Find<NotificationCentre>().GetNotificationBase<ScenarioNotificationItem>();

      ScenarioNotification.ChangeNotificationTime(level,time);
      GoToScenarioIndex(level);

   }
   public static void GoToScenarioIndex(int skipScenario)
   {
      gameManager.scenarioManager.ResetUserStateForGetReadyToSetScenario();
      gameManager.scenarioManager.SkipScenario(skipScenario+1);
   }
   
}
