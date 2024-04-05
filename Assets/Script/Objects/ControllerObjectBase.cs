using UnityEngine;

namespace Script.Objects
{
    public abstract class ControllerObjectBase : MonoBehaviour
    {
        protected bool activated;
        public abstract void Activate();
        public abstract void Deactivate();


    }
}