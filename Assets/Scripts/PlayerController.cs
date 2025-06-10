using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : StateBasedAI<PlayerController.State>
{
    private Animator _animator;
    private FieldOfView _fieldOfView;

    [SerializeField] private float _smoothness;

    public bool IsUsingSpecialSkill { get; private set; }
    public bool IsSpecialSkillCooldown { get; private set; }
    public float SpecialSkillTimer { get; set; }

    public bool IsDamaged { get; private set; }
    public bool IsDead => CurState == State.Dead;

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
        _animator = GetComponentInChildren<Animator>();
        _fieldOfView = GetComponent<FieldOfView>();
    }

    protected override IEnumerator OnStart()
    {
        CurState = State.Idle;
        yield break;
    }

    protected override void DefineStates()
    {
        AddState(State.Idle, new StateElem
        {
            Doing = new Func<IEnumerator>(OnIdle)
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
                break;
            }

            Vector3 characterDir = Vector3.Scale(Singleton<CameraManager>.Instance().MainCamera.transform.forward, new Vector3(1f, 0f, 1f));
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(characterDir), Time.deltaTime * _smoothness);

            yield return null;
        }

        yield break;
    }
}
