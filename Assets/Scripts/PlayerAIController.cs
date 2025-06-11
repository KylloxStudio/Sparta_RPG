using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAIController : StateBasedAI<PlayerAIController.State>
{
    private Player _player;
    private NavMeshAgent _agent;
    private FieldOfView _fieldOfView;

    [SerializeField] private float _smoothness;

    public Enemy Target { get; private set; }

    public bool IsSpecialSkillCooldown => SpecialSkillTimer < _player.Stats.SpecialSkillCooltime;
    public float SpecialSkillTimer { get; private set; }

    public bool IsDamaged { get; private set; }
    public bool IsMoving => CurState == State.Chase;
    public bool IsAttacking => CurState == State.Attack;
    public bool IsReloading => CurState == State.Reload;
    public bool IsUsingSpecialSkill => CurState == State.SpecialSkill;
    public bool IsUsingExSkill => CurState == State.ExSkill;
    public bool IsDead => CurState == State.Dead;

    private float _stoppingDistance;

    public enum State
    {
        Invalid = -1,
        Idle,
        Normal,
        Chase,
        Attack,
        Reload,
        SpecialSkill,
        ExSkill,
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
            return 8;
        }
    }

    protected override void OnAwake()
    {
        _player = GetComponent<Player>();
        _agent = GetComponent<NavMeshAgent>();
        _fieldOfView = GetComponent<FieldOfView>();
    }

    protected override IEnumerator OnInitialized()
    {
        _agent.speed = _player.Stats.MoveSpeed;
        _agent.stoppingDistance = _player.Weapon.Info.AttackDistance * 0.94f;

        CurState = State.Idle;
        yield break;
    }

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (_player.Stats.Cost < _player.Stats.ExSkillCost || IsUsingSpecialSkill || _player.Weapon.Info.CurAmmo <= 0)
            {
                return;
            }

            OnUsedExSkill();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (IsSpecialSkillCooldown || IsUsingExSkill || _player.Weapon.Info.CurAmmo <= 0)
            {
                return;
            }

            OnUsedSpecialSkill();
        }

        if (IsSpecialSkillCooldown)
        {
            SpecialSkillTimer += Time.deltaTime;
        }
        else
        {
            SpecialSkillTimer = _player.Stats.SpecialSkillCooltime;
        }
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

        AddState(State.Reload, new StateElem
        {
            Entered = new Action(ReloadEntered),
            Doing = new Func<IEnumerator>(ReloadDoing),
            Exited = new Action(ReloadExited)
        });

        AddState(State.SpecialSkill, new StateElem
        {
            Entered = new Action(SpecialSkillEntered),
            Doing = new Func<IEnumerator>(SpecialSkillDoing),
            Exited = new Action(SpecialSkillExited)
        });

        AddState(State.ExSkill, new StateElem
        {
            Entered = new Action(ExSkillEntered),
            Doing = new Func<IEnumerator>(ExSkillDoing),
            Exited = new Action(ExSkillExited)
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
            if (_fieldOfView.Targets.Length > 0)
            {
                CurState = State.Normal;
            }

            RotateTo(Vector3.Scale(Singleton<CameraManager>.Instance().MainCamera.transform.forward, new Vector3(1f, 0f, 1f)));
            Singleton<CameraManager>.Instance().SetCameraLookRotation(Quaternion.Euler(25f, transform.forward.y, transform.forward.z));

            yield return null;
        }

        yield break;
    }

    private IEnumerator NormalDoing()
    {
        if (_fieldOfView.Targets.Length > 0)
        {
            Enemy[] enemies = _fieldOfView.Targets.Select(o => o.GetComponent<Enemy>()).ToArray();
            Target = Singleton<InGameManager>.Instance().GetNearestEnemy(enemies);

            if (Target == null)
            {
                yield break;
            }

            float distance = Vector3.Distance(transform.position, Target.transform.position);
            if (distance > _player.Weapon.Info.AttackDistance)
            {
                CurState = State.Chase;
            }
            else if (!Target.Controller.IsAppearing)
            {
                CurState = State.Attack;
            }
        }

        yield return new WaitForSeconds(0.1f);
        yield break;
    }

    void ChaseEntered()
    {
        _agent.isStopped = false;
        _player.Animation.Animator.SetBool("isMoving", true);
    }

    private IEnumerator ChaseDoing()
    {
        while (Target != null && !Target.Controller.IsDead)
        {
            if (IsInterrupted)
            {
                yield break;
            }

            float distance = Vector3.Distance(transform.position, Target.transform.position);
            if (distance <= _agent.stoppingDistance)
            {
                CurState = State.Attack;
            }
            else
            {
                _agent.SetDestination(Target.transform.position);
            }

            RotateTo(Vector3.Scale(Target.transform.position - transform.position, new Vector3(1f, 0f, 1f)));
            Singleton<CameraManager>.Instance().SetCameraLookRotation(Quaternion.Euler(25f, transform.forward.y, transform.forward.z));

            yield return null;
        }

        CurState = State.Normal;
        yield break;
    }

    private void ChaseExited()
    {
        _agent.isStopped = true;
        _player.Animation.Animator.SetBool("isMoving", false);
    }

    private void AttackEntered()
    {
        _agent.stoppingDistance = _player.Weapon.Info.AttackDistance * 0.94f;
        _player.Animation.SetLayerWeight("Attack Layer", 0.9f, 50f);
        _player.Animation.Animator.SetBool("isShooting", true);
    }

    private IEnumerator AttackDoing()
    {
        while (Target != null && !Target.Controller.IsAppearing && !Target.Controller.IsDead)
        {
            if (IsInterrupted)
            {
                yield break;
            }

            if (_fieldOfView.VisibleTargets.Length <= 0)
            {
                _agent.stoppingDistance = 3.5f;
                CurState = State.Chase;
            }

            float distance = Vector3.Distance(transform.position, Target.transform.position);
            if (distance > _player.Weapon.Info.AttackDistance)
            {
                CurState = State.Chase;
            }

            Vector3 targetDir = Target.transform.position - transform.position;
            Singleton<CameraManager>.Instance().SetCameraLookRotation(Quaternion.LookRotation(targetDir));
            RotateTo(Vector3.Scale(targetDir, new Vector3(1f, 0f, 1f)));

            yield return null;
        }

        CurState = State.Normal;
        yield break;
    }

    private void AttackExited()
    {
        _player.Animation.Animator.SetBool("isShooting", false);
        _player.Animation.SetLayerWeight("Attack Layer", 0f, 10f);
    }

    public void EventReload()
    {
        CurState = State.Reload;
    }

    private void ReloadEntered()
    {
        _player.Animation.SetLayerWeight("Attack Layer", 0.9f, 50f);
        _player.Animation.Animator.SetTrigger("doReload_" + UnityEngine.Random.Range(1, 3).ToString());
    }

    private IEnumerator ReloadDoing()
    {
        if (IsReloading)
        {
            yield return new WaitForSeconds(_player.Weapon.Info.ReloadTime);

            _player.Weapon.Info.CurAmmo = _player.Weapon.Info.MaxAmmo;
            _player.Weapon.OnAmmoEvent?.Invoke(_player.Weapon.Info.CurAmmo, _player.Weapon.Info.MaxAmmo);
        }

        if (PrevState == State.Attack)
        {
            CurState = PrevState;
        }
        else
        {
            CurState = State.Normal;
        }

        yield break;
    }

    private void ReloadExited()
    {
        _player.Animation.Animator.ResetTrigger("doReload_1");
        _player.Animation.Animator.ResetTrigger("doReload_2");
        _player.Animation.SetLayerWeight("Attack Layer", 0f, 10f);
    }

    public void OnUsedSpecialSkill()
    {
        if (IsUsingSpecialSkill)
        {
            return;
        }

        CurState = State.SpecialSkill;
    }

    private void SpecialSkillEntered()
    {
        SpecialSkillTimer = 0f;
        _player.Weapon.Victim = null;
        _player.Animation.Animator.SetTrigger("doSpecialSkill");
    }

    private IEnumerator SpecialSkillDoing()
    {
        if (IsUsingSpecialSkill)
        {
            yield return new WaitForSeconds(0.3f);
            yield return new WaitForSeconds(_player.Animation.Animator.GetCurrentAnimatorStateInfo(0).length);
        }

        if (PrevState == State.Attack)
        {
            CurState = PrevState;
        }
        else
        {
            CurState = State.Normal;
        }

        yield break;
    }

    private void SpecialSkillExited()
    {
        _player.Animation.Animator.ResetTrigger("doSpecialSkill");
    }

    public void OnUsedExSkill()
    {
        if (IsUsingExSkill)
        {
            return;
        }

        CurState = State.ExSkill;
    }

    private void ExSkillEntered()
    {
        _player.Stats.Cost = 0f;
        _player.Weapon.Victim = null;
        _player.Animation.Animator.SetTrigger("doExSkill");
    }

    private IEnumerator ExSkillDoing()
    {
        if (IsUsingExSkill)
        {
            yield return new WaitForSeconds(0.3f);
            yield return new WaitForSeconds(_player.Animation.Animator.GetCurrentAnimatorStateInfo(0).length);
        }

        if (PrevState == State.Attack)
        {
            CurState = PrevState;
        }
        else
        {
            CurState = State.Normal;
        }

        yield break;
    }

    private void ExSkillExited()
    {
        _player.Animation.Animator.ResetTrigger("doExSkill");
    }

    public void OnDamaged(GameObject attacker, int damage)
    {
        if (IsDead)
        {
            return;
        }

        Singleton<CameraManager>.Instance().ShakeCamera(damage * 0.29f, 0.08f);

        _player.Stats.Health -= damage;
        if (_player.Stats.Health <= 0)
        {
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
        _player.Animation.Animator.SetTrigger("doDeath");
        yield return new WaitForSeconds(0.3f);
        yield return new WaitForSeconds(_player.Animation.Animator.GetCurrentAnimatorStateInfo(0).length);
        
        // Todo: 사망 UI / 부활 로직 작성

        yield break;
    }

    public void RotateTo(Vector3 targetDir)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDir), Time.deltaTime * _smoothness);
    }
}
