using Photon.Deterministic;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Quantum {
    [UnityEngine.Scripting.Preserve]
    public unsafe class PurpleCoinSystem : SystemMainThread, ISignalOnStageReset,
        ISignalOnEntityBumped {

        public override void OnInit(Frame f) {
            f.Context.Interactions.Register<PurpleCoin, MarioPlayer>(f, OnPurpleCoinMassMarioInteraction);
        }

        public override void Update(Frame f) {
        }

        private static void IncrementPurpleCoins(Frame f, EntityRef purplecoinEntity, bool isOnStage = true) {
            f.Unsafe.GetPointer<Interactable>(purplecoinEntity)->ColliderDisabled = true;

            f.Events.MarioCollectedPurpleCoin(f.Unsafe.GetPointer<PurpleCoin>(purplecoinEntity)->CoinNumber, f.Unsafe.GetPointer<Transform2D>(purplecoinEntity)->Position);

            if (f.Global->MaxPurpleCoins == 0) {
                //temp
                f.Global->MaxPurpleCoins = 3;
            } 

            f.Global->PurpleCoins++;
            if (f.Global->PurpleCoins >= f.Global->MaxPurpleCoins) {
                f.Global->PurpleCoins -= f.Global->MaxPurpleCoins;
                f.Signals.OnPurpleCoinSpawnStar();
            }

            Debug.Log("Increment Purple coins: " + f.Global->PurpleCoins + " Max: " + f.Global->MaxPurpleCoins);
        }

        #region Interactions
        public static void OnPurpleCoinMassMarioInteraction(Frame f, EntityRef purplecoinEntity, EntityRef marioEntity) {
            IncrementPurpleCoins(f, purplecoinEntity);
        }
        #endregion

        #region Signals
        public void OnStageReset(Frame f, QBoolean full) {
            var filter = f.Filter<PurpleCoin, Interactable>();
            while (filter.NextUnsafe(out EntityRef entity, out PurpleCoin* purplecoin, out Interactable* interactable)) {
                interactable->ColliderDisabled = false;
            }
            f.Events.ResetPurpleCoins();
        }


        public void OnEntityBumped(Frame f, EntityRef coinEntity, FPVector2 position, EntityRef bumpOwner, QBoolean fromBelow) {
            //Idk this is wip code
            /*if (!f.Unsafe.TryGetPointer(coinEntity, out Coin* coin)
                || !f.Unsafe.TryGetPointer(coinEntity, out Transform2D* transform)
                || coin->IsCollected
                || coin->UncollectableFrames > 0) {
                return;
            }

            if (coin->IsCurrentlyDotted) {
                if (coin->DottedChangeFrames == 0) {
                    coin->DottedChangeFrames = 30;
                }
                return;
            } else if (!coin->IsCollected && f.Unsafe.TryGetPointer(bumpOwner, out MarioPlayer* mario)) {
                if (f.Unsafe.TryGetPointer(coinEntity, out ObjectiveCoin* objectiveCoin)) {
                    bool sameTeam = ((mario->GetTeam(f) + 1) ?? int.MinValue) == objectiveCoin->UncollectableByTeam;
                    if (mario->IsDead || (sameTeam && (!mario->CanCollectOwnTeamsObjectiveCoins || objectiveCoin->SpawnedViaSelfDamage))) {
                        return;
                    }
                }

                f.Signals.OnMarioPlayerCollectedCoin(bumpOwner, coinEntity, transform->Position, false, false);

                if (coin->CoinType.HasFlag(CoinType.BakedInStage)) {
                    coin->IsCollected = true;
                    f.Events.CoinChangeCollected(coinEntity, *coin, true);
                } else {
                    f.Destroy(coinEntity);
                }
            }*/
        }
        #endregion
    }
}