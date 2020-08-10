using CQCFrameWork.AssetBundlesMgr;
using CQCFrameWork.Panel;
using CQCFrameWork.ResourcesMgr;
using System.Collections.Generic;
using UnityEngine;

namespace CQCFrameWork.GameObjectMgr
{
    /// <summary>
    /// UI层级
    /// </summary>
    public enum UI_Layer
    {
        Bot,    //底层,只能显示一个界面
        Mid,    //中层,可叠加界面
        Top,    //高层,可叠加界面
        System, //系统提示层,可叠加界面
    }
    /// <summary>
    /// 游戏对象管理类
    /// </summary>
    public class GameObjectMgr : Singleton<GameObjectMgr>
    {
        public Dictionary<string, BasePanel> panelDicRe = new Dictionary<string, BasePanel>();
        public Dictionary<PrefabName, BasePanel> panelDicAB = new Dictionary<PrefabName, BasePanel>();
        public Dictionary<PrefabName, Base3DObject> Object3DDicAB = new Dictionary<PrefabName, Base3DObject>();
        Transform bot, mid, top, system;
        public GameObjectMgr()
        {
            GameObject ga = GameObject.Find("Canvas");
            if (ga != null)
            {
                GameObject.DontDestroyOnLoad(ga);
                bot = ga.transform.Find("Bot");
                mid = ga.transform.Find("Mid");
                top = ga.transform.Find("Top");
                system = ga.transform.Find("System");
                return;
            }
            GameObject canvas = ResMgr.Instance.Load<GameObject>("Canvas");
            GameObject.DontDestroyOnLoad(canvas);
            canvas.name = canvas.name.Substring(0, 6);   // 去掉(clone)
            bot = canvas.transform.Find("Bot");
            mid = canvas.transform.Find("Mid");
            top = canvas.transform.Find("Top");
            system = canvas.transform.Find("System");
        }

        #region ======================同步显示/关闭resources文件夹中MessageTip==================================

        /// <summary>
        /// UI需要手动输入层级设置 显示resources资源
        /// </summary>
        /// <typeparam name="T">输入界面类型，一般和脚本名字相同</typeparam>
        /// <param name="panelName">输入预制体在resources中的路径</param>
        /// <param name="layer">输入界面将要显示的层级</param>
        /// <param name="args">输入要传入的参数</param>
        public void ShowMessageTip(params object[] localizationKeyString)
        {
            GameObject obj = ResMgr.Instance.Load<GameObject>("MessageTip");
            obj.transform.SetParent(top);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            (obj.transform as RectTransform).offsetMax = Vector2.zero;
            (obj.transform as RectTransform).offsetMin = Vector2.zero;
            //去掉(clone)
            obj.name = obj.name.Substring(0, obj.name.Length - 7);
            //得到预设体身上的面板脚本
            BasePanel panel = obj.GetComponent<BasePanel>();
            if (panel != null)
            {
                panel.OpenPanel(PrefabName.NULL, localizationKeyString);
            }
            else
            {
                LogMgr.LogMgr.LogError(string.Format("没有找到{0}界面的BasePanel脚本，是不是忘记挂了？？", obj.name));
            }

        }

        #endregion ===============================同步显示/关闭resources文件夹中MessageTip========================================================

        #region ===============================同步显示/关闭resources文件夹中资源==================================================================

        /// <summary>
        /// UI默认层级为Mid 显示resources资源
        /// 默认在top层
        /// </summary>
        /// <typeparam name="T">输入界面类型，一般和脚本名字相同</typeparam>
        /// <param name="panelName">输入预制体在resources中的路径</param>
        /// <param name="args">输入要传入的参数</param>
        public void ShowPanel(string panelName, params object[] args)
        {
            UI_Layer layer = UI_Layer.Top;
            ShowPanel(panelName, layer, args);
        }
        /// <summary>
        /// UI需要手动输入层级设置 显示resources资源
        /// </summary>
        /// <typeparam name="T">输入界面类型，一般和脚本名字相同</typeparam>
        /// <param name="panelName">输入预制体在resources中的路径</param>
        /// <param name="layer">输入界面将要显示的层级</param>
        /// <param name="args">输入要传入的参数</param>
        public void ShowPanel(string panelName, UI_Layer layer, params object[] args)
        {
            GameObject obj = ResMgr.Instance.Load<GameObject>(panelName);
            Transform father = null;  //默认下层
            switch (layer)
            {
                case UI_Layer.Bot:
                    if (bot.childCount > 0)
                    {
                        for (int i = 0; i < bot.childCount; i++)
                        {
                            GameObject.Destroy(bot.GetChild(i).gameObject);
                        }
                    }
                    father = bot;
                    break;
                case UI_Layer.Mid:
                    father = mid;
                    break;
                case UI_Layer.Top:
                    father = top;
                    break;
                case UI_Layer.System:
                    father = system;
                    break;
            }
            obj.transform.SetParent(father);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            (obj.transform as RectTransform).offsetMax = Vector2.zero;
            (obj.transform as RectTransform).offsetMin = Vector2.zero;
            //去掉(clone)
            obj.name = obj.name.Substring(0, obj.name.Length - 7);
            //得到预设体身上的面板脚本
            BasePanel panel = obj.GetComponent<BasePanel>();
            if (panel != null)
            {
                //把面板存起来
                if (!panelDicRe.ContainsKey(panelName))
                {
                    panelDicRe.Add(panelName, panel);
                }
                panel.OpenPanel(PrefabName.NULL, args);
            }
            else
            {
                LogMgr.LogMgr.LogError(string.Format("没有找到{0}界面的BasePanel脚本，是不是忘记挂了？？", obj.name));
            }

        }
        /// <summary>
        /// UI关闭同步、异步界面 resources资源
        /// </summary>
        /// <param name="panelName">输入预制体在resources中的路径</param>
        public void ClosePanel(string panelName)
        {
            if (panelDicRe.ContainsKey(panelName))
            {
                GameObject.Destroy(panelDicRe[panelName].gameObject);
                panelDicRe.Remove(panelName);
            }
            else
            {
                LogMgr.LogMgr.LogError("在已加载的界面列表中没有找到该界面名字");
            }
        }
        #endregion ===============================同步显示/关闭resources文件夹中资源==================================================================

        #region ===============================异步显示/关闭resources文件夹中资源==================================================================

        /// <summary>
        /// UI默认层级为Mid 显示resources资源
        /// 一般只有主界面UI在Bot层，提示信息在top层
        /// </summary>
        /// <typeparam name="T">输入界面类型，一般和脚本名字相同</typeparam>
        /// <param name="panelName">输入预制体在resources中的路径</param>
        /// <param name="args">输入要传入的参数</param>
        public void ShowPanel1(string panelName, params object[] args)
        {
            UI_Layer layer = UI_Layer.Mid;
            ShowPanel1(panelName, layer, args);
        }
        /// <summary>
        /// UI需要手动输入层级设置 显示resources资源
        /// </summary>
        /// <typeparam name="T">输入界面类型，一般和脚本名字相同</typeparam>
        /// <param name="panelName">输入预制体在resources中的路径</param>
        /// <param name="layer">输入界面将要显示的层级</param>
        /// <param name="args">输入要传入的参数</param>
        public void ShowPanel1(string panelName, UI_Layer layer, params object[] args)
        {
            ResMgr.Instance.LoadAsync<GameObject>(panelName, obj =>
            {
                Transform father = null;  //默认下层
                switch (layer)
                {
                    case UI_Layer.Bot:
                        if (bot.childCount > 0)
                        {
                            for (int i = 0; i < bot.childCount; i++)
                            {
                                GameObject.Destroy(bot.GetChild(i).gameObject);
                            }
                        }
                        father = bot;
                        break;
                    case UI_Layer.Mid:
                        father = mid;
                        break;
                    case UI_Layer.Top:
                        father = top;
                        break;
                    case UI_Layer.System:
                        father = system;
                        break;
                }
                obj.transform.SetParent(father);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localScale = Vector3.one;
                (obj.transform as RectTransform).offsetMax = Vector2.zero;
                (obj.transform as RectTransform).offsetMin = Vector2.zero;
                //去掉(clone)
                obj.name = obj.name.Substring(0, obj.name.Length - 7);
                //得到预设体身上的面板脚本
                BasePanel panel = obj.GetComponent<BasePanel>();
                if (panel != null)
                {
                    //把面板存起来
                    if (!panelDicRe.ContainsKey(panelName))
                    {
                        panelDicRe.Add(panelName, panel);
                    }
                    panel.OpenPanel(PrefabName.NULL, args);
                }
                else
                {
                    LogMgr.LogMgr.LogError(string.Format("没有找到{0}界面的BasePanel脚本，是不是忘记挂了？？", obj.name));
                }
            });
        }

        #endregion===============================异步显示/关闭resources文件夹中的窗口资源===============================================================

        #region ===========================异步显示/关闭assetbundle窗口资源=================================

        /// <summary>
        /// UI异步加载 assetbundle资源,默认为Mid层
        /// 一般只有主界面UI在Bot层，提示信息在top层
        /// </summary>
        /// <param name="panelName">界面名字</param>
        /// <param name="args">要传给界面的参数</param>
        public void ABShowPanel(PrefabName panelName, params object[] args)
        {
            UI_Layer layer = UI_Layer.Mid;
            ABShowPanel(panelName, layer, null, args);
        }
        /// <summary>
        /// UI异步加载 assetbundle资源,手动设置层级
        /// </summary>
        /// <param name="panelName">界面名字</param>
        /// <param name="layer">UI层级</param>
        /// <param name="path">路径</param>
        /// <param name="args">参数</param>
        public void ABShowPanel(PrefabName panelName, UI_Layer layer, string path = null, params object[] args)
        {
            AssetBundleMgr.Instance.ABLoadAsync<GameObject>(panelName, path, obj =>
             {
                 Transform father = null;
                 switch (layer)
                 {
                     case UI_Layer.Bot:
                         father = bot;
                         if (father.childCount >= 1)
                         {
                             CloseABPanel(father.GetChild(0).GetComponent<BasePanel>().prefabName);
                         }
                         break;
                     case UI_Layer.Mid:
                         father = mid;
                         break;
                     case UI_Layer.Top:
                         father = top;
                         break;
                     case UI_Layer.System:
                         father = system;
                         break;
                 }
                 obj.transform.SetParent(father);
                 obj.transform.localPosition = Vector3.zero;
                 obj.transform.localScale = Vector3.one;
                 //UGUI才有这两句
                 (obj.transform as RectTransform).offsetMax = Vector2.zero;
                 (obj.transform as RectTransform).offsetMin = Vector2.zero;
                 obj.name = obj.name.Substring(0, obj.name.Length - 7);
                 //得到预设体身上的面板脚本
                 BasePanel panel = obj.GetComponent<BasePanel>();
                 if (panel != null)
                 {
                     //把面板存起来
                     if (!panelDicAB.ContainsKey(panelName))
                     {
                         panelDicAB.Add(panelName, panel);
                     }
                     panel.OpenPanel(panelName, args);
                 }
                 else
                 {
                     LogMgr.LogMgr.LogError(string.Format("没有找到{0}界面的BasePanel脚本，是不是忘记挂了？？", obj.name));
                 }
             });
        }

        /// <summary>
        /// UI关闭界面 AssetBundle资源
        /// </summary>
        /// <param name="panelName">输入预制体在resources中的路径</param>
        public void CloseABPanel(PrefabName panelName)
        {
            if (panelDicAB.ContainsKey(panelName))
            {
                GameObject.Destroy(panelDicAB[panelName].gameObject);
                panelDicAB.Remove(panelName);
                AssetBundleMgr.Instance.UnLoadAB(panelName);
            }
            else
            {
                LogMgr.LogMgr.LogError("在已加载的界面列表中没有找到" + panelName + "界面名字");
            }

        }
        #endregion===================异步显示/关闭assetbundle窗口资源===========================================

        #region ===============================异步加载/移除assetbundle 3D物体资源==============================================================   
        /// <summary>
        /// 3D对象异步加载 assetbundle资源
        /// </summary>
        /// <param name="Object3DName">3d物体名</param>
        /// <param name="parent">父级</param>
        /// <param name="path">路径</param>
        /// <param name="args">参数</param>
        public void Show3DObject(PrefabName Object3DName, Transform parent = null, string path = null, params object[] args)
        {
            AssetBundleMgr.Instance.ABLoadAsync<GameObject>(Object3DName, path, obj =>
            {
                if (parent != null)
                {
                    obj.transform.SetParent(parent);
                }
                //默认生成状态
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localScale = Vector3.one;
                obj.transform.rotation = Quaternion.identity;
                obj.name = obj.name.Substring(0, obj.name.Length - 7);
                //得到预设体身上的3d对象脚本
                Base3DObject obj3D = obj.GetComponent<Base3DObject>();
                if (obj3D != null)
                {
                    //把3d对象存起来
                    if (!Object3DDicAB.ContainsKey(Object3DName))
                    {
                        Object3DDicAB.Add(Object3DName, obj3D);
                    }
                    obj3D.OnAwake();
                    obj3D.Instantiate(args);
                }
                else
                {
                    LogMgr.LogMgr.LogError(string.Format("没有找到{0}界面的Base3DObject脚本，是不是忘记挂了？？", obj.name));
                }
            });
        }

        public void Destory3DObj(PrefabName Object3DName)
        {
            if (Object3DDicAB.ContainsKey(Object3DName))
            {
                GameObject.Destroy(Object3DDicAB[Object3DName].gameObject);
                Object3DDicAB.Remove(Object3DName);
                AssetBundleMgr.Instance.UnLoadAB(Object3DName);
            }
            else
            {
                LogMgr.LogMgr.LogError("在已加载的3d物体列表中没有找到该3d物体名字");
            }
        }
        #endregion===============================异步加载/移除assetbundle 3D物体资源==============================================================




    }
}