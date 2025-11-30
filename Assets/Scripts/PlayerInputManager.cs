using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class PlayerInputManager : MonoBehaviour
{


    private WaveSystem waveSystem;
    private Animator anim;

    private int[] keyCodes;

    private string wordToType = "empty";

    private int letterTypedIndex;

    [SerializeField] private GameObject shieldAnchor;
    [SerializeField] private GameObject shield;
    [SerializeField] private SpriteRenderer elevatorSprite;

    private int health;
    [SerializeField] private Sprite[] elevatorDamagedSprites;
    [SerializeField] private float deathFallSpeed;
    [SerializeField] private float deathYEndLevel;

    private bool isDead;

    private Rigidbody2D rb;
    private GameObject focusedGhost;

    [SerializeField] private AudioSource fallingSound;
    [SerializeField] private AudioSource hitSound;
    [SerializeField] private AudioSource magicSound1;
    [SerializeField] private AudioSource magicSound2;
    [SerializeField] private AudioSource shieldSound;

    [SerializeField] private GameObject magicParticles;
    [SerializeField] private GameObject shieldParticles;
    [SerializeField] private GameObject elevatorParticles;


    void Awake() {
        keyCodes = (int[])System.Enum.GetValues(typeof(KeyCode));

        waveSystem = GameObject.FindGameObjectWithTag("WaveSystem").GetComponent<WaveSystem>();
        anim = gameObject.GetComponent<Animator>();
        rb = gameObject.GetComponent<Rigidbody2D>();

        health = 4;
        elevatorSprite.sprite = elevatorDamagedSprites[0];
    }

    void Update()
    {
        if (isDead == false)
        {
            if (Input.anyKeyDown)
            {
                foreach (KeyCode keyCode in keyCodes)
                {
                    if (Input.GetKeyDown(keyCode)) {

                        if (wordToType != "empty")
                        {
                            if ((char)keyCode == wordToType[letterTypedIndex])
                            {
                                letterTypedIndex++;

                                if (focusedGhost != null)
                                {   
                                    focusedGhost.GetComponent<EnemyController>().RemoveLetter();
                                }

                                anim.SetBool("IsCasting", true);

                                if (letterTypedIndex == wordToType.Length)
                                {
                                    GameObject particles = Instantiate(magicParticles, focusedGhost.transform.position, Quaternion.identity);
                                    Destroy(particles, 1.1f);

                                    Destroy(focusedGhost);
                                    wordToType = "empty";
                                    letterTypedIndex = 0;

                                    if (Random.value > 0.5f)
                                    {
                                        magicSound1.Play();
                                    }
                                    else
                                    {
                                        magicSound2.Play();
                                    }

                                    anim.SetBool("IsCasting", false);
                                }
                            }
                        }

                        break;
                    }
                }
            }

            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 5.23f;

            Vector3 objectPos = Camera.main.WorldToScreenPoint (transform.position);
            mousePos.x = mousePos.x - objectPos.x;
            mousePos.y = mousePos.y - objectPos.y;

            float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
            shieldAnchor.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 90f));


            if (waveSystem.spawnedGhosts.Count > 0)
            {
                GameObject oldFocusedGhost = focusedGhost;
                focusedGhost = GetClosestEnemy(waveSystem.spawnedGhosts, gameObject.transform);

                if (oldFocusedGhost != focusedGhost)
                {
                    letterTypedIndex = 0;
                }


                if(focusedGhost != null)
                {
                    focusedGhost.GetComponent<EnemyController>().ShowPrompt();
                }

                foreach (GameObject ghost in waveSystem.spawnedGhosts)
                {
                    if (focusedGhost != null)
                    {
                        if (ghost != null)
                        {
                            if (ghost != focusedGhost)
                            {
                                ghost.GetComponent<EnemyController>().Unfocus();
                            }
                        }
                    }
                }

                if (focusedGhost != null)
                {
                    focusedGhost.GetComponent<EnemyController>().ShowPrompt();
                    wordToType = focusedGhost.GetComponent<EnemyController>().Prompt;
                }
            }
        }
        else
        {
            rb.velocity = Vector2.down * deathFallSpeed;

            if (transform.position.y <= deathYEndLevel)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }

    GameObject GetClosestEnemy (List<GameObject> enemies, Transform fromThis)
    {
        GameObject bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = fromThis.position;

        foreach(GameObject potentialTarget in enemies)
        {
            if (potentialTarget != null)
            {
                Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if(dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = potentialTarget;
                }
            }
        }             
        return bestTarget;
    }

    public void PlayShieldSound()
    {
        shieldSound.Play();
    }

    public void SpawnShieldParticles()
    {
        GameObject particle = Instantiate(shieldParticles, shield.transform.position, shield.transform.rotation);
        Destroy(particle, 0.7f);
    }



    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Enemy Ghost"))
        {
            Destroy(collision.gameObject);

            health--;
            hitSound.Play();

            GameObject particle = Instantiate(elevatorParticles, transform.position, Quaternion.identity);
            Destroy(particle, 1.75f);

            if (health == 3)
            {
                elevatorSprite.sprite = elevatorDamagedSprites[1];
            }
            else if (health == 2)
            {
                elevatorSprite.sprite = elevatorDamagedSprites[2];
            }
            else if (health == 1)
            {
                elevatorSprite.sprite = elevatorDamagedSprites[3];
            }
            else if (health == 0)
            {
                //dead
                elevatorSprite.sprite = elevatorDamagedSprites[4];  

                anim.SetBool("IsDead", true);
                fallingSound.Play();
                isDead = true;
            }

        }
    }

}
