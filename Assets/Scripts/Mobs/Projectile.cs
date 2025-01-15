using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Projectile : MonoBehaviour
{
    [SerializeField] private int speed;
    [SerializeField] private Sprite[] explosionSprites = new Sprite[3];

    private Sprite initSprite;
    private GameObject fatherObject;
    private ObjectPool<Projectile> myPool;
    private SpriteRenderer projectileRenderer;
    private BoxCollider2D projectileCollider;
    private bool canMove = true;

    public ObjectPool<Projectile> MyPool { get => myPool; set => myPool = value; }
    public GameObject FatherObject { get => fatherObject; set => fatherObject = value; }
    public Sprite InitSprite { get => initSprite; set => initSprite = value; }
    public SpriteRenderer ProjectileRenderer { get => projectileRenderer; set => projectileRenderer = value; }
    public bool CanMove { get => canMove; set => canMove = value; }

    private void Start()
    {
        projectileRenderer = GetComponent<SpriteRenderer>();
        projectileCollider = GetComponent<BoxCollider2D>();
        initSprite = projectileRenderer.sprite;
    }

    void Update()
    {
        if(canMove)
            transform.Translate(Vector3.right * speed * Time.deltaTime);

        // El dispose de la pool parece no funcionar, esto es un workaround :(
        // El problema que tenía es que al destruir un GameObject que tiene una pool (los enemigos), sus "balas" se quedan existiendo en escena sin ser destruidas.
        // No he podido hacer que compartan una pool, que era mi objetivo.
        if (fatherObject == null && !projectileRenderer.isVisible)
        {
            Destroy(gameObject);
            return;
        }

        if (!projectileRenderer.isVisible)
            myPool.Release(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == null || fatherObject == null || this.gameObject == null)
            return;

        if (collision.gameObject.tag != "Projectile" && fatherObject.tag != collision.gameObject.tag)
        {
            Mob mob = collision.gameObject.GetComponent<Mob>();

            if(mob != null)
                mob.ApplyDamage();

            if (this.gameObject.activeSelf)
                StartCoroutine(AssignRandomSpriteAndDestroy());
        }
    }

    private IEnumerator AssignRandomSpriteAndDestroy()
    {
        //Give explosion sprite before destroying
        canMove = false;

        Sprite randomSprite = explosionSprites[Random.Range(0, explosionSprites.Length)];
        projectileRenderer.sprite = randomSprite;
        
        yield return new WaitForSeconds(0.1f);
        projectileCollider.enabled = false;
        yield return new WaitForSeconds(0.14f);

        projectileCollider.enabled = true;

        myPool.Release(this);
    }
}
