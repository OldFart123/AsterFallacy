using UnityEngine;

public class Player_Attack : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] fireballs;
    [SerializeField] private AudioClip fireballSound;

    private Animator Animl;
    private Player_Movement playerMovement;
    //private float cooldownTimer = Mathf.Infinity;

    private void Awake()
    {
        Animl = GetComponent<Animator>();
        playerMovement = GetComponent<Player_Movement>();
    }

    //private void Update()
    //{
    //    if (Input.GetKey(KeyCode.E) && cooldownTimer > attackCooldown && playerMovement.canAttack())
    //        //Kick();

    //    cooldownTimer += Time.deltaTime;
    //}

    //private void Kick()
    //{
    //    SoundManager.instance.PlaySound(FireBallSound);
    //    Animl.SetTrigger("Kick");
    //    cooldownTimer = 0;

    //    fireballs[FindFireball()].transform.position = firePoint.position;
    //    fireballs[FindFireball()].GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x));
    //}
}
