using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using CQCFrameWork.GameObjectMgr;

namespace CQCFrameWork.SceneMgr
{
    public class ScenesMgr : Singleton<ScenesMgr>
    {
        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="name">场景名</param>
        /// <param name="isAdditive">是否叠加显示场景</param>
        /// <param name="func">回调函数</param>
        public void LoadSceneAsync(string name, UnityAction func, bool isAdditive = false)
        {
            GameObjectMgr.GameObjectMgr.Instance.ABShowPanel(PrefabName.LoadingPanel, UI_Layer.Top);
            MonoMgr.Instance.StartCoroutine(ReallyLoadSceneAsync(name, func, isAdditive));
        }
        private IEnumerator ReallyLoadSceneAsync(string name, UnityAction func, bool isAdditive = false)
        {
            AsyncOperation ao = null;
            if (isAdditive)
            {
                ao = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
            }
            else
            {
                ao = SceneManager.LoadSceneAsync(name, LoadSceneMode.Single);
            }
            if (ao == null)
            {
                LogMgr.LogMgr.LogError("加载场景AsyncOperation=null，加载失败");
                yield break;
            }
            func();
            while (!ao.isDone)
            {
                // 调用显示加载进度
                EventCenter.Broadcast(EventDefine.ShowProgress, ao.progress);
                yield return null;
            }
           
        }
        /// <summary>
        /// 异步卸载场景
        /// </summary>
        /// <param name="name"></param>
        /// <param name="func"></param>
        public void UnLoadSceneAsync(string name)
        {
            MonoMgr.Instance.StartCoroutine(ReallyUnLoadSceneAsync(name));
        }
        private IEnumerator ReallyUnLoadSceneAsync(string name)
        {
            AsyncOperation ao = SceneManager.UnloadSceneAsync(name);
            while (!ao.isDone)
            {
                // 调用显示卸载进度
                EventCenter.Broadcast(EventDefine.ShowProgress, ao.progress);
                yield return ao.progress;
            }
        }


    }
}