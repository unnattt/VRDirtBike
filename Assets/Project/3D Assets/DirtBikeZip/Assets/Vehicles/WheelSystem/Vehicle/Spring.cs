using UnityEngine;
using System.Collections;

namespace WheelsSystem
{
    [DisallowMultipleComponent]
    public class Spring : MonoBehaviour
    {
        public Transform springDownDetail;
        public Transform springUpDetail;
        public Transform downPivot;
        public Transform upPivot;
        public Transform spring;

        private Vector3 springStartScale;
        private float detailsStartDistance;
        private Transform downDetailParent;
        private Vector3 localPosition;

        void Start ()
        {
            if (!springDownDetail || !springUpDetail || !spring || !downPivot || !upPivot)
            {
                enabled = false;
                return;
            }
            springStartScale = spring.localScale;
            detailsStartDistance = Vector3.Distance(downPivot.position, upPivot.position);
        }

        public void SetParent (Transform upDetailParent, Transform downDetailParent)
        {
            if (!springDownDetail || !springUpDetail || !spring || !downPivot || !upPivot )
            {
                enabled = false;
                return;
            }
            springUpDetail.parent = upDetailParent;
            springDownDetail.parent = upDetailParent;
            this.downDetailParent = downDetailParent;
            if (!downDetailParent)
            {
                enabled = false;
                return;
            }

            localPosition = VectorOperator.getLocalPosition(downDetailParent, springDownDetail.position);
        }

        void LateUpdate()
        {
            if (!downDetailParent)
            {
                enabled = false;
                return;
            }
            springDownDetail.position = VectorOperator.getWordPosition(downDetailParent, localPosition);
            springDownDetail.LookAt(springUpDetail.position);
            springUpDetail.LookAt(springDownDetail.position);

            float deltaDistance = Vector3.Distance(downPivot.position, upPivot.position) / detailsStartDistance;

            spring.localScale = new Vector3(springStartScale.x, springStartScale.y, springStartScale.z * deltaDistance);
        }
        public void TryToSetFromEditorWindow (Transform springDownDetail, Transform springUpDetail, Transform downPivot, Transform upPivot, Transform spring)
        {
            this.springDownDetail = springDownDetail;
            this.springUpDetail = springUpDetail;
            this.downPivot = downPivot;
            this.upPivot = upPivot;
            this.spring = spring;
        }
        public void TryToSet (Transform absorber)
        {
            foreach (var item in absorber.GetComponentsInChildren<Transform>())
            {
                if (item.name.Contains("DownDetail"))
                {
                    springDownDetail = item;
                }
                else if (item.name.Contains("UpDetail"))
                {
                    springUpDetail = item;
                }
                else if (item.name.Contains("DownPivot"))
                {
                    downPivot = item;
                } 
                else if (item.name.Contains("UpPivot"))
                {
                    upPivot = item;
                } 
                else if (item.name.Contains("Spring"))
                {
                    spring = item;
                } 
            }
        }
    }
}
