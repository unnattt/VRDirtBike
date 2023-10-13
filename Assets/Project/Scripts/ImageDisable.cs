using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yudiz.DirtBikeVR.CoreGamePlay
{
    public class ImageDisable : MonoBehaviour
    {
        [SerializeField] private Canvas tuitorialImage;

        private void Start()
        {
            StartCoroutine(PopUps());
        }

        IEnumerator PopUps()
        {
            //tuitorialImage.enabled = true;
            yield return new WaitForSeconds(8f);
            tuitorialImage.enabled = false;
            //Time.timeScale = 0f;
        }
    }
}
