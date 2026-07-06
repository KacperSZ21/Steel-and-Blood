using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("Objects for Restart instances")]
    public WinorLose winorLose;
    public BuySystem buySystem;

    public void Start()
    {
        ResetTimeScale();
    }

    public void LoadLevel(int index)
    {
        ResetTimeScale();
        UnitSelectionManager.Instance.RestartUnitSelectionManager();
        winorLose.RestartWinORLosemanager();
        CursorManager.Instance.RestartCursorManager();
        Cameracontroll.Instance.ResetCameraCursor();
        buySystem.RestartBuySystem();
        ResourceManager.Instance.RestartResourceManager();
        SceneManager.LoadScene(index);
    }

    public void RestartLevel()
    {
        ResetTimeScale();
        UnitSelectionManager.Instance.RestartUnitSelectionManager();
        winorLose.RestartWinORLosemanager();
        CursorManager.Instance.RestartCursorManager();
        Cameracontroll.Instance.ResetCameraCursor();
        buySystem.RestartBuySystem();
        ResourceManager.Instance.RestartResourceManager();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void ResetTimeScale()
    {
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = 0.02f;
        AudioListener.pause = false;
    }

    public void AddResources()
    {
        ResourceManager.Instance.IncreaseResource(ResourceType.Gold, 1000);
        ResourceManager.Instance.IncreaseResource(ResourceType.Wood, 1000);
    }
}