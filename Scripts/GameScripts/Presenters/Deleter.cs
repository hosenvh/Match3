using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deleter : MonoBehaviour
{
    public GameObject[] deleteGameObject;

    private void Awake()
    {
        foreach (var item in deleteGameObject)
        {
            if (item)
            {
                item.SetActive(false);
                Destroy(item.gameObject);
            }
        }
    }
}
