using UnityEngine;
using TMPro;
using System.Collections;
using Photon.Pun;

public class Health : MonoBehaviourPunCallbacks
{
    [Header("This Script Was Made By Vex")]

    public int maxHealth = 100;
    public int currentHealth;
    public Transform respawnPoint;
    public GameObject gorillaPlayer;
    public TextMeshPro healthText;

    private Collider[] objectsWithColliders;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthText();      

        objectsWithColliders = FindObjectsOfType<Collider>();
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthText();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        StartCoroutine(Teleport());
        currentHealth = maxHealth;
        UpdateHealthText();
    }

    IEnumerator Teleport()
    {
        gorillaPlayer.GetComponent<Rigidbody>().isKinematic = true;
        foreach (Collider collider in objectsWithColliders)
        {
            collider.enabled = false;
        }
        gorillaPlayer.transform.position = respawnPoint.transform.position;
        yield return new WaitForSeconds(1.0f);
        foreach (Collider collider in objectsWithColliders)
        {
            collider.enabled = true;
        }
        gorillaPlayer.GetComponent<Rigidbody>().isKinematic = false;
    }

    private void UpdateHealthText()
    {
        healthText.text = "Health: " + currentHealth.ToString();
    }
}