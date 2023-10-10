using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace WheelsSystem
{
    public abstract class Absorber : MonoBehaviour
    {
        public event Action OnUpdade;
        public Rigidbody body;
        public Rigidbody connectedBody;

        protected void CallUpdate()
        {
            if (OnUpdade != null)
            {
                OnUpdade();
            }
        }
    }
}
