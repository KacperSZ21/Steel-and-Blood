using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private UnityEngine.UI.Slider slider;
    [SerializeField] private List<GameObject> elementsToHide;
    [SerializeField] private List<GameObject> elementsToShow;

    private UnityEngine.AsyncOperation loadOperation;


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void StartLoading(int sceneIndex)
    {

        if (elementsToHide.Count != 0)
        {
            foreach (var element in elementsToHide)
            {
                element.SetActive(false);
            }
        }

        if (elementsToShow.Count != 0)
        {
            foreach (var element in elementsToShow)
            {
                element.SetActive(true);
            }
        }

        if (slider != null)
        {
            slider.value = 0;
        }

        Debug.Log("Started loading");

        StartCoroutine(LoadAsync(sceneIndex));
    }

    private IEnumerator LoadAsync(int sceneIndex)
    {
        loadOperation = SceneManager.LoadSceneAsync(sceneIndex);
        loadOperation.allowSceneActivation = false;
        Debug.Log("Async Started");
        while (loadOperation.progress < 0.9f)
        {
            float progress = loadOperation.progress / 0.9f;
            if (slider) slider.value = progress;
            yield return null;
        }

        // 100%
        if (slider) slider.value = 1f;

        // one frame gap
        yield return null;

        loadOperation.allowSceneActivation = true;
        ResetTimeScale();
    }

    public void ResetTimeScale()
    {
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = 0.02f;
        AudioListener.pause = false;
    }
}
