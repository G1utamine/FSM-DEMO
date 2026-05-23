using System;
using System.Collections.Generic;
public class StateMachine : IStateMachine
{
    //存储所有状态
    private Dictionary<Type, IState> states = new Dictionary<Type, IState>();
    //存储所有切换逻辑
    private Dictionary<Type, List<Transition>> transitions = new Dictionary<Type, List<Transition>>();
    private List<Transition> anyTransitions = new List<Transition>();
    //当前状态
    private IState currentState;
    private IState lastState;
    private Type currentStateType;
    public IState LastState => lastState;
    public IState CurrentState => currentState;
    //添加状态逻辑
    public void AddState<T>(IState state) where T : IState
    {
        states[typeof(T)] = state;
        transitions[typeof(T)] = new List<Transition>();
    }
    public void AddAnyTransition<TTo>(Func<bool> condition)
    where TTo : IState
    {

        anyTransitions.Add(new Transition(typeof(TTo), condition));
    }
    //添加状态切换条件逻辑
    public void AddTransition<TForm, TTo>(Func<bool> condition)
        where TForm : IState
        where TTo : IState
    {
        transitions[typeof(TForm)].Add(new Transition(typeof(TTo), condition));
    }
    //设置初始状态
    public void Init<T>() where T : IState
    {
        currentStateType = typeof(T);
        currentState = states[currentStateType];
        currentState.OnEnter();
    }
    //切换状态
    private void ChangeState(Type nextType)
    {
        if (currentStateType == nextType) return;
        currentState.OnExit();
        currentStateType = nextType;
        lastState = currentState;
        currentState = states[currentStateType];
        currentState.OnEnter();
    }
    //逻辑更新
    public void OnLogic()
    {
        //检查全局状态切换
        foreach (var transition in anyTransitions)
        {
            if (transition.to == currentStateType) continue;
            if (transition.condition()) {
                ChangeState(transition.to);
                return;
            }
        }
        //遍历当前状态的转换列表查找符合条件的新状态，找到了就中止循环进行更新
        foreach (var transition in transitions[currentStateType])
        {
            if (transition.condition())
            {
                ChangeState(transition.to);
                return;
            }
        }
        currentState.OnLogic();
    }
    //物理逻辑更新
    public void OnPhysicsLogic()
    {
        currentState.OnPhysicsLogic();
    }
    //状态切换数据类
    private class Transition
    {
        public Type to;
        public Func<bool> condition;

        public Transition(Type to, Func<bool> condition)
        {
            this.to = to;
            this.condition = condition;
        }
    }
}
