using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Photon.Pun;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ShooterPlayer : MonoBehaviourPun
{
    [SerializeField] float speed;
    [SerializeField] TMP_Text playerName;
    [SerializeField] float MaxHealth = 100;
    [SerializeField] float RestoreValue = 10;
    [SerializeField] float DamageValue = 10;
    [SerializeField] float health = 10;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] ShooterBullet bulletPrefab;
    [SerializeField] Animator animator;
    // [SerializeField] SpriteRenderer rend;
    // [SerializeField] Sprite[] avatarSprites
    [SerializeField] Renderer rend;
    [SerializeField] Texture[] avatarTextures;
    Vector2 moveDir;
    private void Start()
    {
        playerName.text = photonView.Owner.NickName + $"({health})";
        if (photonView.Owner.CustomProperties.TryGetValue(PropertyNames.Player.AvatarIndex, out var AvatarIndex))
            rend.material.mainTexture = avatarTextures[(int)AvatarIndex];
        // rend.sprite = avatarSprites[(int)AvatarIndex];
        // if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(PropertyNames.Room.MaxHealth, out var roomMaxHealth))
        // {
        // this.MaxHealth = (float)roomMaxHealth;
        // }
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(PropertyNames.Room.MaxHealth, out var roomMaxHealth))
        {
            this.MaxHealth = (float)roomMaxHealth;
            this.health = this.MaxHealth;
            playerName.text = photonView.Owner.NickName + $"({health})";
        }
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(PropertyNames.Room.RestoreValue, out var roomRestoreValue))
        {
            this.RestoreValue = (float)roomRestoreValue;
        }
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(PropertyNames.Room.DamageValue, out var roomDamageValue))
        {
            this.DamageValue = (float)roomDamageValue;
        }


    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine == false)
            return;

        moveDir = new(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        // transform.Translate(moveDir * Time.deltaTime * speed);

        if (moveDir == Vector2.zero)
            animator.SetBool("IsMove", false);
        else
            animator.SetBool("IsMove", true);

        if (Input.GetMouseButtonDown(0))
        {
            var mouseScreenPos = Input.mousePosition;
            var mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            mouseWorldPos = new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0);
            var directionVector = mouseWorldPos - this.transform.position;

            Fire(this.transform.position, directionVector.normalized, new PhotonMessageInfo());

            photonView.RPC("Fire", RpcTarget.Others, this.transform.position, directionVector.normalized);
        }
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     photonView.RPC("TakeDamage", RpcTarget.All, 1);
        // }
    }

    private void FixedUpdate()
    {
        if (photonView.IsMine == false)
            return;
        rb.velocity = moveDir * speed;
    }

    [PunRPC]
    public void Fire(Vector3 position, Vector3 direction, PhotonMessageInfo info)
    {
        float lag = (float)(PhotonNetwork.Time - info.SentServerTime);
        if (photonView.IsMine)
            lag = 0;
        var bullet = Instantiate(bulletPrefab);
        bullet.Set(this, position, direction, lag);
    }

    [PunRPC]
    public void TakeDamage()
    {
        health -= DamageValue;
        health = Mathf.Clamp(health, 0, MaxHealth);
        playerName.text = photonView.Owner.NickName + $"({health})";
        // GetComponent<SpriteRenderer>().DOColor(Color.red, 0.2f).SetLoops(1, LoopType.Yoyo).From();
        var sequence = DOTween.Sequence();
        sequence.Append(rend.material.DOColor(Color.red, 0.2f).SetLoops(1, LoopType.Yoyo).From());
        sequence.Append(rend.material.DOColor(Color.white, 0.1f));
    }

    public void RestoreHealth()
    {
        if (photonView.IsMine)
            photonView.RPC("RestoreHealthRPC", RpcTarget.AllViaServer);
    }


    [PunRPC]
    public void RestoreHealthRPC()
    {
        health += RestoreValue;
        health = Mathf.Clamp(health, 0, MaxHealth);
        playerName.text = photonView.Owner.NickName + $"({health})";
        // GetComponent<SpriteRenderer>().DOColor(Color.red, 0.2f).SetLoops(1, LoopType.Yoyo).From();
        var sequence = DOTween.Sequence();
        sequence.Append(rend.material.DOColor(Color.green, 0.2f).SetLoops(1, LoopType.Yoyo).From());
        sequence.Append(rend.material.DOColor(Color.white, 0.1f));
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            if (photonView.IsMine)
            {
                photonView.RPC("TakeDamage", RpcTarget.AllViaServer);
            }
        }
    }
}
