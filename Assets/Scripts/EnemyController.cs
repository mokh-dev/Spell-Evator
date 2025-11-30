using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class EnemyController : MonoBehaviour
{
    private GameObject player;
    public TextMeshPro promptText;
    public float speed;
    [SerializeField] private float killTime;
    [SerializeField] private float speedMultiplier;

    public string OGPromptText;

    public string Prompt;

    private bool hasShownPrompt;

    private Rigidbody2D rb;

    public float spawnDist;

    private SpriteRenderer sr;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        promptText = transform.GetChild(0).GetComponent<TextMeshPro>();
        rb = gameObject.GetComponent<Rigidbody2D>();   
        sr = gameObject.GetComponent<SpriteRenderer>();   
    }

    void Update()
    {

        /*
        if (playerDirVector.y > 0)
        {
            //Going UP
            rb.velocity = playerDirVector * (speed - ((risingSpeed + risingSpeedBelowBoost) * Mathf.Abs(playerDirVector.y)));
        }
        else
        {
            //Going Down
            rb.velocity = playerDirVector * (speed + (risingSpeed * Mathf.Abs(playerDirVector.y)));
        }*/

        rb.velocity = (player.transform.position - gameObject.transform.position).normalized * speed * Mathf.Sqrt(Mathf.Abs(spawnDist)) * speedMultiplier;

        if (transform.position.x < 0)
        {
            sr.flipX = false;
        }
        else
        {
            sr.flipX = true;
        }
    }


    public void ShowPrompt()
    {
        promptText.color = Color.yellow;
    }

    public void Unfocus()
    {
        promptText.color = Color.white;
        promptText.text = OGPromptText;
    }

    public void RemoveLetter()
    {
        if (promptText.text.Length > 1)
        {
            promptText.text = promptText.text.Substring(1);
        }
    }

    public void RestorePrompt()
    {
        promptText.text = OGPromptText;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Shield"))
        {
            if (gameObject.CompareTag("Enemy"))
            {   
                player.GetComponent<PlayerInputManager>().SpawnShieldParticles();
                player.GetComponent<PlayerInputManager>().PlayShieldSound();
                Destroy(gameObject);
            }
        }
    }
}
