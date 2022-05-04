using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerBeginState : GameManagerState {

    private float countDownTime = 3f;
    private float timer;
    public delegate void BeginState();
    public static event BeginState OnBeginState;

    public override void Enter(GameManagerStateInput stateInput, CharacterStateTransitionInfo transitionInfo = null)
    {
        //stateInput.spawnWalls.SetActive(true);
        timer = countDownTime;
        stateInput.currentRun++;

        if (OnBeginState != null)
            OnBeginState.Invoke();
    }

    public override void Update(GameManagerStateInput stateInput)
    {
        timer -= Time.deltaTime;
        if (timer <= 0) {
            character.ChangeState<GameManagerRaceState>();
        }
    }

    public override void ForceCleanUp(GameManagerStateInput stateInput)
    {
        base.ForceCleanUp(stateInput);
    }
}

