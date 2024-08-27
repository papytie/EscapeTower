using System;
using UnityEngine;

public class EnemyAnimationComponent : MonoBehaviour
{
    EnemyController controller;

    public void Init(EnemyController ctrlRef)
    {
        controller = ctrlRef;
    }

    public void UpdateMoveAnimDirection(Vector2 direction)
    {
        controller.Animator.SetFloat(SRAnimators.EnemyBaseAnimator.Parameters.up, direction.y);
        controller.Animator.SetFloat(SRAnimators.EnemyBaseAnimator.Parameters.right, direction.x);
    }

    public void UpdateMoveAnimSpeed(float speed)
    {
        controller.Animator.SetFloat(SRAnimators.EnemyBaseAnimator.Parameters.speed, 1 + MathF.Round(speed / 2, 2));
    }

    public void ActivateDieTrigger()
    {
        controller.Animator.SetTrigger(SRAnimators.EnemyBaseAnimator.Parameters.die);
    }

    public void ActivateTakeDamageTrigger()
    {
        controller.Animator.SetTrigger(SRAnimators.EnemyBaseAnimator.Parameters.takeDamage);
    }
}
