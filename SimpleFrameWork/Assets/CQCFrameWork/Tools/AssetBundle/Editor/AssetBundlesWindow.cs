using UnityEditor;
using System.IO;
using UnityEngine;

public class AssetBundlesWindow
{
    [MenuItem("Tools/BuildAssetBundle")]
    static void NewMenuOption()
    {
        var wnd = EditorWindow.GetWindow<BuildAssetBundle>("打AssetsBundle包");
        wnd.minSize = new Vector2(300, 600);
        wnd.maxSize = new Vector2(510, 600);
    }
}
