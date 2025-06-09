using UnityEngine;

namespace CodeBase.UI.Elements.Hud
{
    public class GroupSelector : MonoBehaviour
    {
        public void Select(int groupId)
        {
            KeyCode[] keys = {KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5};
            
            SimpleInputHelper.TriggerKeyClick(keys[groupId]);
        }

        public void AllIn()
        {
            SimpleInputHelper.TriggerKeyClick(KeyCode.Space);
        }
    }
}