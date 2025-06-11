using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateBasedAI<T> : MonoBehaviour where T : IConvertible
{
    protected T CurState
    {
        get
        {
            return _curState;
        }
        set
        {
            TransitionTo(value, false);
        }
    }

    protected T PrevState => _prevState;

    public bool IsInterrupted { get; set; }
    protected abstract T InvalidState { get; }
    protected abstract int StateEnumCount { get; }

    private static readonly EqualityComparer<T> Comparer = EqualityComparer<T>.Default;
    private readonly Dictionary<T, StateElem> _states = new Dictionary<T, StateElem>();

    private T _curState;
    private T _prevState;

    protected class StateElem
    {
        public Action Entered;
        public Func<IEnumerator> Doing;
        public Action Exited;
    }

    protected void TransitionTo(T nextState, bool force = false)
    {
        if (!force && IsAIEnded())
        {
            return;
        }

        IsInterrupted = (!IsTerminalState(_curState) || force);
        if (!force && IsTerminalState(_curState))
        {
            return;
        }

        _prevState = _curState;
        _curState = nextState;
        if (!Comparer.Equals(_prevState, InvalidState))
        {
            StateElem stateElem = _states.Get(_prevState, null);
            if (stateElem != null && stateElem.Exited != null)
            {
                stateElem.Exited();
            }
        }

        if (!Comparer.Equals(_curState, InvalidState))
        {
            StateElem stateElem = _states.Get(_curState, null);
            if (stateElem != null && stateElem.Entered != null)
            {
                stateElem.Entered();
            }
        }
    }

    protected abstract void DefineStates();

    protected virtual void OnAwake()
    {

    }

    protected virtual IEnumerator OnInitialized()
    {
        yield break;
    }

    protected virtual IEnumerator OnBeforeDoingState()
    {
        yield break;
    }

    protected virtual IEnumerator OnAfterDoingState()
    {
        yield break;
    }

    protected abstract bool IsAIEnded();

    protected abstract bool IsTerminalState(T state);

    private void Awake()
    {
        DefineStates();
        OnAwake();
    }

    protected void Init()
    {
        _curState = InvalidState;
        _prevState = InvalidState;

        StartCoroutine(ProcessInit());
    }

    private IEnumerator ProcessInit()
    {
        yield return StartCoroutine(OnInitialized());
        while (!IsAIEnded())
        {
            IsInterrupted = false;

            yield return StartCoroutine(OnBeforeDoingState());

            StateElem state = _states.Get(CurState, null);
            if (state != null)
            {
                if (state.Doing == null)
                {
                    while (!IsInterrupted)
                    {
                        yield return null;
                    }
                }
                else
                {
                    yield return StartCoroutine(state.Doing());
                }
            }

            yield return StartCoroutine(OnAfterDoingState());
            yield return null;
        }

        yield break;
    }

    protected void AddState(T state, StateElem stateElem)
    {
        _states.Add(state, stateElem);
    }
}