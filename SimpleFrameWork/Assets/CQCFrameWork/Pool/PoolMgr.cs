using CQCFrameWork.ResourcesMgr;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CQCFrameWork.Pool
{
    public class PoolData
    {
        GameObject fatherObj;
        public List<GameObject> poolList;
        /// <summary>
        /// 初始化一个容器来装GameObject，并把这个容器命名为物体名字，作为父物体进行归类
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="poolObj"></param>
        public PoolData(GameObject obj, GameObject poolObj)
        {
            fatherObj = new GameObject(obj.name);
            fatherObj.transform.parent = poolObj.transform;
            poolList = new List<GameObject>();
            PushObj(obj);
        }
        public void PushObj(GameObject obj)
        {
            obj.transform.parent = fatherObj.transform;
            poolList.Add(obj);
        }
        public GameObject GetObj()
        {
            GameObject obj = null;
            obj = poolList[0];
            poolList.RemoveAt(0);
            obj.transform.parent = null;
            return obj;
        }
    }
    public class PoolMgr : Singleton<PoolMgr>
    {
        //设置Pool节点，把归类的GameObject挂载Pool下
        public Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();
        private GameObject poolObj;
        const string mPool = "Pool";
        /// <summary>
        /// 获取对象池里的东西
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public void GetObj(string path, UnityAction<GameObject> callback)
        {
            if (poolDic.ContainsKey(path) && poolDic[path].poolList.Count > 0)
            {
                callback(poolDic[path].GetObj());
            }
            //第一次加载
            else
            {
                ResMgr.Instance.LoadAsync<GameObject>(path, o =>
                {
                    o.name = path;
                    callback(o);
                });
            }
        }
        /// <summary>
        /// 放回对象池
        /// </summary>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        public void PushObj(string name, GameObject obj)
        {
            //设置Pool节点
            if (poolObj == null)
            {
                poolObj = new GameObject(mPool);
                poolObj.SetActive(false);
            }
            //如果有这个归类
            if (poolDic.ContainsKey(name))
            {
                poolDic[name].PushObj(obj);
            }
            else
            {
                poolDic.Add(name, new PoolData(obj, poolObj));
            }
        }

        //切换场景时删除对象池
        public void ClearPool()
        {
            poolDic.Clear();
            MonoBehaviour.Destroy(poolObj);
        }
    }
}