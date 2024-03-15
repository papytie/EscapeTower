using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThiefShadowCasterSetter : MonoBehaviour
{

    public Animator animator;
    private string clipName;
    private AnimatorClipInfo[] currentClipInfo;
    private Vector3 farPosition;
    private Vector3 goodPosition;

    public GameObject idleBack;
    public GameObject idleFront;
    public GameObject idleLeft;
    public GameObject idleRight;


    public float speed = 5f; // Vitesse de déplacement du personnage
    private Rigidbody2D rb; // Référence au Rigidbody2D du personnage

    // Start is called before the first frame update
    void Start()
    {
        goodPosition = new Vector3(0, 0, 0);
        farPosition = new Vector3(100, 0, 0);

        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

        // Déplacement horizontal et vertical
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        if (moveHorizontal > 0)
        {
            idleBack.transform.localPosition = farPosition;
            idleRight.transform.localPosition = goodPosition;
            animator.SetBool("tempright", true);
        }
        else
        {
            idleBack.transform.localPosition = goodPosition;
            idleRight.transform.localPosition = farPosition;
            animator.SetBool("tempright", false);
        }

        // Calcul du vecteur de déplacement
        Vector2 movement = new Vector2(moveHorizontal, moveVertical);

        // Normalisation du vecteur de déplacement pour s'assurer que la vitesse diagonale est la même que la vitesse horizontale/verticale
        movement = movement.normalized * speed * Time.deltaTime;

        // Déplacement du personnage
        rb.MovePosition(rb.position + movement);



        /*currentClipInfo = this.animator.GetCurrentAnimatorClipInfo(0);
        clipName = currentClipInfo[0].clip.name;

        if (clipName == "ThiefIdleBack")
        {
            idleBack.transform.localPosition = goodPosition;
            idleFront.transform.localPosition = farPosition;
            idleLeft.transform.localPosition = farPosition;
            idleRight.transform.localPosition = farPosition;
        }

        if (clipName == "ThiefIdleFront")
        {
            idleBack.transform.localPosition = farPosition;
            idleFront.transform.localPosition = goodPosition;
            idleLeft.transform.localPosition = farPosition;
            idleRight.transform.localPosition = farPosition;
        }

        if (clipName == "ThiefIdleLeft")
        {
            idleBack.transform.localPosition = farPosition;
            idleFront.transform.localPosition = farPosition;
            idleLeft.transform.localPosition = goodPosition;
            idleRight.transform.localPosition = farPosition;
        }

        if (clipName == "ThiefIdleRight")
        {
            idleBack.transform.localPosition = farPosition;
            idleFront.transform.localPosition = farPosition;
            idleLeft.transform.localPosition = farPosition;
            idleRight.transform.localPosition = goodPosition;
        }*/
    }
}
