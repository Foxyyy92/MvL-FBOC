using UnityEngine;
using Photon.Pun;
using NSMB.Utils;

public class Thwomp : KillableEntity {
    [SerializeField] float speed, deathTimer = -1, ThumpTimer = 0, Y = 0, YSpot = 0;
    [SerializeField] bool Thumpin, Thumped;

    public new void Start() {
        base.Start();
        body.velocity = new Vector2(0, 0);
        Y = 0;
        YSpot = body.position.y;
    }

    public new void FixedUpdate() {
        if (GameManager.Instance && GameManager.Instance.gameover) {
            body.velocity = Vector2.zero;
            body.angularVelocity = 0;
            animator.enabled = false;
            body.isKinematic = true;
            return;
        }

        base.FixedUpdate();
        if (dead) {
            Y = 0;
            body.isKinematic = true;
            body.velocity = new Vector2(body.velocity.x, body.velocity.y - 0.2f);
            if (deathTimer >= 0 && (photonView?.IsMine ?? true)) {
                Utils.TickTimer(ref deathTimer, 0, Time.fixedDeltaTime);
                if (deathTimer == 0)
                    PhotonNetwork.Destroy(gameObject);
            }
            return;
        }

            Collider2D closest = null;
            Vector2 closestPosition = Vector2.zero;
            float distance = float.MaxValue;
            foreach (var hit in Physics2D.OverlapCircleAll(body.position, 8f)) {
                if (!hit.CompareTag("Player"))
                    continue;
                Vector2 actualPosition = hit.attachedRigidbody.position + hit.offset;
                float tempDistance = Vector2.Distance(actualPosition, body.position);
                if (tempDistance > distance)
                    continue;
                distance = tempDistance;
                closest = hit;
                closestPosition = actualPosition;
            }
            if (!Thumpin && !Thumped && closest && (!(closestPosition.y > YSpot + 1) || !(closestPosition.y > YSpot - 16))) {
               if (Mathf.Abs(closestPosition.x - body.position.x) < 1) {
               Thumpin = true;
               animator.SetBool("Angry", true);
               animator.SetBool("Grrr", false);
               } else if (Mathf.Abs(closestPosition.x - body.position.x) < 2) {
               animator.SetBool("Grrr", true);
               } else {
               animator.SetBool("Grrr", false);
               }
            }
            if (Thumpin) {
                Utils.TickTimer(ref ThumpTimer, 0, -Time.fixedDeltaTime);
                if (ThumpTimer < 0.25) {
                } else {
                body.isKinematic = false;
                Y = Mathf.Clamp(Y - 0.5f, -8, 0);
                }
            }
            if (Thumped) {
                Utils.TickTimer(ref ThumpTimer, 0, -Time.fixedDeltaTime);
                if (ThumpTimer > 1) {
                    animator.SetBool("Angry", false);
                    animator.SetBool("Grrr", false);
                    Y = 2;
                }
                if (body.position.y > YSpot) {
                    body.isKinematic = true;
                    body.position = new Vector2(body.position.x, YSpot);
                    Thumped = false;
                    ThumpTimer = 0;
                    Y = 0;
                }
            }

        physics.UpdateCollisions();
        if (physics.onGround && Thumpin) {
            Thumped = true;
            Thumpin = false;
            ThumpTimer = 0;
            CameraController.ScreenShake = 0.25f;
        }
        body.velocity = new Vector2(0, Y);
    }

    [PunRPC]
    public override void InteractWithPlayer(PlayerController player) {
        if (player.state == Enums.PowerupState.BlueShell && (player.crouching || player.groundpound) && !player.inShell) {
            return;
        } else if (player.invincible > 0 || player.inShell || player.state == Enums.PowerupState.MegaMushroom) {
            PlaySound(Enums.Sounds.Enemy_Shell_Kick);
            photonView.RPC(nameof(SpecialKill), RpcTarget.All, player.body.velocity.x > 0, player.groundpound, player.StarCombo++);
        } else if (player.hitInvincibilityCounter <= 0) {
            player.photonView.RPC(nameof(PlayerController.Powerdown), RpcTarget.All, false);
        }
    }
}