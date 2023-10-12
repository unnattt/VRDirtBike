using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayFps : MonoBehaviour
{
    public float updateDelay = 0f;

    private float _targetFPS = 72f;
    private float _currentFPS = 0f;
    private float _deltaTime = 0f;

    [SerializeField] private TMP_Text _textFPS;
    // Start is called before the first frame update
    void Start()
    {
        //_textFPS = GetComponent<TextMeshProUGUI>();
        StartCoroutine(DisplayFramesPerSecond());
    }

    // Update is called once per frame
    void Update()
    {

        GenerateFramesPerSecond();
    }

    private void GenerateFramesPerSecond()
    {
        _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * .1f;
        _currentFPS = 1.0f / _deltaTime;
    }

    private IEnumerator DisplayFramesPerSecond()
    {
        while (true)
        {
            if (_currentFPS >= _targetFPS)
            {
                _textFPS.color = new Color32(0, 177, 215, 255);
            }
            else
            {
                _textFPS.color = new Color32(200, 68, 124, 255);
            }
            _textFPS.text = "FPS: " + _currentFPS.ToString(".0");
            yield return new WaitForSeconds(updateDelay);
        }

    }
}
