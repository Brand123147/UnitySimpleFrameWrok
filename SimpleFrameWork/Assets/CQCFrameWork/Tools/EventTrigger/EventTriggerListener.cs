using CQCFrameWork.AudioMgr;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventTriggerListen
{
    public static EventTriggerListener Get(GameObject go, params object[] args)
    {
        EventTriggerListener listener = new EventTriggerListener().Get(go);
        listener.data = args;
        return listener;
    }
}

public class EventTriggerListener : EventTrigger
{
    public object[] data;
    public delegate void EventTriggerHandler(GameObject go,params object[] args);
    public EventTriggerHandler poninterEnter;
    public EventTriggerHandler poninterExit;
    public EventTriggerHandler beginDrag;
    public EventTriggerHandler cancle;
    public EventTriggerHandler deselect;
    public EventTriggerHandler drag;
    public EventTriggerHandler drop;
    public EventTriggerHandler endDrag;
    public EventTriggerHandler initPotentialDrag;
    public EventTriggerHandler move;
    public EventTriggerHandler pointerClick;
    public EventTriggerHandler pointerDown;
    public EventTriggerHandler pointerUp;
    public EventTriggerHandler scroll;
    public EventTriggerHandler select;
    public EventTriggerHandler submit;
    public EventTriggerHandler updateSelected;
    public EventTriggerListener Get(GameObject go)
    {
        EventTriggerListener listener = go.GetComponent<EventTriggerListener>();
        if (listener == null) listener = go.AddComponent<EventTriggerListener>();
        return listener;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        poninterEnter?.Invoke(gameObject, data);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        poninterExit?.Invoke(gameObject, data);
    }
    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        beginDrag?.Invoke(gameObject, data);
    }
    /// <summary>
    /// 取消
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnCancel(BaseEventData eventData)
    {
        base.OnCancel(eventData);
        cancle?.Invoke(gameObject, data);
    }
    /// <summary>
    /// 取消选择
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        deselect?.Invoke(gameObject, data);
    }
    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        drag?.Invoke(gameObject, data);
    }
    public override void OnDrop(PointerEventData eventData)
    {
        base.OnDrop(eventData);
        drop?.Invoke(gameObject, data);
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        endDrag?.Invoke(gameObject, data);
    }
    public override void OnInitializePotentialDrag(PointerEventData eventData)
    {
        base.OnInitializePotentialDrag(eventData);
        initPotentialDrag?.Invoke(gameObject, data);
    }
    public override void OnMove(AxisEventData eventData)
    {
        base.OnMove(eventData);
        move?.Invoke(gameObject, data);
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        AudioMgr.Instance.PlaySound(PrefabName.Click_touch);  //自定义按钮声音
        base.OnPointerClick(eventData);
        pointerClick?.Invoke(gameObject, data);
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        pointerDown?.Invoke(gameObject, data);
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        pointerUp?.Invoke(gameObject, data);
    }
    public override void OnScroll(PointerEventData eventData)
    {
        base.OnScroll(eventData);
        scroll?.Invoke(gameObject, data);
    }
    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        select?.Invoke(gameObject, data);
    }
    public override void OnSubmit(BaseEventData eventData)
    {
        base.OnSubmit(eventData);
        submit?.Invoke(gameObject, data);
    }
    public override void OnUpdateSelected(BaseEventData eventData)
    {
        base.OnUpdateSelected(eventData);
        updateSelected?.Invoke(gameObject, data);
    }
}
