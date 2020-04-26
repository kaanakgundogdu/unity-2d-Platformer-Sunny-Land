using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBasics : MonoBehaviour
{

    protected Animator anim;
    protected Rigidbody2D rbEnemy;
    protected AudioSource death;

    protected virtual void Start()  //virtual olunca kendi değişkeninmiş gibi alabiliyorsun
    {
        anim = GetComponent<Animator>();
        rbEnemy = GetComponent<Rigidbody2D>();
        death = GetComponent<AudioSource>();
    }

    public void JumpedOn()
    {
        anim.SetTrigger("Death_Enemy");
        death.Play();
        rbEnemy.velocity = Vector2.zero; // new verctor2 (0,0) aynı işi görüyor
    }

    private void DeathEnemy()
    {
        Destroy(this.gameObject);
    }

}