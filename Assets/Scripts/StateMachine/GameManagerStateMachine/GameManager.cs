using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Character<GameManager, GameManagerState, GameManagerStateInput> {

	override protected void Init()
    {
        stateInput.stateMachine = this;
        stateInput.variableHolder = GetComponent<GameManagerVariableHolder>();
        stateInput.spawnWalls = stateInput.variableHolder.spawnWalls;
        stateInput.gameManagerController = GetComponent<GameManagerController>();
        stateInput.winCanvas = stateInput.variableHolder.winCanvas;
        //stateInput.winCanvas.SetActive(false);
        stateInput.runNum = 4;
        stateInput.currentRun = 0;
    }

    override protected void SetInitialState()
    {
        ChangeState<GameManagerBeginState>();
    }

    public GameManagerStateInput GetStateInput() {
        return stateInput;
    }

    public GameManagerState GetState() {
        return state;
    }

}

public abstract class GameManagerState : CharacterState<GameManager, GameManagerState, GameManagerStateInput>
{

}

public class GameManagerStateInput : CharacterStateInput
{
    public GameManagerController gameManagerController;
    public GameManager stateMachine;
    public GameManagerVariableHolder variableHolder;
    public GameObject spawnWalls;
    public int runNum;
    public int currentRun = 0;

    public GameObject winCanvas;
}

