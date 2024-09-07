using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class CloudSaveAuthorizationInputPanelController : MonoBehaviour
{

    [SerializeField] private GameObject inputPanelGameObject;
    [SerializeField] private TMP_InputField codeInputField = default;

    private Action<bool, string> onConfirm;
    
    public void Open(Action<bool, string> onConfirm)
    {
        inputPanelGameObject.SetActive(true);
        this.onConfirm = onConfirm;
    }

    public void ConfirmButton()
    {
        onConfirm.Invoke(true, codeInputField.text);
        inputPanelGameObject.SetActive(false);
    }

    public void Close()
    {
        onConfirm.Invoke(false, "");
        inputPanelGameObject.SetActive(false);
    }
    
}
