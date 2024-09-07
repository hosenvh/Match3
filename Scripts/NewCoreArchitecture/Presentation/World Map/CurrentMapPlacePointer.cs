using System;
using DG.Tweening;
using UnityEngine;


namespace Match3.Presentation.WorldMap
{
    public class CurrentMapPlacePointer : MonoBehaviour
    {

        private bool isMoving;
        private WorldMapPlace currentPlace;


        public void Setup(WorldMapPlace place)
        {
            currentPlace = place;
            transform.position = place.transform.position;
        }
        
        public bool CanMove()
        {
            return !isMoving;
        }
        
        public void MoveToPlace(WorldMapPlace place, Action onReached)
        {
            if(isMoving) return;
            
            isMoving = true;
            currentPlace = place;
            transform.DOMove(place.transform.position, 1.3f, true).SetEase(Ease.OutQuart).OnComplete(() =>
            {
                isMoving = false;
                onReached();
            });
        }
        
    }    
}


