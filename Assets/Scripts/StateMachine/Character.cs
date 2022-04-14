using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character<C, S, I> : MonoBehaviour
where C : Character<C, S, I>
where S : CharacterState<C, S, I>
where I : CharacterStateInput, new()
{
    /// A soft transition is for when you want to allow a state to finish doing something before switching. A hard transition is when you want to immediately switch
    public delegate void SoftTransitionAction();
    /// Call this to complete the SoftTransition (remember to look for an example of this)
    public SoftTransitionAction softTransitionChangeState;

    ///  using the System class, we're able to refer to type declarations as a class using Type. In this case, we're using this to make a dictionary
    ///  that maps from a Type to a class. This is useful because we can make all of our states inherit from one class, and we can use that class's type
    ///  to initialize all the state classes that we make.
    protected Dictionary<Type, S> stateMap;
    protected S state = null;
    protected I stateInput = new I();

    // NOTE: Little hack to ensure Enter is called on next update after transition
    private Trigger stateChanged;
    private CharacterStateTransitionInfo info;

    protected void Start()
    {
        stateMap = new Dictionary<Type, S>();
        Init();

        //Gets all inherited classes of S and instantiates them using voodoo magic code I got from Brandon Shockley lol
        foreach (Type type in Assembly.GetAssembly(typeof(S)).GetTypes()
            .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(S))))
        {
            S newState = (S)Activator.CreateInstance(type);
            newState.character = this;
            newState.Init(stateInput);
            stateMap.Add(type, newState);
        }

        SetInitialState();
    }

    /// Initialization within the subclass
    /// Override this class function if you want to setup any initial values here
    protected virtual void Init() { }

    ///Init initial state here
    protected abstract void SetInitialState();

    /// Override this method if you want to update any values in StateInput
    protected virtual void UpdateInput() { }

    /// Runs at the end of every frame
    protected void LateUpdate()
    {
        // using the Value property to to make sure that the next state's Enter function gets run
        if (stateChanged.Value)
            state.Enter(stateInput, info);
        UpdateInput();
        state.Update(stateInput);
    }

    // This is the actual Unity FixedUpdate since the states that we make don't inherit from Monobehavior
    protected void FixedUpdate()
    {
        state.FixedUpdate(stateInput);
    }

    protected void OnAnimationEvent(string eventName)
    {
        state.OnAnimationEvent(eventName);
    }

    /// This can be called by any of this character's states in order to transition to a new one.
    /// The optional transition info provided is passed to the new state when it is Enter()-ed.
    public void ChangeState<N>(CharacterStateTransitionInfo transitionInfo = null) where N : S
    {
        if (state != null)
        {
            state.ForceCleanUp(stateInput);
        }
        state = stateMap[typeof(N)]; //gets the numerical representation of the State you put in, then uses that as a key to get the actual instantiated state we put into the statemap before
        softTransitionChangeState = null;
        stateChanged.Value = true;
        info = transitionInfo;
    }

    /// Gives the current state a gentle warning that it should transition as soon as possible to the given state N
    public void ChangeStateSoft<N>(CharacterStateTransitionInfo transitionInfo = null) where N : S
    {
        softTransitionChangeState = () => ChangeState<N>(transitionInfo);
        state.SoftTransitionWarning(stateInput);
    }
}

public abstract class CharacterState<C, S, I>
where C : Character<C, S, I>
where S : CharacterState<C, S, I>
where I : CharacterStateInput, new()
{
    public Character<C, S, I> character;

    public virtual void Init(I stateInput) { }

    public virtual void Enter(I stateInput, CharacterStateTransitionInfo transitionInfo = null) { }

    /// Called externally to request that a state finish ASAP and call softTransitionChangeState()
    /// The default implementation will immediately transition
    public virtual void SoftTransitionWarning(I stateInput) { character.softTransitionChangeState(); }

    /// Called externally to alert a state that it has until this function returns to clean up any internal logic
    /// There will be a state transition immediately upon returning
    public virtual void ForceCleanUp(I stateInput) { }

    public virtual void Update(I stateInput) { }

    public virtual void FixedUpdate(I stateInput) { }

    public virtual void OnAnimationEvent(string eventName) { }
}

public abstract class CharacterStateInput
{
}

/// Extend this class for custom state transition info
/// Cast it within the Enter() function of the state that uses it
public abstract class CharacterStateTransitionInfo
{
}