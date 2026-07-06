using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ErrorManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject errorPanel;
    [SerializeField] private TMP_Text errorText;
    [SerializeField] private Button copyButton;
    [SerializeField] private Button resumeButton;

    private string lastErrorMessage;
    private bool errorActive = false;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        errorPanel.SetActive(false);

        copyButton.onClick.AddListener(CopyErrorToClipboard);
        resumeButton.onClick.AddListener(ResumeGame);
    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (errorActive)
            return;

        if (type == LogType.Error || type == LogType.Exception)
        {
            errorActive = true;

            string codeLine = ExtractFirstCodeLine(stackTrace);

            lastErrorMessage =
                $"TYPE: {type}\n\n" +
                $"MESSAGE:\n{logString}\n\n" +
                $"LOCATION:\n{codeLine}\n\n" +
                $"FULL STACK TRACE:\n{stackTrace}";

            ShowError(lastErrorMessage);
        }
    }


    private void ShowError(string message)
    {
        Time.timeScale = 0f;

        errorText.text = message;
        errorPanel.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private string ExtractFirstCodeLine(string stackTrace)
    {
        if (string.IsNullOrEmpty(stackTrace))
            return "No stack trace available";

        string[] lines = stackTrace.Split('\n');

        foreach (string line in lines)
        {
            if (line.Contains("(at "))
                return line.Trim();
        }

        return "No code line found";
    }


    private void ResumeGame()
    {
        Time.timeScale = 1f;
        errorPanel.SetActive(false);

        errorActive = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void CopyErrorToClipboard()
    {
        GUIUtility.systemCopyBuffer = lastErrorMessage;
        Debug.Log("Error copied to clipboard");
    }
}