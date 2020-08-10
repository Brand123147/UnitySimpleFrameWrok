/*
 * 1.让没继承MonoBehaviour类的脚本通过调用MonoMgr.Instance.XXX也可以实现MonoBehaviour中的功能
 * 2.要用到什么封装什么可自行添加
 * 
 */

using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Internal;

public class MonoMgr : Singleton<MonoMgr>
{
    public MonoController controller;
    public MonoMgr()
    {
        //保证了monocontroller对象的唯一性
        GameObject obj = new GameObject("MonoController");
        controller = obj.AddComponent<MonoController>();
    }
    public void AddUpdateListener(UnityAction func)
    {
        controller.AddUpdateListener(func);
    }
    public void RemoveUpdateListener(UnityAction func)
    {
        controller.RemoveUpdateListener(func);
    }
    public Coroutine StartCoroutine(string methodName)
    {
        return controller.StartCoroutine(methodName);
    }
    
    public Coroutine StartCoroutine(IEnumerator routine)
    {
        return controller.StartCoroutine(routine);
    }
    public Coroutine StartCoroutine(string methodName, [DefaultValue("null")] object value)
    {
        return controller.StartCoroutine(methodName, value);
    }
    public void StopAllCoroutines()
    {
        controller.StopAllCoroutines();
    }
    public void StopCoroutine(IEnumerator routine)
    {
        controller.StopCoroutine(routine);
    } 
    public void StopCoroutine(Coroutine routine)
    {
        controller.StopCoroutine(routine);
    }
    public void StopCoroutine(string methodName)
    {
        controller.StopCoroutine(methodName);
    }
   
}
