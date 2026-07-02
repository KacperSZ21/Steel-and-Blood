using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenuController : MonoBehaviour
{
    public GameObject gameMenu;

    private GameObject cameraControl;
    private float previousTimeScale = 1f;
    private float previousFixedDeltaTime = 0.02f;
    private bool isPaused = false;
    // Start is called before the first frame update
    void Start()
    {
        cameraControl = GameObject.Find("CameraController");
        previousTimeScale = Time.timeScale;
        previousFixedDeltaTime = Time.fixedDeltaTime;
        if (gameMenu)
        {
            gameMenu.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!isPaused)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }
    }

    public void PauseGame(bool gamemenuenable = true)
    {
        isPaused = true;
        previousTimeScale = Time.timeScale;
        previousFixedDeltaTime = Time.fixedDeltaTime;

        Time.timeScale = 0f;
        Time.fixedDeltaTime = 0f;
        AudioListener.pause = true;

        Cameracontroll.Instance.ResetCameraCursor();
        var script = cameraControl.GetComponent<Cameracontroll>();
        script.enabled = false;
        if (gamemenuenable)
        {
            gameMenu.SetActive(true);
        }
        UnitSelectionManager.Instance.RememberSelectedUnits();
    }

    public void ResumeGame(bool gamemenudisable = true)
    {
        isPaused = false;
        Time.timeScale = previousTimeScale;
        Time.fixedDeltaTime = previousFixedDeltaTime;
        AudioListener.pause = false;

        var script = cameraControl.GetComponent<Cameracontroll>();
        script.enabled = true;
        if (gamemenudisable)
        {
            gameMenu.SetActive(false);
        }
        if (GameObject.Find("MasterPanel") != null)
        {
            GameObject.Find("MasterPanel").SetActive(false);
        }
        UnitSelectionManager.Instance.ReselectRememberedUnits();
    }
}
