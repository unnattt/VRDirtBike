using UnityEngine;
using System.Collections;
using System;

namespace WheelsSystem
{
    [ExecuteInEditMode]
    [AddComponentMenu("Physics/Wheels System/Details/Steering")]
    [DisallowMultipleComponent]
    public class Steering : MonoBehaviour
    {
        [Serializable]
        public struct Parameters
        {
            public string name;
            public float torque;
            public float velocity;
            public float damper;
            public float limit; 
            public float mass;

            public Parameters(string name, float torque, float velocity, float damper, float limit, float mass)
            {
                this.name = name;
                this.torque = torque;
                this.velocity = velocity;
                this.damper = damper;
                this.limit = limit;
                this.mass = mass;
            }
            public static bool operator !=(Parameters p1, Parameters p2)
            {
                return p1.torque != p2.torque || p1.velocity != p2.velocity || p1.damper != p2.damper || p1.limit != p2.limit || p1.mass != p2.mass;
            }
            public static bool operator ==(Parameters p1, Parameters p2)
            {
                return p1.torque == p2.torque && p1.velocity == p2.velocity && p1.damper == p2.damper && p1.limit == p2.limit && p1.mass == p2.mass;
            }
            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
            public override string ToString()
            {
                return string.Format("[Parameters]");
            }
        }
        public Parameters parameters;
        private float rotationX;
        [System.NonSerialized] public float rotation;
        public float rotationUnit
        {
            get{ return rotation / parameters.limit; }
        }
        public float driftFactor
        {
            get;
            set;
        }

        void OnEnable ()
        {
            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                parameters = new Parameters(name, 10000.0f, 50.0f, 5.0f, 30.0f, 5.0f);
            }
            else
            {
            #endif
                driftFactor = 1.0f;
                rotationX = transform.localRotation.eulerAngles.x;
            #if UNITY_EDITOR
            }
            #endif
        }
            
        void Update ()
        {
            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (parameters.name != name)
                {
                    parameters.name = name;
                }
            }
            else
            {
            #endif
                transform.localRotation = Quaternion.Euler(rotationX, 0.0f, rotation);
            #if UNITY_EDITOR
            }
            #endif
        }

        public void Rotate(float unitSpeed)
        {
            unitSpeed = Mathf.Clamp(unitSpeed, -1.0f, 1.0f);
            rotation += driftFactor * parameters.velocity * unitSpeed * Time.fixedDeltaTime;
            rotation = Mathf.Clamp(rotation, -parameters.limit, parameters.limit);
        }
        public void Normalize ()
        {
            if (rotation > 1.0f)
            {
                rotation -= 0.5f * parameters.velocity * Time.fixedDeltaTime;
            }
            else if (rotation < -1.0f)
            {
                rotation += 0.5f * parameters.velocity * Time.fixedDeltaTime;
            }
            else
            {
                rotation = 0.0f;
            }
            rotation = Mathf.Clamp(rotation, -parameters.limit, parameters.limit);
        }
    }
}