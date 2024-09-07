using Match3.Data.Map;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Match3.EditorTools.Editor.Menus.Optimization
{
    public partial class GolmoradAssetOptimizationUtilityWindow
    {
        public class UnusedMapItemElementFinder
        {
            public List<MapItem_Element> Find(MapsMetaDatabase mapsMetaDatabase, DisplayProgress displayProgressAction)
            {
                var allMapItemElements = FindAllUsedMapItemElements();
                var usedMapElements = ExtractUsedMapElements(mapsMetaDatabase);

                allMapItemElements.ExceptWith(usedMapElements);
                return allMapItemElements.ToList();
            }

            private HashSet<MapItem_Element> FindAllUsedMapItemElements()
            {
                var allMapItemElements = new HashSet<MapItem_Element>();
                var prefabsGuids = AssetDatabase.FindAssets($"t:Prefab");

                foreach(var guid in prefabsGuids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    if (AssetEditorUtilities.IsInResourcesFolder(path))
                    {
                        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                        var element = prefab.GetComponent<MapItem_Element>();

                        if (element != null)
                            allMapItemElements.Add(element);
                    }
                }

                return allMapItemElements;
            }

            private HashSet<MapItem_Element> ExtractUsedMapElements(MapsMetaDatabase mapsMetaDatabase)
            {
                var usedMapElements = new HashSet<MapItem_Element>();

                foreach (var metaData in mapsMetaDatabase.MapsMetadata)
                {
                    var map = Resources.Load<State_Map>(metaData.mapResourcesPath);

                    var singleUserSelectItems = map.GetComponentsInChildren<MapItem_UserSelect_Single>();
                    var singleStateItems = map.GetComponentsInChildren<MapItem_State_Single>();

                    foreach (var userSelect in singleUserSelectItems)
                        foreach (var path in userSelect.mapItem_ElementsPaths)
                            usedMapElements.Add(Resources.Load<MapItem_Element>(path));


                    foreach (var state in singleStateItems)
                        foreach (var path in state.mapItem_ElementsPaths)
                            usedMapElements.Add(Resources.Load<MapItem_Element>(path));
                }

                return usedMapElements;
            }
        }
    }
}