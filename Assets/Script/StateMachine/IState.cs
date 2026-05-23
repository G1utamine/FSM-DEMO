public interface IState {
    void OnEnter();
    void OnLogic();
    void OnPhysicsLogic();
    void OnExit();
}
