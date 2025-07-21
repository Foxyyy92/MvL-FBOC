using Quantum;
using UnityEngine;

public class LoopingMusicData : AssetObject {

#if QUANTUM_UNITY
    public UnityEngine.AudioClip FrontRunning;
    public UnityEngine.AudioClip fastFrontRunning;
    public UnityEngine.AudioClip Normal;
    public UnityEngine.AudioClip fastNormal;
    [Header("---Not Functional, Not Required---")]
    public UnityEngine.AudioClip UnderWater;
    public UnityEngine.AudioClip fastUnderWater;
    [Space]
#endif
    public float loopStartSeconds;
    public float loopEndSeconds;
    public float speedupFactor = 1.25f;

}