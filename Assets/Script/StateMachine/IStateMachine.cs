using System;

public interface IStateMachine
{
    IState LastState { get; }
    IState CurrentState { get; }
    void Init<T>() where T : IState;
    void AddState<T>(IState state) where T : IState;
    void AddAnyTransition<TTo>(Func<bool> condition) where TTo : IState;
    void AddTransition<TForm, TTo>(Func<bool> condition) where TForm : IState where TTo : IState;
    void OnLogic();
    void OnPhysicsLogic();
}
