using UnityEngine;
//using Yudiz.CarVR.CoreGamePlay;

namespace Yudiz.DirtBikeVR.Camera
{
    public class CameraFollow : MonoBehaviour
    {
        #region PUBLIC_VARS           
        [SerializeField] private Transform target;
        #endregion

        #region PRIVATE_VARS        
        #endregion

        #region UNITY_CALLBACKS
        private void Start()
        {
            SetParentToBikeView();
        }
        #endregion

        #region STATIC_FUNCTIONS
        #endregion

        #region PUBLIC_FUNCTIONS        
        private void SetParentToBikeView()
        {
            transform.SetParent(target,false);            
        }       
        #endregion

        #region PRIVATE_FUNCTIONS        
        #endregion

        #region CO-ROUTINES
        #endregion

        #region EVENT_HANDLERS
        #endregion

        #region UI_CALLBACKS
        #endregion
    }
}