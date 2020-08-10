using CQCFrameWork.Panel;
using DG.Tweening;
using UnityEngine;
    public class MessageTip : BasePanel
    {
        protected override void OnPanelInitialize(params object[] args)
        {
            int length = Localization.Get(args[0].ToString()).Length;
            Transform bg = transform.Find("Image");
            bg.GetComponent<RectTransform>().sizeDelta = new Vector2(20 + length * 30, 45f);
            Transform text = transform.Find("Text");
            text.GetComponent<LocalizationText>().KeyString = args[0].ToString();
            text.GetComponent<LocalizationText>().fontSize = 30;
            text.GetComponent<RectTransform>().sizeDelta = new Vector2(20 + length * 30, 45f);
            transform.DOLocalMoveY(50, 1f).OnComplete(
                () => { Destroy(gameObject); });
        }
    }

