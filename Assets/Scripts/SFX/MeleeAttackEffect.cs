using UnityEngine;

[RequireComponent(typeof(Animator))]

public class MeleeAttackEffect : MonoBehaviour, IVisualEffect
{
    Animator effectAnimator;
    EnemyController enemyController;
    MeleeData attackData;

    public void Init(EnemyController controller, IActionData data)
    {
        effectAnimator = GetComponent<Animator>();
        enemyController = controller;
        attackData = data as MeleeData;

        //Set start position and visibility
    }

    public void Play()
    {
        effectAnimator.SetTrigger(SRAnimators.AttackEffectAnimTemplate.Parameters.play);
    }

    public void UpdateEffect()
    {

    }

    public void Interrupt()
    {
        effectAnimator.SetTrigger(SRAnimators.AttackEffectAnimTemplate.Parameters.stop);
    }
}
