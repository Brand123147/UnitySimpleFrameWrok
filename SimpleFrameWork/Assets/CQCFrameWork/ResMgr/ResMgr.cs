/* 
 * Resources文件夹资源加载
 */
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace CQCFrameWork.ResourcesMgr
{
    public class ResMgr : Singleton<ResMgr>
    {
        /// <summary>
        /// 从resources同步加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public T Load<T>(string path) where T : UnityEngine.Object
        {
            T res = Resources.Load<T>(path);
            if (res is GameObject)
            {
                return UnityEngine.Object.Instantiate(res);
            }
            else  //文本， 声音文件啥的
            {
                return res;
            }
        }
        /// <summary>
        /// 从resources异步加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="callback"></param>
        public void LoadAsync<T>(string path, UnityAction<T> callback = null) where T : UnityEngine.Object
        {
            MonoMgr.Instance.StartCoroutine(ReallyLoadAsync(path, callback));
        }
        private IEnumerator ReallyLoadAsync<T>(string path, UnityAction<T> callback = null) where T : UnityEngine.Object
        {
            ResourceRequest rr = Resources.LoadAsync<T>(path);
            yield return rr;

            if (rr.asset == null)
            {
                LogMgr.LogMgr.LogError(string.Format("Dont find the {0} resource!!!", path));
            }
            if (rr.asset is GameObject)
            {
                callback(GameObject.Instantiate(rr.asset) as T);
            }
            else
            {
                callback(rr.asset as T);
            }
        }
        
    }
}