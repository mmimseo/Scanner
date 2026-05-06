using UdonSharp;
using UnityEngine;
using UnityEngine.Playables;
using VRC.SDKBase;

public class TimelineTrigger : UdonSharpBehaviour
{
    [Header("Timeline")]
    public PlayableDirector playableDirector;

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (!player.isLocal) return;

        if (playableDirector != null)
            playableDirector.Play();
    }
}