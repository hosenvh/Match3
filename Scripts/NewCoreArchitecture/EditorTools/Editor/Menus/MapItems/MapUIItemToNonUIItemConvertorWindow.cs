using UnityEditor;
using UnityEngine;
using Match3.EditorTools.Editor.Menus.MapItems.Base;


namespace Match3.EditorTools.Editor.Menus.MapItems
{
    public class MapUIItemToNonUIItemConvertorWindow : EditorWindow
    {
        [MenuItem("Golmorad/Map Items/Map UI Item To Non UI Item Convertor")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(MapUIItemToNonUIItemConvertorWindow));
        }


        float globalScaleUp = 1;

        PureImageToSpriteRendererConvertor pureImageToSpriteRendererConvertor = new PureImageToSpriteRendererConvertor();
        MapImageElementToMapSpriteElementConvertor mapImageElementToMapSpriteElementConvertor = new MapImageElementToMapSpriteElementConvertor();
        MapObjectElementImageToSpriteConvertor mapObjectElementWithImageToSpriteConvertor = new MapObjectElementImageToSpriteConvertor();

        SkeletonGraphicMapItemToSkeletonAnimationConvertor skeletonGraphicMapItemToSkeletonAnimationConvertor = new SkeletonGraphicMapItemToSkeletonAnimationConvertor();
        MapObjectElementSkeletonGraphicToSkeletonAnimationConvertor mapObjectElementSkeletonGraphicToSkeletonAnimationConvertor = new MapObjectElementSkeletonGraphicToSkeletonAnimationConvertor();

        WrongImageAspectRatioLogger wrongImageAspectRatioLogger = new WrongImageAspectRatioLogger();
        WrongUserSelectPivotLogger wrongUserSelectPivotLogger = new WrongUserSelectPivotLogger();
        WrongStatePivotLogger wrongStatePivotLogger = new WrongStatePivotLogger();

        void OnGUI()
        {
            globalScaleUp = EditorGUILayout.FloatField(nameof(globalScaleUp), globalScaleUp);

            pureImageToSpriteRendererConvertor.SetGlobalScaleUp(globalScaleUp);
            mapImageElementToMapSpriteElementConvertor.SetGlobalScaleUp(globalScaleUp);
            mapObjectElementWithImageToSpriteConvertor.SetGlobalScaleUp(globalScaleUp);
            skeletonGraphicMapItemToSkeletonAnimationConvertor.SetGlobalScaleUp(globalScaleUp);
            mapObjectElementSkeletonGraphicToSkeletonAnimationConvertor.SetGlobalScaleUp(globalScaleUp);


            if (GUILayout.Button("Convert Image Map Elements"))
                mapImageElementToMapSpriteElementConvertor.ApplyOn(Selection.gameObjects);
            if (GUILayout.Button("Convert Object Map Elements with Image"))
                mapObjectElementWithImageToSpriteConvertor.ApplyOn(Selection.gameObjects);
            if (GUILayout.Button("Convert Pure Images"))
                pureImageToSpriteRendererConvertor.ApplyOn(Selection.gameObjects);
            GUILayout.Space(20);

            if (GUILayout.Button("Convert Skeleton Graphic Map Elements"))
                skeletonGraphicMapItemToSkeletonAnimationConvertor.ApplyOn(Selection.gameObjects);
            if (GUILayout.Button("Convert Object Map Elements with Skeleton Graphic"))
                mapObjectElementSkeletonGraphicToSkeletonAnimationConvertor.ApplyOn(Selection.gameObjects);
            GUILayout.Space(20);

            if (GUILayout.Button("Log possible wrong Image Aspect Ratio"))
                wrongImageAspectRatioLogger.ApplyOn(Selection.gameObjects);
            if (GUILayout.Button("Log possible wrong MapItem Pivot"))
            {
                wrongUserSelectPivotLogger.ApplyOn(Selection.gameObjects);
                wrongStatePivotLogger.ApplyOn(Selection.gameObjects);
            }
        }
    }
}