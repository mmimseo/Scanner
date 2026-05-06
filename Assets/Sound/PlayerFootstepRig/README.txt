PlayerFootstepRig (UdonSharp, VRChat World)
=============================================

플레이어가 걸으면 좌/우 발에 따라 발소리가 번갈아 재생되는 시스템.
- 머리 회전·정지 상태에서는 소리 안 남 (속도 기반 검출)
- 좌우 클립 풀 + Stereo Pan 으로 헤드폰에서 좌우 분리
- 피치 미세 흔들기 (기계적 반복 방지)

요구사항
--------
- Unity 2022.3.x
- VRChat SDK Worlds (UdonSharp 포함)


설치 (3단계)
------------
1. 이 zip 을 풀면 PlayerFootstepRig 폴더가 나옵니다.
   그 폴더 통째로 자기 프로젝트의 Assets 폴더 안 어디든 넣어주세요.
   예: Assets/PlayerFootstepRig/

2. Unity 가 자동으로 import + 컴파일.
   (혹시 Inspector 에 Player Footsteps 컴포넌트가 안 보이면
    Window > UdonSharp > Refresh All UdonSharpProgramAssets 한 번 실행)

3. Project 창에서 Player_FootstepRig.prefab 을 찾아 씬의 Hierarchy 로
   드래그하면 끝.

   ▶ Play 모드에서 WASD 로 걸어보면 발소리가 들려야 합니다.


튜닝 (Inspector 에서)
---------------------
Step Distance     : 한 발 디딜 때 거리 (걷기 0.7~0.9 / 달리기 1.0~1.3)
Min Speed         : 이 속도 이하면 무음 (기본 0.3 m/s)
Volume            : 발소리 볼륨
Pitch Variation   : 매 발소리 피치 흔드는 폭 (0~0.3)
Stereo Pan        : 좌우 분리 강도 (0=중앙, 1=완전 좌우)
Follow Player     : AudioSource 를 매번 플레이어 위치로 이동


폴더 구성
---------
PlayerFootstepRig/
├── Player_FootstepRig.prefab     <- 씬에 드래그
├── Scripts/
│   ├── PlayerFootsteps.cs        <- 메인 로직 (수정 가능)
│   └── PlayerFootsteps.asset     <- UdonSharp 컴파일 결과 (수정 X)
└── Audio/
    ├── leftFoot/   scanner_leftfoot1~4.wav
    └── rightFoot/  scanner_rightfoot1~4.wav
