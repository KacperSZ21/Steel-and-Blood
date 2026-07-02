using UnityEngine;
using Cinemachine;
using UnityEngine.Playables;

public class CameraPathSequenceController : MonoBehaviour
{
    [Header("Kamery z wózkami w kolejności od 0 do N")]
    public CinemachineDollyCart[] carts;

    [Header("Timeline do resetowania")]
    public PlayableDirector timeline;

    [Header("Płynne przejścia")]
    public float blendTime = 1.0f; // transition time between cameras
    public WhiteScreenFader fader;


    private int currentIndex = 0;
    private CinemachineBrain brain;

    void Start()
    {
        brain = Camera.main.GetComponent<CinemachineBrain>();

        if (brain != null)
        {
            // Set a global transition definition
            brain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseInOut, blendTime);
        }

        ActivateCamera(0);
        timeline.Play();
    }

    void Update()
    {
        if (carts.Length == 0)
            return;

        var currentCart = carts[currentIndex];

        if (currentCart.m_Position >= currentCart.m_Path.PathLength)
        {
            GoToNextCamera();
            StartCoroutine(fader.FadeWhite());
        }
    }

    void GoToNextCamera()
    {
        StartCoroutine(fader.FadeWhite());  // <<< Fade to white

        SetCameraPriority(carts[currentIndex], 0);

        currentIndex++;

        if (currentIndex >= carts.Length)
            currentIndex = 0;

        ActivateCamera(currentIndex);

        ResetTimeline();
    }


    void ActivateCamera(int index)
    {
        for (int i = 0; i < carts.Length; i++)
            SetCameraPriority(carts[i], (i == index) ? 10 : 0);

        carts[index].m_Position = 0f;
    }

    void SetCameraPriority(CinemachineDollyCart cart, int priority)
    {
        var cam = cart.GetComponentInChildren<CinemachineVirtualCamera>();
        if (cam != null)
            cam.Priority = priority;
    }

    void ResetTimeline()
    {
        timeline.time = 0;
        timeline.Evaluate();
        timeline.Play();
    }
}