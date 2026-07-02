using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{
    public static SoundManagerScript Instance { get; set; }

    [Header("Units")]
    public AudioClip infantryattackClip;
    public AudioClip infantryDeathClip;
    public AudioClip infantryMoveClip;

    private AudioSource infantryattackChannel;
    private AudioSource infantryDeathChannel;
    private AudioSource infantryMoveChannel;

    [Header("Buildings")]
    public AudioClip buildingDestroyClip;
    public AudioClip buildingConstructClip;
    public AudioClip sellBuildingClip;
    private AudioSource constructBuildingChannel;
    private AudioSource destructionBuildingChannel;
    private AudioSource sellBuildingChannel;

    void Awake()
    {
        if (Instance != null & Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        infantryattackChannel = gameObject.AddComponent<AudioSource>();
        infantryattackChannel.volume = 0.1f;
        infantryattackChannel.playOnAwake = false;

        infantryDeathChannel = gameObject.AddComponent<AudioSource>();
        infantryDeathChannel.volume = 0.2f;
        infantryDeathChannel.playOnAwake = false;

        infantryMoveChannel = gameObject.AddComponent<AudioSource>();
        infantryMoveChannel.volume = 0.5f;
        infantryMoveChannel.playOnAwake = false;

        constructBuildingChannel = gameObject.AddComponent<AudioSource>();
        constructBuildingChannel.volume = 0.1f;
        constructBuildingChannel.playOnAwake = false;

        destructionBuildingChannel = gameObject.AddComponent<AudioSource>();
        destructionBuildingChannel.volume = 0.1f;
        destructionBuildingChannel.playOnAwake = false;

        sellBuildingChannel = gameObject.AddComponent<AudioSource>();
        sellBuildingChannel.volume = 0.5f;
        sellBuildingChannel.playOnAwake = false;
    }

    public void PlayinfantryattackSound()
    {
        if (infantryattackChannel && infantryattackChannel.isPlaying == false)
        {
            infantryattackChannel.PlayOneShot(infantryattackClip);
        }
    }

    public void PlayinfantryDeathSound()
    {
        if (infantryDeathChannel && infantryDeathChannel.isPlaying == false)
        {
            infantryDeathChannel.PlayOneShot(infantryDeathClip);
        }
    }

    public void PlayinfantryMoveSound()
    {
        if (infantryMoveChannel && infantryMoveChannel.isPlaying == false)
        {
            infantryMoveChannel.PlayOneShot(infantryMoveClip);
        }
    }

    public void PlayBuildingDestroySound()
    {
        if (destructionBuildingChannel && destructionBuildingChannel.isPlaying == false)
        {
            destructionBuildingChannel.PlayOneShot(buildingDestroyClip);
        }
    }

    public void PlayBuildingConstructionSound()
    {
        if (constructBuildingChannel && constructBuildingChannel.isPlaying == false)
        {
            constructBuildingChannel.PlayOneShot(buildingConstructClip);
        }
    }

    public void PlaySellingBuildingSound()
    {
        if (sellBuildingChannel && sellBuildingChannel.isPlaying == false)
        {
            sellBuildingChannel.PlayOneShot(sellBuildingClip);
        }
    }
}
