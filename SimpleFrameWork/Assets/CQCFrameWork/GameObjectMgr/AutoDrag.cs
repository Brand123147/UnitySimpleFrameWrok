/* 
 * 随意拖拽ui脚本，直接挂在要拖拽的UI上即可
 * 相当于ngui中的ondrag
 */
using UnityEngine;
using UnityEngine.EventSystems;

public class AutoDrag : MonoBehaviour, IDragHandler
{
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }
}
