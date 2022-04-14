using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerRaceState : GameManagerState
{

    public bool raceWon;
    public override void Enter(GameManagerStateInput stateInput, CharacterStateTransitionInfo transitionInfo = null)
    {
        raceWon = false;
    }

    public override void Update(GameManagerStateInput stateInput)
    {
        if (raceWon) {
            character.ChangeState<GameManagerBeginState>();
            if (stateInput.currentRun > stateInput.runNum) {
                stateInput.winCanvas.SetActive(true);
            }
        }
    }

    public override void ForceCleanUp(GameManagerStateInput stateInput)
    {
        stateInput.gameManagerController.spawnHazards();
        stateInput.gameManagerController.respawnPlayer();
        base.ForceCleanUp(stateInput);
    }
}

