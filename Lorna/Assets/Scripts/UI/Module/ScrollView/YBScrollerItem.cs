using UnityEngine;
using System.Collections;

namespace FrameWork_UI
{
    public class YBScrollerItem : MonoBehaviour {
        private RectTransform m_Rt;
        public RectTransform Rt {
            get {
                if(!m_Rt) {
                    m_Rt = GetComponent<RectTransform>();
                }
                return m_Rt;
            }
        }
        public int dataIndex;
    }
}