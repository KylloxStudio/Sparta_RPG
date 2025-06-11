using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Player _player;
    public Animator Animator { get; private set; }

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
        Animator = GetComponent<Animator>();
    }

    public void SetLayerWeight(string layerName, float weight, float speed = 0f)
    {
        int layerIndex = Animator.GetLayerIndex(layerName);
        if (layerIndex == -1)
        {
            return;
        }

        StartCoroutine(ProcessLayerWeight(layerIndex, weight, speed));
    }

    private IEnumerator ProcessLayerWeight(int layerIndex, float weight, float speed)
    {
        if (speed != 0f)
        {
            float curWeight = 0f;
            while (curWeight < weight)
            {
                curWeight = Animator.GetLayerWeight(layerIndex);
                Animator.SetLayerWeight(layerIndex, Mathf.Lerp(curWeight, weight, Time.deltaTime * speed));

                yield return null;
            }
        }

        Animator.SetLayerWeight(layerIndex, weight);

        yield break;
    }

    public void Shoot()
    {
        if (_player.Controller.IsMoving || _player.Controller.IsDead || Animator.GetLayerWeight(Animator.GetLayerIndex("Attack Layer")) < 0.5f)
        {
            return;
        }

        _player.Weapon.Shoot();
    }
}
