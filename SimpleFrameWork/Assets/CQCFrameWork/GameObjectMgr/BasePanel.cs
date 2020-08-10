using CQCFrameWork.AssetBundlesMgr;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CQCFrameWork.Panel
{
    public abstract class BasePanel : MonoBehaviour
    {
        /// <summary>
        /// 按钮声音
        /// </summary>
        //[SerializeField] protected PrefabName soundBtn = PrefabName.Click_touch;
        List<Transform> children = new List<Transform>();
        [HideInInspector] public PrefabName prefabName;
        public virtual void Start()
        {
            OnRegisterEvent();
        }
        public virtual void OnDestroy()
        {
            OnUnRegisterEvent();
            AssetBundleMgr.Instance.UnLoadImage();
        }
        public virtual void Update()
        {

        }
        /// <summary>
        /// 当外界调用showPanel的时候会自动调用
        /// </summary>
        /// <param name="args">参数</param>
        public void OpenPanel(PrefabName prefabName, params object[] args)
        {
            OnPanelInitialize(args);
            this.prefabName = prefabName;
        }
        /// <summary>
        /// 找到所有子对象,其实这样做不好，每次调用都会去遍历数组中的元素，万一需要的对象在数组的末尾相当于要所有都遍历一遍
        /// 这样写能找到未激活的对象，unity内置的api方法不能找到未激活的对象
        /// </summary>
        /// <typeparam name="T">输入控件类型名</typeparam>
        protected Transform FindChildren(string goName)
        {
            if (children.Count <= 0)
            {
                children.Add(transform);
                for (int i = 0; i < transform.childCount; i++)
                {
                    Transform[] items = transform.GetChild(i).GetComponentsInChildren<Transform>();
                    foreach (var item in items)
                    {
                        children.Add(item);
                    }
                }
            }
            foreach (var item in children)
            {
                if (item.name == goName)
                {
                    return item;
                }
            }
            LogMgr.LogMgr.LogError("界面中没有" + goName + "这个物体");
            return null;
        }


        //------------------------------------------------- UGUI工具---------------------------------------------------------------
        
        /// <summary>
        /// 修改image图片
        /// </summary>
        /// <param name="goName">物体的名字</param>
        /// <param name="sprite">需要设置的图片</param>
        protected void SetImage(PrefabName atlasName, string spriteName, string goName)
        {
            AssetBundleMgr.Instance.LoadImage(atlasName, spriteName, null, (_sprite) => { SetImage(goName, _sprite); });
        }
        protected void SetImage(PrefabName atlasName, string spriteName, Transform tran)
        {
            AssetBundleMgr.Instance.LoadImage(atlasName, spriteName, null, (_sprite) => { SetImage(tran, _sprite); });
        }
        protected void SetImage(PrefabName atlasName, string spriteName, GameObject go)
        {
            AssetBundleMgr.Instance.LoadImage(atlasName, spriteName, null, (_sprite) => { SetImage(go, _sprite); });
        }
        protected void SetImage(string goName, Sprite _sprite)
        {
            FindChildren(goName).GetComponent<Image>().sprite = _sprite;
        }
        protected void SetImage(Transform tran, Sprite _sprite)
        {
            tran.GetComponent<Image>().sprite = _sprite;
        }
        protected void SetImage(GameObject go, Sprite _sprite)
        {
            go.GetComponent<Image>().sprite = _sprite;
        }
        protected void SetImage(string goName, Color _color)
        {
            FindChildren(goName).GetComponent<Image>().color = _color;
        }
        protected void SetImage(Transform tran, Color _color)
        {
            tran.GetComponent<Image>().color = _color;
        }
        /// <summary>
        /// 设置文字
        /// </summary>
        /// <param name="goName"></param>
        /// <param name="str"></param>
        protected void SetText(string goName, string str)
        {
            FindChildren(goName).GetComponent<Text>().text = str;
        }
        protected void SetText(Transform tran, string str)
        {
            tran.GetComponent<Text>().text = str;
        }
        protected void SetText(string goName, Color _color)
        {
            FindChildren(goName).GetComponent<Text>().color = _color;
        }
        protected string GetText(string goName)
        {
            return FindChildren(goName).GetComponent<Text>().text;
        }
        protected InputField GetInputField(string goName)
        {
            return FindChildren(goName).GetComponent<InputField>();
        }
        protected string GetInputFieldText(string goName)
        {
            return FindChildren(goName).GetComponent<InputField>().text;
        }
        /// <summary>
        /// scrollview添加Item
        /// </summary>
        /// <param name="Item"></param>
        /// <param name="content"></param>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        protected void AddScrollView(Transform Item, Transform content, object data, UnityAction<object, int, Transform> callback)
        {
            GameObject go = Instantiate(Item, content).gameObject;
            int index = content.childCount;
            callback(data, index, go.transform);
            go.SetActive(true);
        }

        /// <summary>
        /// 设置Slider
        /// </summary>
        /// <param name="goName"></param>
        /// <param name="value"></param>
        protected void SetSliderValue(string goName, float value)
        {
            FindChildren(goName).GetComponent<Slider>().value = value;
        }




        //------------------------------------------------- 必须实现以下接口---------------------------------------------------------------
        /// <summary>
        /// 窗口初始化
        /// </summary>
        /// <param name="args">外界传进来的参数</param>
        protected abstract void OnPanelInitialize(params object[] args);


        //----------------------------------------------------- 虚函数接口---------------------------------------------------------------
        /// <summary>
        /// 窗口初始化时注册/监听事件
        /// </summary>
        protected virtual void OnRegisterEvent() { }
        /// <summary>
        /// 关闭窗口时移除事件
        /// </summary>
        protected virtual void OnUnRegisterEvent() { }
    }
}