//挂在父物体上检测UI的射线检测有没有打开
using UnityEngine;
using UnityEngine.UI;
class RayCheck : MonoBehaviour
{
    static Vector3[] fourCorners = new Vector3[4];
    void OnDrawGizmos()
    {
        foreach (MaskableGraphic g in GameObject.FindObjectsOfType<MaskableGraphic>())
        {
            if (g.raycastTarget)
            {
                RectTransform rectTransform = g.transform as RectTransform;
                rectTransform.GetWorldCorners(fourCorners);
                for (int i = 0; i < 4; i++)
                    Debug.DrawLine(fourCorners[i], fourCorners[(i + 1) % 4], Color.blue);

            }
        }
    }
  
}
