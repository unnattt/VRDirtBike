using System.Collections;
using UnityEngine;
using Yudiz.DirtBikeVR.Managers;

namespace Yudiz.DirtBikeVR.CoreGamePlay
{
    public class CarAudio : MonoBehaviour
    {
        #region PUBLIC_VARS
        #endregion

        #region PRIVATE_VARS
        private CarController carController;
        private AudioSource audioSource;

        private float minSpeed = 0.1f;
        private float maxSpeed;
        private float minPitch = 0.5f;
        private float maxPitch = 1.5f;

        private bool isEngineRunning;
        #endregion

        #region UNITY_CALLBACKS
        private void OnEnable()
        {
            InputController.OnCarStart += StartCarSound;
        }

        private void OnDisable()
        {
            InputController.OnCarStart -= StartCarSound;
        }

        private void Start()
        {
            carController = GetComponent<CarController>();
            audioSource = AudioManager.instance.audioSource;
            maxSpeed = carController.maxSpeed;                        
        }

        private void Update()
        {
            if (audioSource != null && isEngineRunning)
            {
                EngineSound();
            }
        }
        #endregion

        #region PUBLIC_FUNCTIONS
        #endregion

        #region PRIVATE_FUNCTIONS

        private void StartCarSound()
        {
            InputController.OnCarStart = null;
            StartCoroutine(SoundCooldown());
        }

        private void EngineSound()
        {
            float currentSpeed = carController.CarSpeedRigidBody();
            float carPitch = currentSpeed / maxSpeed;
            //Debug.Log("CarPitch: " + carPitch);
            if (currentSpeed < minSpeed)
            {
                audioSource.pitch = minPitch;
            }

            if (currentSpeed > minSpeed && currentSpeed < maxSpeed)
            {
                audioSource.pitch = minPitch + carPitch;
            }

            if (currentSpeed > maxSpeed)
            {
                audioSource.pitch = maxPitch;
            }
        }
        #endregion

        #region CO-ROUTINES
        IEnumerator SoundCooldown()
        {           
            AudioManager.instance.PlaySound(AudioTrack.BikeStart, false);
            yield return new WaitForSeconds(0.5f);
            AudioManager.instance.PlaySound(AudioTrack.BikeEngine, true);
            isEngineRunning = true;            
        }
        #endregion

        #region EVENT_HANDLERS
        #endregion

        #region UI_CALLBACKS
        #endregion
    }
}
