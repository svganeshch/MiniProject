public class StateMachine
{
    public State currentState;
    public State previousState;

    public void Initialize(State initialState)
    {
        currentState = initialState;
        currentState.Enter();
    }

    public void ChangeState(State newState, bool repeat = false)
    {
        if (!repeat)
        {
            if (newState == currentState) return;
        }

        previousState = currentState;
        currentState.Exit();

        currentState = newState;
        newState.Enter();
    }
}
