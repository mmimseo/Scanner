using UnityEngine;
using UdonSharp;
using VRC.SDKBase;

namespace UdonSharp.Examples.Utilities
{
    [AddComponentMenu("Udon Sharp/Utilities/Player Footsteps")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class PlayerFootsteps : UdonSharpBehaviour
    {
        [Tooltip("발소리를 재생할 AudioSource (이 스크립트와 같은 GameObject 또는 자식)")]
        public AudioSource audioSource;

        [Tooltip("왼발 발소리 클립 (여러 개 넣으면 랜덤). 비어 있으면 오른발 풀에서 폴백.")]
        public AudioClip[] leftFootClips;

        [Tooltip("오른발 발소리 클립 (여러 개 넣으면 랜덤). 비어 있으면 왼발 풀에서 폴백.")]
        public AudioClip[] rightFootClips;

        [Tooltip("한 발 디딜 때마다의 이동 거리 (m). 걷기 0.7~0.9 / 달리기 1.0~1.3 권장")]
        public float stepDistance = 0.8f;

        [Tooltip("이 속도 이하면 발소리 안남 (m/s)")]
        public float minSpeed = 0.3f;

        [Tooltip("발소리 볼륨")]
        [Range(0f, 1f)]
        public float volume = 0.7f;

        [Tooltip("재생마다 피치를 흔드는 폭 (0이면 고정)")]
        [Range(0f, 0.3f)]
        public float pitchVariation = 0.05f;

        [Tooltip("좌우 스테레오 분리 폭 (0=중앙, 1=완전 좌/우). 0.2~0.4 권장")]
        [Range(0f, 1f)]
        public float stereoPan = 0.3f;

        [Tooltip("AudioSource를 매 발소리마다 플레이어 위치로 이동시킬지")]
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

            audioSource.PlayOneShot(clip, volume);
            nextIsLeft = !nextIsLeft;
        }
    }
}
