using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // SerializeField: Eleman private olsa bile unity arayüzünde görülebilir.
    [SerializeField] private Rigidbody2D playerRb;     
    [SerializeField] private Animator anim;
    [SerializeField] private LayerMask ground;
    [SerializeField] private int cherries=0;
    [SerializeField] private Text scoreText;
    [SerializeField] public float hurtDamage=5f;
    [SerializeField] private AudioSource cherrySound;
    [SerializeField] private AudioSource stepSound;


    private enum State {idle, running, jumping,falling,hurt}
    private State state = State.idle;    
    private Collider2D isGround;


    public float moveHorizontal;
    private float speed=6.5f;
    private float jumpforce=8.5f;

    
    
    void Start()
    {
        playerRb= GetComponent<Rigidbody2D>();
        anim= GetComponent<Animator>();
        isGround = GetComponent<Collider2D>();
        scoreText = GameObject.Find("Score").GetComponent<Text>();
    }


    void Update()
    {
        //Hareket etmek için gereken fonksion frame başı çağırılıyor.
        Movement();
        //Animasyonlar için oluşturulan fonksiyon
        AnimationState();

        //Animasyonun ardından stabil hale dönüş
        anim.SetInteger("state", (int)state );

    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Collectable")
        {
            cherrySound.Play();
            Destroy(collision.gameObject);
            cherries++;
            scoreText.text = cherries.ToString();
        }

    }


    // Düşmanla etkileşime girince yaılacaklar 
    private void OnCollisionEnter2D (Collision2D other)
    {
        if(other.gameObject.tag=="Enemy")
        {
            EnemyBasics frog= other.gameObject.GetComponent<EnemyBasics>();

            //Sadece yukardan gelirsek  yok edilecek düşman
            if(state==State.falling)
            { 
                frog.JumpedOn();
                Jump();
            }

            else
            {
                state = State.hurt;
                if(other.gameObject.transform.position.x > transform.position.x)
                {
                    // düşman sağda,  hasar alacam ve sola gidecek
                    playerRb.velocity= new Vector2(-hurtDamage,playerRb.velocity.y);
                }

                else
                {
                    //düşmaan solda hasar alacam ve sağa gidecek 
                    playerRb.velocity= new Vector2(hurtDamage,playerRb.velocity.y);

                }
            }
        }
    }

    private void Movement()
    {
        moveHorizontal = Input.GetAxis ("Horizontal");

        float moveVertical = Input.GetAxis ("Vertical");

        // Saga hareket
        if(moveHorizontal<0)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime*moveHorizontal);
            transform.localScale = new Vector2 (-1,1); // animasyın icin
        }

        // Sola hareket
        else if(moveHorizontal>0)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime*moveHorizontal);
            transform.localScale = new Vector2 (1,1); // animasyın icin
        }

        // Zıplama
        if (Input.GetKeyDown(KeyCode.Space) && isGround.IsTouchingLayers(ground) )
        {
            Jump();
        }
    }


    private void Jump()
    {
            playerRb.AddForce(new Vector2(0,1f)*jumpforce,ForceMode2D.Impulse);
            state= State.jumping;
    }


    private void AnimationState()
    {
        if (state == State.jumping)
        {
            if (playerRb.velocity.y < 0.1f)
            {
                state = State.falling;
            }
        }

        else if (state ==State.falling)
        {
            if(isGround.IsTouchingLayers(ground))
            {
                state = State.idle;
            }
        }

        else if (state == State.hurt)
        {
            if(Mathf.Abs(playerRb.velocity.x) < 0.1f)
            {
                state= State.idle;
            }
        }

        else if (Mathf.Abs (moveHorizontal) > 0.2f)
        {
            state = State.running;
        }

        else 
        {
            state = State.idle;
            
        }
    }

    private void FootStep()
    {
        stepSound.Play(); //ses kaydını çalıyor
    }
}
