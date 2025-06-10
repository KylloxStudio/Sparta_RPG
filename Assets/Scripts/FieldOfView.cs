using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] private float _viewRadius;
    public float ViewRadius => _viewRadius;

    [SerializeField, Range(0, 360)] private float _viewAngle;
    public float ViewAngle => _viewAngle;

    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private LayerMask _obstacleLayer;

    private List<GameObject> _visibleTargets = new List<GameObject>();
    public GameObject[] VisibleTargets => _visibleTargets.ToArray();

    [SerializeField] private bool _ignoreObstacle;

    private void Start()
    {
        StartCoroutine(FindVisibleTargets());
    }

    private IEnumerator FindVisibleTargets()
    {
        while (!Singleton<GameManager>.Instance().IsGameEnded)
        {
            _visibleTargets.Clear();
            Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, _viewRadius, _targetLayer);
            for (int i = 0; i < targetsInViewRadius.Length; i++)
            {
                Transform target = targetsInViewRadius[i].transform;
                Vector3 targetDir = (target.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, targetDir) < _viewAngle * 0.5f)
                {
                    float distToTarget = Vector3.Distance(transform.position, target.position);
                    if (_ignoreObstacle)
                    {
                        _visibleTargets.Add(target.gameObject);
                    }
                    else if (!Physics.Raycast(transform.position, targetDir, distToTarget, _obstacleLayer))
                    {
                        _visibleTargets.Add(target.gameObject);
                    }
                }
            }

            yield return null;
        }

        yield break;
    }

    public Vector3 DirFromAngle(float angleDegrees, bool globalAngle)
    {
        if (!globalAngle)
        {
            angleDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Cos((-angleDegrees + 90) * Mathf.Deg2Rad), 0, Mathf.Sin((-angleDegrees + 90) * Mathf.Deg2Rad));
    }
}