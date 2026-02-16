using Quantum;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

namespace NSMB.Entities.World {
    public unsafe class PurpleCoinAnimator : QuantumEntityViewComponent {

        //---Serialized Variables
        [SerializeField] private GameObject CoinSparklePrefab;
        private List<GameObject> CoinSparklePooling = new();

        public List<SpriteRenderer> coinrenderers;

        [SerializeField] float speed = 9;
        [SerializeField] private Sprite[] Sprites;
        private float timer;
        private int SpriteNumber;
        public void Start() {
            QuantumEvent.Subscribe<EventMarioCollectedPurpleCoin>(this, OnCoinCollected);
            QuantumEvent.Subscribe<EventResetPurpleCoins>(this, OnStageReset);
        }

        public void Update() {
            timer += Time.deltaTime * speed;
            if (timer > 1) {
                timer -= 1;
                SpriteNumber++;
                if (SpriteNumber >= Sprites.Length) {
                    SpriteNumber = 0;
                }
                foreach (var coin in coinrenderers) {
                    coin.sprite = Sprites[SpriteNumber];
                }
            }
        }

        public void OnStageReset(EventResetPurpleCoins e) {
            foreach (var coin in coinrenderers) {
                coin.enabled = true;
            }
        }

        private void OnCoinCollected(EventMarioCollectedPurpleCoin e) {
            int i = e.CoinNumber;
            coinrenderers[i].enabled = false;

            foreach (var particleobject in CoinSparklePooling) {
                if (!particleobject.activeSelf) {
                    //Pool
                    particleobject.SetActive(true);
                    particleobject.transform.position = e.Pos.ToUnityVector3();
                    return;
                }
            }
            //Add New Object To Pool
            var newparticle = Instantiate(CoinSparklePrefab, e.Pos.ToUnityVector3(), Quaternion.identity);
            newparticle.SetActive(true);
            newparticle.transform.SetParent(gameObject.transform, true);
            CoinSparklePooling.Add(newparticle);
        }
    }
}