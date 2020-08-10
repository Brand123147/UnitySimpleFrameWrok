using UnityEditor;
using UnityEngine;

public class AutoAssetBundleName 
{
    [MenuItem("Assets/Auto AssetBundle Name")]
    static void DoAutoAssetBundlesName()
    {
        if (Selection.objects.Length < 1)
        {
            Debug.LogError("请先选中一个预制体/场景...");
            return;
        }
        foreach (Object obj in Selection.objects)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError(string.Format("获取{0}路径失败",obj.name));
                return;
            }
            string assetBundleName = path.ToLower();
            AssetImporter assetImporter = AssetImporter.GetAtPath(path);
            if (assetImporter == null)
            {
                Debug.LogError("assetImporter = null, 自动命名assetbundle失败");
                return;
            }
            assetImporter.assetBundleName = assetBundleName;
            if (assetImporter.assetBundleName.Length <= 0)
            {
                Debug.LogError("自动命名assetbundle失败");
                return;
            }
            else
            {
                Debug.Log(string.Format("<color=yellow>{0}</color>", assetImporter.assetBundleName+"成功"));
            }
        }
    }
}
