public abstract class State
{
    protected StateManager manager;
    protected StateMachine stateMachine;

    protected State(StateManager manager, StateMachine stateMachine)
    {
        this.manager = manager;
        this.stateMachine = stateMachine;
    }

    public virtual void Enter()
    {

    }

    public virtual void LogicUpdate()
    {

    }

    public virtual void Exit()
    {

    }
}
