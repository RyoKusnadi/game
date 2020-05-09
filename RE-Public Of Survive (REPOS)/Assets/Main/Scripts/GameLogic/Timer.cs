using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
   [SerializeField] private Text uiText;
   [SerializeField] private float mainTimer;

    private float timer= 300f;
    private bool canCount = true;
    private bool doOnce = false;

    private void Update()
    {
        if(timer >= 0.0f && canCount)
        {
            timer -= Time.deltaTime;
            uiText.text = timer.ToString("F");
        }
        else if(timer <= 0.0f && !doOnce)
        {
            canCount = false;
            doOnce = true;
            uiText.text = "0.00";
            timer = 0.0f;
            SceneManager.LoadScene("End",LoadSceneMode.Additive);
        }
    }
}