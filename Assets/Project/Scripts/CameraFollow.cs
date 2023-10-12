using UnityEngine;
using Yudiz.DirtBikeVR.CoreGamePlay;

namespace Yudiz.DirtBikeVR.Camera
{
    public class CameraFollow : MonoBehaviour
    {
        #region PUBLIC_VARS
        private Transform target;
        #endregion

        #region PRIVATE_VARS        
        #endregion

        #region UNITY_CALLBACKS
        //private void LateUpdate()
        //{
        //    if(target == null) { return; }
        //    FollowTarget();
        //    //FollowTargetRotation();
        //}
        #endregion

        #region STATIC_FUNCTIONS
        #endregion

        #region PUBLIC_FUNCTIONS
        //public void AssignCarAsTargetToThisCamera(CarController c)
        //{
        //    target = c.gameObject.GetComponentInChildren<LookAroundCamera>().transform;

        //    //as target is assigned, turn on the camera
        //    gameObject.SetActive(true);
        //}

        public void SetParentToCarView()
        {
            transform.SetParent(target,false);
            
        }

        //public void AssignTargetTransform(Transform targetTransform)
        //{
        //    //Debug.Log($"Transform: {targetTransform}");
        //    //target = targetTransform.GetComponentInChildren<LookAroundCamera>().transform;
        //}
        #endregion

        #region PRIVATE_FUNCTIONS
        //private void FollowTarget()
        //{
        //    transform.position = target.position; //Vector3.Lerp(transform.position, target.position, Time.deltaTime);
        //}

        //private void FollowTargetRotation()
        //{
        //    transform.rotation = target.rotation;
        //}
        #endregion

        #region CO-ROUTINES
        #endregion

        #region EVENT_HANDLERS
        #endregion

        #region UI_CALLBACKS
        #endregion
    }
}