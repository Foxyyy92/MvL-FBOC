using Quantum;
using UnityEngine;

public class LoopingMusicData : AssetObject {

#if QUANTUM_UNITY
    public AudioClip FrontRunning;
    public AudioClip fastFrontRunning;
    public AudioClip Normal;
    public AudioClip fastNormal;
    [Space]
#endif
    public float loopStartSeconds;
    public float loopEndSeconds;
    public float speedupFactor = 1.25f;

}