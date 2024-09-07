using System;
using System.IO;
using Spine.Unity;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapItem_Element_LinkedWithSpine))]
public class MapItem_Element_LinkedWithSpine_Editor : Editor
{
    [SerializeField] private MapItem_Element_LinkedWithSpine myScript;

    private GUIStyle yellowLabelStyle;
    private bool stylesInited = false;
    private string localPath = "Not Exists!";
    
    
    
    void OnEnable()
    {
        myScript = (MapItem_Element_LinkedWithSpine) target;
        
        if(!myScript.linkedSpineName.IsNullOrEmpty() 
           && File.Exists($"{Application.dataPath}/Resources/{MapItem_Element_LinkedWithSpine.linkedSpineMainDirectory}{myScript.linkedSpineName}.prefab"))
            localPath = $"Assets/Resources/{MapItem_Element_LinkedWithSpine.linkedSpineMainDirectory}{myScript.linkedSpineName}.prefab";
        
    }
    

    void InitStyles()
    {
        stylesInited = true;
        yellowLabelStyle = new GUIStyle(GUI.skin.label);
        yellowLabelStyle.normal.textColor = Color.yellow;
    }


    void BoldSpine()
    {
        if (myScript.mySpineRect != null)
        {
            myScript.mySpineRect.GetComponent<SkeletonGraphic>().color = myScript.boldColor;
            myScript.isBold = true;
        }
    }

    void UnBoldSpine()
    {
        if (myScript.mySpineRect != null)
        {
            myScript.mySpineRect.GetComponent<SkeletonGraphic>().color = Color.white;
            myScript.isBold = false;
        }
    }
    
    public void OnSceneGUI()
    {
        if (Event.current.isKey && Event.current.type == EventType.KeyDown)
        {
            switch (Event.current.keyCode)
            {
                case KeyCode.A:
                    myScript.ChangeActivenessOfSpine();
                    myScript.GetLocalSpinePosition();
                    break;
                case KeyCode.W:
                    myScript.ScaleUp();
                    break;
                case KeyCode.S:
                    myScript.ScaleDown();
                    break;
                case KeyCode.D:
                    myScript.GetLocalSpinePosition();
                    break;
            }
        }
    }
    

    public override void OnInspectorGUI()
    {
        if(!stylesInited) InitStyles();
        
        serializedObject.Update();    
        
        
        myScript.showTouchGraphic = EditorGUILayout.Toggle("Need Touch Graphic Field", myScript.showTouchGraphic);
        if (myScript.showTouchGraphic)
        {
            myScript.touchGraphicGameObject = (GameObject) EditorGUILayout.ObjectField("Touch Graphic Object", myScript.touchGraphicGameObject, typeof(GameObject), true);
        }
        

        myScript.fadeAfterAnimationEnd = EditorGUILayout.Toggle("Fade After Animation End", myScript.fadeAfterAnimationEnd);
        myScript.deleteSpineAfterAnimationEnd = EditorGUILayout.Toggle("Delete Spine At End", myScript.deleteSpineAfterAnimationEnd);

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Editor Fields");
        myScript.mySpineRect = (RectTransform) EditorGUILayout.ObjectField("My Spine Rect", myScript.mySpineRect, typeof(RectTransform), true);

        if (myScript.mySpineRect != null)
        {
            if (GUI.changed && myScript.linkedSpineName!=myScript.mySpineRect.gameObject.name)
            {
                myScript.linkedSpineName = myScript.mySpineRect.gameObject.name;
                myScript.mySpineLocalPosition = myScript.mySpineRect.localPosition;
                localPath = "Not Exists!";
            }

            if(GUILayout.Button("Init Spine"))
                myScript.InitMySpineObject();
            if(GUILayout.Button("Get Spine Local Position"))
                myScript.GetLocalSpinePosition();
            if (GUILayout.Button("Make Spine Prefab"))
            {
                localPath = $"Assets/Resources/{MapItem_Element_LinkedWithSpine.linkedSpineMainDirectory}{myScript.linkedSpineName}.prefab";
                
//                if (PrefabUtility.IsPrefabAssetMissing(myScript.mySpineRect.gameObject) ||
//                    PrefabUtility.IsPartOfAnyPrefab(myScript.mySpineRect.gameObject))
//                {
//                    if(myScript.mySpineRect.parent==null)
//                        PrefabUtility.UnpackPrefabInstance(myScript.mySpineRect.gameObject,
//                            PrefabUnpackMode.Completely, InteractionMode.UserAction);
//                }

                var insObject = Instantiate(myScript.mySpineRect.gameObject, myScript.mySpineRect.transform.parent);
                DestroyImmediate(myScript.mySpineRect.gameObject);
                myScript.mySpineRect = insObject.GetRectTransform();
                PrefabUtility.SaveAsPrefabAssetAndConnect(insObject, localPath,
                    InteractionMode.UserAction, out bool success);
                
                
//                PrefabUtility.SaveAsPrefabAsset(myScript.mySpineRect.gameObject, localPath, out bool success);
                
                if(!success)
                    Debug.LogWarning("Spine Prefab Doesn't Saved");
            }
            if(GUILayout.Button("Set Spine To Saved Local Position"))
                myScript.SetSpineToCorrectPosition();
            
            
            EditorGUILayout.BeginHorizontal();
            if (!myScript.isBold)
            {
                if (GUILayout.Button("Bold Spine"))
                    BoldSpine();
            }
            else
            {
                if (GUILayout.Button("UnBold Spine"))
                    UnBoldSpine();
            }
            myScript.boldColor = EditorGUILayout.ColorField("Bold Color", myScript.boldColor);
            EditorGUILayout.EndHorizontal();
            
            
            EditorGUILayout.Space();
        }


        GUILayout.Label("My Spine Path:");
        GUILayout.Label((myScript.linkedSpineName.IsNullOrEmpty() ? "Is Null" : localPath), yellowLabelStyle);
        GUILayout.Label("My Spine Local Position:");
        GUILayout.Label($"X: {myScript.mySpineLocalPosition.x}    Y: {myScript.mySpineLocalPosition.y}    Z: {myScript.mySpineLocalPosition.z}", yellowLabelStyle);

        EditorGUILayout.Space();
        
        myScript.scaleAmount = EditorGUILayout.FloatField("Scale Amount", myScript.scaleAmount);
        
        EditorGUILayout.Space();

        
        GUILayout.Label("Key Instruction:");
        GUILayout.Label("W (Scale Up)     S (Scale Down)", yellowLabelStyle);
        GUILayout.Label("A (Change Activeness Of Spine And Get It's Local Position)", yellowLabelStyle);
        GUILayout.Label("D (Get Spine Local Position)", yellowLabelStyle);
        
        
        if(GUI.changed)
        {
            EditorUtility.SetDirty(myScript);
            EditorApplication.update.Invoke();
        }
    }

    
    //        EditorGUILayout.LabelField("My Spine Path", myScript.linkedSpineName.IsNullOrEmpty() ? "Is Null" : myScript.linkedSpineName, yellowLabelStyle);
}
