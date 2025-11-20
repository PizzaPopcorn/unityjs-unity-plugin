using UnityEngine;

namespace UnityJS.Events
{
    [JSExposedClass("animator.setTrigger")]
    public class Event_AnimatorSetTrigger : JSEventVoid<GameObject, string>
    {
        protected override void Invoke(GameObject target, string trigger)
        {
            target.GetComponent<Animator>().SetTrigger(trigger);
        }
    }
}