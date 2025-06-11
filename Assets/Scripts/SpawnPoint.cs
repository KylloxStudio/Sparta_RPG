using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public bool IsUsingPoint { get; private set; }

    private Collider[] _targets = new Collider[1];


    private void Update()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position, 7.5f, _targets, LayerMask.GetMask("Character", "Enemy"));
        if (count > 0)
        {
            IsUsingPoint = true;
        }
        else
        {
            IsUsingPoint = false;
        }
    }
}