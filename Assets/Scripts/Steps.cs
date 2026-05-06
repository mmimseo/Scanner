using UnityEngine;
using UdonSharp;
using VRC.SDKBase;

[AddComponentMenu("Udon Sharp/Utilities/Steps")]
[UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
public class Steps : UdonSharpBehaviour
{
    [Tooltip("발소리를 재생할 AudioSource")]
    public AudioSource audioSource;

    [Tooltip("왼발 발소리")]
    public AudioClip[] leftFootClips;

    [Tooltip("오른발 발소리")]
    public AudioClip[] rightFootClips;

    [Tooltip("한 발 디딜 때 이동 거리")]
    public float stepDistance = 0.8f;

    [Tooltip("이 속도 이하면 발소리 없음")]
    public float minSpeed = 0.3f;

    [Tooltip("볼륨")]
    [Range(0f, 1f)]
    public float volume = 0.7f;

    [Tooltip("피치 랜덤 범위")]
    [Range(0f, 0.3f)]
    public float pitchVariation = 0.05f;

    [Tooltip("좌우 스테레오")]
    [Range(0f, 1f)]
    public float stereoPan = 0.3f;

    [Tooltip("플레이어 위치 따라갈지")]
    public bool followPlayer = true;

    private VRCPlayerApi localPlayer;
    private float distanceAccumulator;
    private bool initialized;
    private bool nextIsLeft = true;

    void Start()
    {
        localPlayer = Networking.LocalPlayer;
        if (localPlayer != null)
        {
            initialized = true;
        }

        if (audioSource != null)
        {
            audioSource.spatialBlend = 0f;
            audioSource.volume = volume;
        }
    }

    void Update()
    {
        if (!initialized) return;
        if (localPlayer == null) return;
        if (audioSource == null) return;

        bool hasLeft = leftFootClips != null && leftFootClips.Length > 0;
        bool hasRight = rightFootClips != null && rightFootClips.Length > 0;
        if (!hasLeft && !hasRight) return;

        Vector3 velocity = localPlayer.GetVelocity();
        Vector3 horizontalVel = new Vector3(velocity.x, 0f, velocity.z);
        float speed = horizontalVel.magnitude;

        if (speed < minSpeed)
        {
            distanceAccumulator = 0f;

            if (audioSource.isPlaying)
                audioSource.Stop();

            return;
        }

        if (!localPlayer.IsPlayerGrounded())
        {
            return;
        }

        distanceAccumulator += speed * Time.deltaTime;

        if (distanceAccumulator >= stepDistance)
        {
            distanceAccumulator = 0f;
            PlayFootstep();
        }
    }

    private void PlayFootstep()
    {
        AudioClip[] pool = nextIsLeft ? leftFootClips : rightFootClips;

        if (pool == null || pool.Length == 0)
        {
            pool = nextIsLeft ? rightFootClips : leftFootClips;
            if (pool == null || pool.Length == 0) return;
        }

        int idx = Random.Range(0, pool.Length);
        AudioClip clip = pool[idx];

        if (clip == null)
        {
            nextIsLeft = !nextIsLeft;
            return;
        }

        if (followPlayer)
        {
            audioSource.transform.position = localPlayer.GetPosition();
        }

        audioSource.panStereo = nextIsLeft ? -stereoPan : stereoPan;

        if (pitchVariation > 0f)
        {
            audioSource.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
        }
        else
        {
            audioSource.pitch = 1f;
        }

        // 🔥 겹침 방지 재생 방식
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();

        nextIsLeft = !nextIsLeft;
    }
}