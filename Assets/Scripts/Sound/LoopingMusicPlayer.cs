using NSMB.UI.Options.Loaders;
using System.Diagnostics.Eventing.Reader;
using UnityEngine;

namespace NSMB.Sound {
    public class LoopingMusicPlayer : MonoBehaviour {

        //---Properties
        public bool IsPlaying => audioSource.isPlaying;
        public float AudioStart => currentAudio.loopStartSeconds * CurrentSpeedupFactor;
        public float AudioEnd => currentAudio.loopEndSeconds * CurrentSpeedupFactor;
        private LoopingMusicData CurrentMusicSong => currentAudio;
        private float CurrentSpeedupFactor => FastMusic ? 1f / (CurrentMusicSong?.speedupFactor ?? 1f) : 1f;

        //---Serialized Variables
        [SerializeField] protected AudioSource audioSource;
        [SerializeField] protected LoopingMusicData currentAudio;
        [SerializeField] private bool playOnAwake = true;

        public void OnEnable() {
            if (playOnAwake && currentAudio) {
                Play(currentAudio, true);
            }
        }

        public void Update() {
            if (!audioSource.isPlaying || !currentAudio) {
                return;
            }

            if (currentAudio.loopEndSeconds != -1) {
                float time = audioSource.time;

                if (time >= AudioEnd) {
                    audioSource.time = AudioStart + (time - AudioEnd);
                }
            }
        }

        public void Restart() {
            if (currentAudio) {
                Play(currentAudio, true);
            }
        }

        public void Unpause() {
            audioSource.UnPause();
        }

        public void Pause() {
            audioSource.Pause();
        }

        public void Stop() {
            audioSource.Stop();
        }

        //---Properties
        private bool _fastMusic;
        public bool FastMusic {
            set {
                if (_fastMusic == value) {
                    return;
                }

                _fastMusic = value;

                if (!CurrentMusicSong) {
                    return;
                }

                float scaleFactor = CurrentMusicSong.speedupFactor;
                if (_fastMusic) {
                    scaleFactor = 1f / scaleFactor;
                }

                float time = audioSource.time;
                audioSource.clip = GetMusicType(CurrentMusicSong, _fastMusic);
                audioSource.Play();
                audioSource.time = time * scaleFactor;

                Update();
            }
            get => _fastMusic && CurrentMusicSong && (CurrentMusicSong.fastNormal || CurrentMusicSong.fastFrontRunning || CurrentMusicSong.fastUnderWater);
        }
        private bool _Frontrunning;
        public bool Frontrunning {
            set {
                if (_Frontrunning == value) {
                    return;
                }

                _Frontrunning = value;

                if (!CurrentMusicSong) {
                    return;
                }

                float time = audioSource.time;
                audioSource.clip = GetMusicType(CurrentMusicSong, _fastMusic);
                audioSource.Play();
                audioSource.time = time;

                Update();
            }
            get => _Frontrunning && CurrentMusicSong && (CurrentMusicSong.fastNormal || CurrentMusicSong.fastFrontRunning || CurrentMusicSong.fastUnderWater);
        }


        public void SetSoundData(LoopingMusicData data) {
            currentAudio = data;
            audioSource.clip = GetMusicType(data, _fastMusic);
        }

        public void Play(LoopingMusicData song, bool restartIfAlreadyPlaying = false) {
            if (currentAudio == song && audioSource.isPlaying && !restartIfAlreadyPlaying) {
                return;
            }

            currentAudio = song;
            audioSource.loop = true;
            if (CurrentMusicSong) {
                audioSource.clip = GetMusicType(CurrentMusicSong, _fastMusic);
            } else {
                audioSource.clip = GetMusicType(CurrentMusicSong, false);
            }
            audioSource.Play();
        }

        public AudioClip GetMusicType(LoopingMusicData song, bool allowfast) {
            if (_Frontrunning) {
                //Frontrunning
                return (allowfast && song.fastFrontRunning) ? song.fastFrontRunning : song.FrontRunning;
            } else if (false && song.UnderWater != null) {
                //Underwater
                return (allowfast && song.fastUnderWater) ? song.fastUnderWater : song.UnderWater;
            } else {
                //Normal
                return (allowfast && song.fastNormal) ? song.fastNormal : song.Normal;
            }
        }
    }
}