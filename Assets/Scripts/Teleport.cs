using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class Teleport : UdonSharpBehaviour
{
    [Header("Teleport")]
    public Transform flippedSpawn;

    [Header("Active Objects")]
    public GameObject Shader;
    public GameObject InteractBox;

    [Header("Audio Disable")]
    public AudioSource[] audioSourcesToDisable;

    [Header("Audio Replace")]
    public AudioSource targetAudioSource;
    public AudioClip newClip;
    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (!player.isLocal) return;

        player.TeleportTo(
            flippedSpawn.position,
            flippedSpawn.rotation,
            VRC_SceneDescriptor.SpawnOrientation.AlignRoomWithSpawnPoint,
            false
        );

        // Interaction1
        if (Shader != null)
            Shader.SetActive(true);

        if (InteractBox != null)
            InteractBox.SetActive(true);

        if (audioSourcesToDisable != null)
        {
            foreach (AudioSource src in audioSourcesToDisable)
            {
                if (src != null)
                    src.Stop();
            }
        }

        if (targetAudioSource != null && newClip != null)
        {
            targetAudioSource.Stop();
            targetAudioSource.clip = newClip;
            targetAudioSource.Play();
        }
    }
}