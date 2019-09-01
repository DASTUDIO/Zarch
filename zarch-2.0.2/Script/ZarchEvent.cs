using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Z
{
    public class ZarchEvent : MonoBehaviour
    {

        public System.Action zclick;

        public void Z_Click(UnityEngine.EventSystems.BaseEventData data = null) { if (zclick != null) { zclick(); } }

    }
}