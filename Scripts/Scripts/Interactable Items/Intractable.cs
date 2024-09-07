using System.Collections.Generic;
using Match3;
using Match3.Foundation.Base.ServiceLocating;
using UnityEngine;


[RequireComponent(typeof(Clickable))]
public abstract class Intractable : MonoBehaviour
{
    
    public int index = -1;
    public int defaultStateIndex = 0;
    public int CurrentStateIndex { protected set; get; } = 0;

    private string CurrentMapId => Base.gameManager.mapManager.CurrentMapId;
    
    
    protected virtual void Awake()
    {
        var clickable = GetComponent<Clickable>();
        clickable.onClickDown.AddListener(ClickDown);
        clickable.onClickUp.AddListener(ClickUp);
    }

    protected virtual void Start()
    {
        LoadState();
    }
    
    
    
    private void ClickDown()
    {
        InternalClickDown();
    }

    private void ClickUp()
    {
        InternalClickUp();
    }

    protected abstract void InternalClickDown();
    protected abstract void InternalClickUp();

    

    public void SaveState()
    {
        ServiceLocator.Find<UserProfileManager>().SaveData(GetStateKey(), CurrentStateIndex);
        InternalSaveState();
    }

    public void LoadState()
    {
        CurrentStateIndex = ServiceLocator.Find<UserProfileManager>().LoadData(GetStateKey(), defaultStateIndex);
        InternalLoadState();
    }

    protected abstract void InternalSaveState();
    protected abstract void InternalLoadState();
    
    
    private string GetStateKey()
    {
        return $"IntractableState_Index_{index}_{CurrentMapId}";
    }

    
    
    
    [ContextMenu("Set Intractables Index")]
    public void SetIndexes()
    {
        var allIntractables = FindObjectsOfType<Intractable>();
        List<int> indexBank = new List<int>(allIntractables.Length);
        List<Intractable> needIndexingIntractables = new List<Intractable>();

        for (int i = 0; i < allIntractables.Length; i++)
        {
            indexBank.Add(i);
        }

        for (int i = allIntractables.Length - 1; i >= 0; i--)
        {
            var tempIndex = allIntractables[i].index;
            
            if (tempIndex == -1 || IsDuplicateOrOutRangeIndex(tempIndex))
            {
                needIndexingIntractables.Add(allIntractables[i]);
            }
            else
            {
                indexBank.Remove(tempIndex);
            }
        }

        for (int j = 0; j < needIndexingIntractables.Count; j++)
        {
            needIndexingIntractables[j].index = indexBank[j];
        }

        indexBank.Clear();

        bool IsDuplicateOrOutRangeIndex(int index)
        {
            return !indexBank.Contains(index);
        }
    }
    
    
}
