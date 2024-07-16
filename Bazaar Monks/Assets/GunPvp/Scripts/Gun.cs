using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using easyInputs;

public class Gun : MonoBehaviour
{
    [Header("This Script Was Made By Vex")]

    public GameObject shootpoint;
    public PhotonView view;
    public GameObject Bullet;
    public GameObject shootParticle;
    public AudioSource shootSound;
    public GameObject casing;
    public Transform casingEjectionPoint;
    public float bulletSpeed = 10f;
    public float lifetime = 2f;
    public float shootCooldown = 0.5f;

    private float lastShootTime = 0f;

    private void Start()
    {
        view = GetComponent<PhotonView>();
    }

    public void Update()
    {
        if (EasyInputs.GetTriggerButtonDown(EasyHand.RightHand) || (Input.GetButtonDown("Fire1")) && CanShoot())
        {
            Shoot();
        }
    }

    private bool CanShoot()
    {
        return Time.time - lastShootTime >= shootCooldown;
    }

    public void Shoot()
    {
        if (view.IsMine)
        {
            GameObject shootparticle = PhotonNetwork.Instantiate(shootParticle.gameObject.name, shootpoint.transform.position, Quaternion.identity, 0);

            view.RPC("ShootSound", RpcTarget.All);

            lastShootTime = Time.time;

            GameObject bullet = PhotonNetwork.Instantiate(Bullet.gameObject.name, shootpoint.transform.position, Quaternion.identity, 0);

            Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
            if (bulletRigidbody == null)
            {
                bulletRigidbody = bullet.AddComponent<Rigidbody>();
            }

            if (casing != null && casingEjectionPoint != null)
            {
                GameObject Casing = PhotonNetwork.Instantiate(casing.gameObject.name, casingEjectionPoint.position, Quaternion.identity, 0);
                Rigidbody casingRB = Casing.GetComponent<Rigidbody>();
                if (casingRB != null)
                {
                    float ejectForce = Random.Range(1.5f, 2f);
                    float ejectTorque = Random.Range(0.5f, 1f);
                    casingRB.AddForce(casingEjectionPoint.right * ejectForce, ForceMode.Impulse);
                    casingRB.AddTorque(Vector3.up * ejectTorque, ForceMode.Impulse);
                }
                StartCoroutine(DestroyCasing(Casing, lifetime));
            }

            bulletRigidbody.velocity = transform.forward * bulletSpeed;
            StartCoroutine(DestroyBullet(bullet, lifetime));
            StartCoroutine(DestroyShootParticle(shootparticle, lifetime));
        }
    }

    private IEnumerator DestroyBullet(GameObject bullet, float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        PhotonNetwork.Destroy(bullet);
    }

    private IEnumerator DestroyShootParticle(GameObject shootparticle, float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        PhotonNetwork.Destroy(shootparticle);
    }

    private IEnumerator DestroyCasing(GameObject Casing, float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        PhotonNetwork.Destroy(Casing);
    }

    [PunRPC]
    public void ShootSound()
    {
        shootSound.Play();
    }
}