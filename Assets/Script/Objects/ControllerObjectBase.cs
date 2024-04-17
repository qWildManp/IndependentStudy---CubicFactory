using System;
using UnityEngine;

namespace Script.Objects
{
    public abstract class ControllerObjectBase : MonoBehaviour
    {
        public bool activated;
        public MeshRenderer powerIndicator;
        public Material hasPowerMat;
        public Material noPowerMat;
        public abstract void Activate();
        public abstract void Deactivate();

        protected virtual void Awake()
        {
            if(activated)
                Activate();
            else
            {
                Deactivate();
            }
        }
    }
}