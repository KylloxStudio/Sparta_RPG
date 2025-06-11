using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

public class EnemyAIController : StateBasedAI<EnemyAIController.State>
{
    private Enemy _enemy;
    private Animator _animator;
    private NavMeshAgent _agent;
    private FieldOfView _fieldOfView;

    [SerializeField] private float _attackCooltime;
    private float _attackCooldownEnd;

    [SerializeField] private float _smoothness;

    private Player _target;

    public bool IsDamaged { get; private set; }
    public bool IsMoving => CurState == State.Chase;
    public bool IsAttacking => CurState == State.Attack;
    public bool IsDead => CurState == State.Dead;

    public bool IsAppearing { get; private set; }

    public enum State
    {
        Invalid = -1,
        Idle,
        Normal,
        Chase,
        Attack,
        Dead
    }

    protected override State InvalidState
    {
        get
        {
            return State.Invalid;
        }
    }

    protected override int StateEnumCount
    {
        get
        {
            return 5;
        }
    }

    protected override void OnAwake()
    {
        _enemy = GetComponent<Enemy>();
        _animator = GetComponentInChildren<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _fieldOfView = GetComponent<FieldOfView>();
    }

    protected override IEnumerator OnInitialized()
    {
        IsAppearing = true;

        _animator.SetTrigger("doAppear");
        _agent.speed = _enemy.Stats.MoveSpeed;

        yield return new WaitForSeconds(0.4f);
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
        CurState = State.Idle;
        IsAppearing = false;

        yield break;
    }

    private void OnEnable()
    {
        Init();
    }

    protected override void DefineStates()
    {
        AddState(State.Idle, new StateElem
        {
            Doing = new Func<IEnumerator>(OnIdle)
        });

        AddState(State.Normal, new StateElem
        {
            Doing = new Func<IEnumerator>(NormalDoing)
        });

        AddState(State.Chase, new StateElem
        {
            Entered = new Action(ChaseEntered),
            Doing = new Func<IEnumerator>(ChaseDoing),
            Exited = new Action(ChaseExited)
        });

        AddState(State.Attack, new StateElem
        {
            Entered = new Action(AttackEntered),
            Doing = new Func<IEnumerator>(AttackDoing),
            Exited = new Action(AttackExited)
        });

        AddState(State.Dead, new StateElem
        {
            Doing = new Func<IEnumerator>(OnDead)
        });
    }

    protected override bool IsAIEnded()
    {
        return false;
    }

    protected override bool IsTerminalState(State state)
    {
        return false;
    }

    private IEnumerator OnIdle()
    {
        while (CurState == State.Idle)
        {
            if (_fieldOfView.VisibleTargets.Length > 0)
            {
                CurState = State.Normal;
            }

            yield return new WaitForSeconds(0.1f);
        }

        yield break;
    }

    private IEnumerator NormalDoing()
    {
        if (_fieldOfView.VisibleTargets.Length > 0)
        {
            _target = _fieldOfView.VisibleTargets[0].GetComponent<Player>();
            if (!_target.Controller.IsDead)
            {
                float distance = Vector3.Distance(transform.position, _target.transform.position);
                if (distance > _enemy.Stats.AttackDistance)
                {
                    CurState = State.Chase;
                }
                else if (Time.time > _attackCooldownEnd)
                {
                    CurState = State.Attack;
                }
                else
                {
                    CurState = State.Normal;
                }
            }
        }

        yield return new WaitForSeconds(0.3f);
        yield break;
    }

    void ChaseEntered()
    {
        _agent.isStopped = false;
        _agent.SetDestination(_target.transform.position);

        _animator.SetBool("isMoving", true);
    }

    private IEnumerator ChaseDoing()
    {
        while (_target != null && !_target.Controller.IsDead)
        {
            if (IsInterrupted)
            {
                yield break;
            }

            float distance = Vector3.Distance(transform.position, _target.transform.position);
            if (distance <= _enemy.Stats.AttackDistance)
            {
                if (Time.time > _attackCooldownEnd)
                {
                    CurState = State.Attack;
                }
                else
                {
                    CurState = State.Normal;
                }

                yield break;
            }

            RotateTo(Vector3.Scale(_target.transform.position - transform.position, new Vector3(1f, 0f, 1f)));

            yield return null;
        }

        CurState = State.Normal;
        yield break;
    }

    private void ChaseExited()
    {
        _agent.isStopped = true;

        _animator.SetBool("isMoving", false);
    }

    private void AttackEntered()
    {
        _animator.SetBool("isAttacking", true);

        _attackCooldownEnd = Time.time + _attackCooltime;
    }

    private IEnumerator AttackDoing()
    {
        while (_target != null && !_target.Controller.IsDead)
        {
            if (IsInterrupted)
            {
                yield break;
            }

            float distance = Vector3.Distance(transform.position, _target.transform.position);
            if (distance > _enemy.Stats.AttackDistance)
            {
                CurState = State.Chase;
            }

            RotateTo(Vector3.Scale(_target.transform.position - transform.position, new Vector3(1f, 0f, 1f)));

            yield return null;
        }

        CurState = State.Normal;
        yield break;
    }

    private void AttackExited()
    {
        _animator.SetBool("isAttacking", false);
    }

    public void OnDamaged(GameObject attacker, int damage)
    {
        if (IsAppearing || IsDead)
        {
            return;
        }

        _enemy.Stats.Health -= damage;
        if (attacker.TryGetComponent(out Player player))
        {
            player.Stats.Cost += player.Weapon.Info.CostResilience;
        }

        if (_enemy.Stats.Health <= 0)
        {
            if (player != null)
            {
                player.Balance.AddPyroxenes(_enemy.Stats.RewardPyroxenes);
            }

            EventDead();
            return;
        }

        CancelInvoke(nameof(OffDamaged));
        IsDamaged = true;

        Invoke(nameof(OffDamaged), 0.4f);
    }

    private void OffDamaged()
    {
        IsDamaged = false;
    }

    public void EventDead()
    {
        CurState = State.Dead;
    }

    private IEnumerator OnDead()
    {
        _animator.SetTrigger("doDeath");
        yield return new WaitForSeconds(0.4f);
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
        _enemy.ReleaseObject();
        yield break;
    }

    private void RotateTo(Vector3 targetDir)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDir), Time.deltaTime * _smoothness);
    }
}
