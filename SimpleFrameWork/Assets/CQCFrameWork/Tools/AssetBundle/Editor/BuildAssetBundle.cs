using CQCFrameWork.AssetBundlesMgr;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildAssetBundle : EditorWindow
{
    /// <summary>
    /// 默认输出路径
    /// </summary>
    const string _DefaultOutPutPath = "Assets/StreamingAssets/AssetBundles";

    /// <summary>
    /// 输出路径
    /// </summary>
    string _outPutPath = null;

    /// <summary>
    /// 打包模式编号
    /// </summary>
    int modeIndex = 0;

    /// <summary>
    /// 平台编号
    /// </summary>
    int platform = 0;

    /// <summary>
    /// 是否清除原有的assetbundle文件
    /// </summary>
    static bool isClearAssetBundleFolders = true;

    /// <summary>
    /// 打包模式
    /// </summary>
    BuildAssetBundleOptions babo;

    /// <summary>
    /// 目标平台
    /// </summary>
    BuildTarget buildTarget;

    /// <summary>
    /// 亲测这个用字符串数组不好使   unity2018
    /// </summary>
    GUIContent[] _BuildAssetBundleOptions =
    {
       new GUIContent("None(LZMA)"),
       new GUIContent("UncompressedAssetBundle(不压缩)"),
       new GUIContent("ChunkBasedCompression(LZ4)")
    };
    GUIContent[] _BuildTarget =
    {
       new GUIContent("NoTarget = -2"),
       new GUIContent("StandaloneOSX = 2"),
       new GUIContent("StandaloneWindows = 5"),
       new GUIContent("iOS = 9"),
       new GUIContent("Android = 13"),
       new GUIContent("StandaloneWindows64 = 19"),
       new GUIContent("PS4 = 31"),
       new GUIContent("XboxOne = 33"),
       new GUIContent("Switch = 38"),
    };

    void OnGUI()
    {
        //===================================================================================================================================
        //设置输出路径
        EditorGUILayout.Space();
        GUILayout.BeginHorizontal();
        var newPath = EditorGUILayout.TextField("Output Path", _outPutPath);
        if (!string.IsNullOrEmpty(newPath) && newPath != _outPutPath)
        {
            _outPutPath = newPath;
        }
        if (GUILayout.Button("Browse", GUILayout.MaxWidth(75f)))
        {
            BrowseForFolder();
        }

        if (GUILayout.Button("Reset", GUILayout.MaxWidth(75f)))
        {
            _outPutPath = _DefaultOutPutPath;
        }
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();
        //===================================================================================================================================
        //是否清理原来的assetbundle文件
        isClearAssetBundleFolders = EditorGUILayout.Toggle("Clear AssetBundle Folders", isClearAssetBundleFolders);
        //===================================================================================================================================
        //设置打包模式
        modeIndex = EditorGUILayout.Popup(new GUIContent("打包模式"), modeIndex, _BuildAssetBundleOptions);
        switch (modeIndex)
        {
            case 0:
                babo = BuildAssetBundleOptions.None;
                break;
            case 1:
                babo = BuildAssetBundleOptions.UncompressedAssetBundle;
                break;
            case 2:
                babo = BuildAssetBundleOptions.ChunkBasedCompression;
                break;
        }
        //===================================================================================================================================
        //打包平台
        platform = EditorGUILayout.Popup(new GUIContent("打包平台"), platform, _BuildTarget);
        switch (platform)
        {
            case 0:
                buildTarget = BuildTarget.NoTarget;
                break;
            case 1:
                buildTarget = BuildTarget.StandaloneOSX;
                break;
            case 2:
                buildTarget = BuildTarget.StandaloneWindows;
                break;
            case 3:
                buildTarget = BuildTarget.iOS;
                break;
            case 4:
                buildTarget = BuildTarget.Android;
                break;
            case 5:
                buildTarget = BuildTarget.StandaloneWindows64;
                break;
            case 6:
                buildTarget = BuildTarget.PS4;
                break;
            case 7:
                buildTarget = BuildTarget.XboxOne;
                break;
            case 8:
                buildTarget = BuildTarget.Switch;
                break;
        }
        //===================================================================================================================================
        //打包按钮
        if (GUILayout.Button("Build"))
        {
            BuildAllAssetBundles();
        }
    }

    void BuildAllAssetBundles()
    {
        if (Directory.Exists(_outPutPath) == true && isClearAssetBundleFolders)
        {
            Directory.Delete(_outPutPath, true);
        }
        if (Directory.Exists(_outPutPath) == false)
        {
            Directory.CreateDirectory(_outPutPath);
        }
        BuildPipeline.BuildAssetBundles(_outPutPath, babo, buildTarget);
    }
    /// <summary>
    /// 设置输出路径
    /// </summary>
    private void BrowseForFolder()
    {
        var newPath = EditorUtility.OpenFolderPanel("Bundle Folder", string.Empty, string.Empty);
        if (!string.IsNullOrEmpty(newPath))
        {
            var gamePath = System.IO.Path.GetFullPath(".");
            gamePath = gamePath.Replace("\\", "/");
            if (newPath.StartsWith(gamePath) && newPath.Length > gamePath.Length)
                newPath = newPath.Remove(0, gamePath.Length + 1);
            _outPutPath = newPath;
        }
    }

    //=====================================设置Editor环境下asset bundle模拟模式==========================
    const string kSimulationMode = "Tools/Simulation Mode";
    [MenuItem(kSimulationMode, false, 1)]
    public static void ToggleSimulationMode()
    {
        AssetBundleMgr.SimulateAssetBundleInEditor = !AssetBundleMgr.SimulateAssetBundleInEditor;
    }
    [MenuItem(kSimulationMode, true)]
    public static bool ToggleSimulationModeValidate()
    {
        Menu.SetChecked(kSimulationMode, AssetBundleMgr.SimulateAssetBundleInEditor);
        return true;
    }
}
