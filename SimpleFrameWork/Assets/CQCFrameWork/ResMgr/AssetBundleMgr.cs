/* 
 * Assetbundle资源（预制体、场景）加载/卸载
 */
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using CQCFrameWork.Config;
using System;

namespace CQCFrameWork.AssetBundlesMgr
{
    public class AssetBundleMgr : Singleton<AssetBundleMgr>
    {
        /// <summary>
        /// 模拟assetbundle模式
        /// </summary>
#if UNITY_EDITOR
        static int m_SimulateAssetBundleInEditor = -1;
        const string kSimulateAssetBundles = "SimulateAssetBundles";
#endif

        /// <summary>
        /// 所有已经加载的Assetbundle
        /// </summary>
        public List<AssetBundle> loadedBundles = new List<AssetBundle>();
        public List<AssetBundle> loadedImages = new List<AssetBundle>();

        UnityWebRequest mRequest = null;

#if UNITY_EDITOR
        /// <summary>
        /// 设置模拟模式
        /// </summary>
        public static bool SimulateAssetBundleInEditor
        {
            get
            {
                if (m_SimulateAssetBundleInEditor == -1)
                    m_SimulateAssetBundleInEditor = EditorPrefs.GetBool(kSimulateAssetBundles, true) ? 1 : 0;

                return m_SimulateAssetBundleInEditor != 0;
            }
            set
            {
                int newValue = value ? 1 : 0;
                if (newValue != m_SimulateAssetBundleInEditor)
                {
                    m_SimulateAssetBundleInEditor = newValue;
                    EditorPrefs.SetBool(kSimulateAssetBundles, value);
                }
            }
        }
#endif


        #region ====================================================加载预制体==============================================================
        /// <summary>
        /// assetbundle异步加载资源
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="prefabName">预制体名字</param>
        /// <param name="path">要加载的对象的路径</param>
        /// <param name="callback">回调函数</param>
        public void ABLoadAsync<T>(PrefabName prefabName, string path = null, UnityAction<T> callback = null) where T : UnityEngine.Object
        {
            MonoMgr.Instance.StartCoroutine(ReallyABLoadAsync(prefabName, path, callback));
        }

        private IEnumerator ReallyABLoadAsync<T>(PrefabName prefabName, string path = null, UnityAction<T> callback = null) where T : UnityEngine.Object
        {
            //从表格里面取名字
            PrefabCfgItem itemCfg = PrefabCfgCtrl.Instance.Get((int)prefabName);
            string bundleName = itemCfg.AssetBundleName;
            string assetName = itemCfg.AssetName;
#if UNITY_EDITOR
            if (SimulateAssetBundleInEditor)
            {
                // 去除后缀
                int endIndex = assetName.LastIndexOf(".");
                string assetNameNoSuffix = endIndex > 0 ? assetName.Substring(0, endIndex) : assetName;
                // 用bundleName和预制体名获取预制体所在的路径
                string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(bundleName, assetNameNoSuffix);
                if (assetPaths.Length == 0)
                {
                    LogMgr.LogMgr.LogError("在编辑器模式没有“" + assetName + "”这个预制体与bundle：" + bundleName + "对应");
                    yield break;
                }
                UnityEngine.Object obj = AssetDatabase.LoadMainAssetAtPath(assetPaths[0]);
                if (obj == null)
                {
                    LogMgr.LogMgr.LogError("在编辑器模式找不到这个对象" + obj.name);
                    yield break;
                }
                if (obj is GameObject)
                {
                    callback(UnityEngine.Object.Instantiate(obj) as T);
                }
                else
                {
                    callback(obj as T);
                }
            }
            else
#endif
            {
                if (path == null)
                {
                    path = Application.streamingAssetsPath + @"/AssetBundles/" + bundleName;
                }
                mRequest = UnityWebRequestAssetBundle.GetAssetBundle(path);
                yield return mRequest.SendWebRequest();
                if (mRequest.isNetworkError || mRequest.isHttpError)
                {
                    LogMgr.LogMgr.Log(mRequest.error);
                }
                AssetBundle ab = DownloadHandlerAssetBundle.GetContent(mRequest);
                if (ab == null)
                {
                    LogMgr.LogMgr.LogError(string.Format("Dont get assetbundle {0}!!!", bundleName));
                    yield break;
                }
                var obj = ab.LoadAssetAsync<T>(bundleName);
                yield return obj;
                if (obj.isDone)
                {
                    if (obj.asset == null)
                    {
                        LogMgr.LogMgr.LogError("LoadAssetAsync failed obj is null");
                        yield break;
                    }
                    if (obj.asset is GameObject)
                    {
                        callback(UnityEngine.Object.Instantiate(obj.asset) as T);
                    }
                    else
                    {
                        callback(obj.asset as T);
                    }
                }
                loadedBundles.Add(ab);
            }
        }


        /// <summary>
        /// 卸载assetbundle,在模拟模式下没有走assetbundle
        /// </summary>
        public void UnLoadAB(PrefabName prefabName)
        {
            Resources.UnloadUnusedAssets();
            if (SimulateAssetBundleInEditor)
            {
                return;
            }
            PrefabCfgItem itemCfg = PrefabCfgCtrl.Instance.Get((int)prefabName);
            string name = itemCfg.AssetBundleName;
            bool isSuccess = false;
            if (mRequest != null)
            {
                mRequest.Dispose();
                mRequest = null;
            }
            foreach (var item in loadedBundles)
            {
                if (name == item.name)
                {
                    item.Unload(false);
                    loadedBundles.Remove(item);
                    isSuccess = true;
                    return;
                }
            }
            if (isSuccess == false)
            {
                LogMgr.LogMgr.LogError("已加载的assetbundle列表未找到：" + name + "  卸载失败");
            }
        }

        #endregion  ====================================================加载预制体===========================================================

        #region ====================================================加载场景==============================================================
        public void LoadSceneAsync(PrefabName sceneName, bool isAdditive = false)
        {
            MonoMgr.Instance.StartCoroutine(ReallySceneABLoadAsync(sceneName, isAdditive));
        }
        private IEnumerator ReallySceneABLoadAsync(PrefabName sceneName, bool additive = true)
        {
            //从表格里面取名字
            PrefabCfgItem itemCfg = PrefabCfgCtrl.Instance.Get((int)sceneName);
            string bundleName = itemCfg.AssetBundleName;
            string assetName = itemCfg.AssetName;
#if UNITY_EDITOR
            if (SimulateAssetBundleInEditor)
            {
                // 去除后缀
                int endIndex = assetName.LastIndexOf(".");
                string assetNameNoSuffix = endIndex > 0 ? assetName.Substring(0, endIndex) : assetName;
                // 用bundleName和预制体名获取预制体所在的路径
                string[] scenePaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(bundleName, assetNameNoSuffix);
                if (scenePaths.Length == 0)
                {
                    LogMgr.LogMgr.LogError("在编辑器模式没有“" + assetName + "”这个场景与bundle：" + bundleName + "对应");
                    yield break;
                }
                AsyncOperation sceneOption = null;
                // 是否叠加载场景
                if (additive)
                {
                    sceneOption = SceneManager.LoadSceneAsync(scenePaths[0], LoadSceneMode.Additive);
                }
                else
                {
                    // 单一加载场景
                    sceneOption = SceneManager.LoadSceneAsync(scenePaths[0], LoadSceneMode.Single);
                }
                if (sceneOption == null)
                {
                    LogMgr.LogMgr.LogError("assetbundle加载场景AsyncOperation=null，加载失败");
                    yield break;
                }
                while (!sceneOption.isDone)
                {
                    // 向外发进度
                    EventCenter.Broadcast(EventDefine.ShowProgress, sceneOption.progress);
                    yield return null;
                    //yield return sceneOption.progress;   //这句和上一句效果一样yield return 后面这个数字不管是多少都是暂停一帧的意思
                }

            }
            else
#endif
            {
                yield break;
            }
        }

        public void UnLoadSceneAsync(PrefabName sceneName)
        {
            MonoMgr.Instance.StartCoroutine(ReallyUnLoadSceneAsync(sceneName));
        }
        private IEnumerator ReallyUnLoadSceneAsync(PrefabName sceneName)
        {
            Resources.UnloadUnusedAssets();
            if (SimulateAssetBundleInEditor)
            {
                yield break;
            }
            PrefabCfgItem itemCfg = PrefabCfgCtrl.Instance.Get((int)sceneName);
            string bundleName = itemCfg.AssetBundleName;
            string assetName = itemCfg.AssetName;
            // 去除后缀
            int endIndex = assetName.LastIndexOf(".");
            string assetNameNoSuffix = endIndex > 0 ? assetName.Substring(0, endIndex) : assetName;
            // 用bundleName和预制体名获取预制体所在的路径
            string[] scenePaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(bundleName, assetNameNoSuffix);
            AsyncOperation ao = SceneManager.UnloadSceneAsync(scenePaths[0]);
            while (!ao.isDone)
            {
                // 调用显示卸载进度
                EventCenter.Broadcast(EventDefine.ShowProgress, ao.progress);
                yield return ao.progress;
            }
            yield return null;
        }
        #endregion====================================================加载场景==============================================================

        #region ====================================================加载Image==============================================================
        /// <summary>
        /// assetbundle异步加载资源
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="prefabName">预制体名字</param>
        /// <param name="path">要加载的对象的路径</param>
        /// <param name="callback">回调函数</param>
        public void LoadImage(PrefabName prefabName, string assetName, string path = null, UnityAction<Sprite> callback = null)
        {
            MonoMgr.Instance.StartCoroutine(ReallyLoadImage(prefabName, assetName, path, callback));
        }

        private IEnumerator ReallyLoadImage(PrefabName prefabName, string assetName, string path = null, UnityAction<Sprite> callback = null)
        {
            yield return null;
            //从表格里面取名字
            PrefabCfgItem itemCfg = PrefabCfgCtrl.Instance.Get((int)prefabName);
            string bundleName = itemCfg.AssetBundleName;

#if UNITY_EDITOR
            if (SimulateAssetBundleInEditor)
            {
                // 获取文件夹下所有的Image
                string[] atlasArry = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(bundleName, assetName);
                if (atlasArry.Length == 0)
                {
                    LogMgr.LogMgr.LogError("在编辑器模式没有“" + bundleName + "”这个图集文件夹！或者没有" + assetName + "这个资源");
                    yield break;
                }
                UnityEngine.Sprite obj = AssetDatabase.LoadAssetAtPath<Sprite>(atlasArry[0]);
                callback(obj);

            }
            else
#endif
            {
                if (path == null)
                {
                    path = Application.streamingAssetsPath + @"/AssetBundles/" + bundleName;
                }
                mRequest = UnityWebRequestAssetBundle.GetAssetBundle(path);
                yield return mRequest.SendWebRequest();
                if (mRequest.isNetworkError || mRequest.isHttpError)
                {
                    LogMgr.LogMgr.Log(mRequest.error);
                }
                AssetBundle ab = DownloadHandlerAssetBundle.GetContent(mRequest);
                if (ab == null)
                {
                    LogMgr.LogMgr.LogError(string.Format("Dont get assetbundle {0}!!!", bundleName));
                    yield break;
                }
                var obj = ab.LoadAssetAsync(bundleName);
                yield return obj;
                if (obj.isDone)
                {
                    if (obj.asset == null)
                    {
                        LogMgr.LogMgr.LogError("LoadAssetAsync failed obj is null");
                        yield break;
                    }
                    callback(obj.asset as Sprite);

                }
                loadedImages.Add(ab);
            }
        }


        /// <summary>
        /// 卸载assetbundle,在模拟模式下没有走assetbundle
        /// </summary>
        public void UnLoadImage()
        {
            Resources.UnloadUnusedAssets();
            if (SimulateAssetBundleInEditor)
            {
                return;
            }
            foreach (var item in loadedImages)
            {
                item.Unload(false);
            }
        }
        #endregion  ====================================================加载Image===========================================================


    }
}