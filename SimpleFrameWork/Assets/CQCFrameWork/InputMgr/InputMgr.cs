/* 
 * 按钮检测模块
 */
using UnityEngine;
namespace CQCFrameWork.InputMgr
{
    public class InputMgr : Singleton<InputMgr>
    {
        /// <summary>
        /// 是否开启输入检测
        /// </summary>
        bool isOpenCheckInput = false;
        public InputMgr()
        {
            MonoMgr.Instance.AddUpdateListener(Update);
        }
        /// <summary>
        /// 是否开启输入检测
        /// </summary>
        /// <param name="isOpenOrClose"></param>
        public void OpenOrCloseInputCheck(bool isOpenOrClose)
        {
            isOpenCheckInput = isOpenOrClose;
        }
        //没有继承monobehaviour所以Update只能走之前写的MonoMgrController那个Update
        //其实即使有继承Monobehaviour也建议走MonoMgrController，以防增加不必要的开销
        private void Update()
        {
            if (!isOpenCheckInput)
            {
                return;
            }
            CheckKeyCode(KeyCode.W);
            CheckKeyCode(KeyCode.A);
            CheckKeyCode(KeyCode.S);
            CheckKeyCode(KeyCode.D);
            CheckKeyCode(KeyCode.Space);
            CheckKeyCode(KeyCode.UpArrow);
            CheckKeyCode(KeyCode.DownArrow);
            CheckKeyCode(KeyCode.LeftArrow);
            CheckKeyCode(KeyCode.RightArrow);
            CheckKeyCode(KeyCode.J);
            CheckKeyCode(KeyCode.K);
            CheckKeyCode(KeyCode.L);
            CheckKeyCode(KeyCode.U);
            CheckKeyCode(KeyCode.I);
            CheckKeyCode(KeyCode.O);
            CheckKeyCode(KeyCode.Mouse0);
            CheckKeyCode(KeyCode.Mouse1);
        }

        private void CheckKeyCode(KeyCode key)
        {
            if (Input.GetKeyDown(key))
            {
                EventCenter.Broadcast(EventDefine.KeyDown, key);
            }
            if (Input.GetKeyUp(key))
            {
                EventCenter.Broadcast(EventDefine.KeyUp, key);
            }
        }
    }
}
