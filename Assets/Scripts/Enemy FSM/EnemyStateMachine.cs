public class EnemyStateMachine
{
    public EnemyState currentState;
    public EnemyState previousState;

    public void Initialize(EnemyState initialState)
    {
        currentState = initialState;
        currentState.Enter();
    }

    public void ChangeState(EnemyState newState)
    {
        previousState = currentState;
        currentState.Exit();

        currentState = newState;
        newState.Enter();
    }
}