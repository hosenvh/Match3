using UnityEngine;


[System.Serializable]
public struct Period
{
   public int startDay;
   public int endDay;
}

public class MapItem_Periodic : MonoBehaviour
{

   // ---------------------------------------------------- Public Fields ---------------------------------------------------- \\ 
   
   public Transform itemPositionHolder;
   
   [Space(10)]
   public Period[] activePeriods;
   
   [Space(10)]
   public string itemPath;
   public GameObject myItem;

   
   // ======================================================================================================================= \\
   
   
   private void Start()
   {
      if(myItem!=null) myItem.SetActive(false);
      CheckToSpawnItem();
   }


   private void CheckToSpawnItem()
   {
      var currentDay = Base.gameManager.taskManager.CurrentDay;
      for (var i = activePeriods.Length - 1; i >= 0; --i)
      {
         if (currentDay >= activePeriods[i].startDay && currentDay <= activePeriods[i].endDay)
         {
            if(myItem==null)
               myItem = Instantiate(Resources.Load<GameObject>(itemPath), itemPositionHolder, false);
            else
               myItem.SetActive(true);
            break;
         }
      }
   }
   
   
}
