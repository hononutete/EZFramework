namespace EZFramework.Game.AI
{
    public enum EComparision
    {
        LESS = 1,
        GREATER = 2,
        LEQUAL = 3,
        GEQUAL = 4,
        EQUAL = 5,
        NOT_EQUAL = 6,
        ALWAYS = 7
    }

    enum EAIStateTransitionConditionType
    {
        TRIGGER = 1,
        INTEGER = 2,
        BY_MASTER = 3,
    }

    enum EAIStateTransitionTargetParameterType
    {
        HP_ABS = 1,
        HP_RATE = 2,
        DISTANCE_TO_TARGET = 3,
        TIME_ELAPSED = 4,
        HAND_COUNT = 5
    }

    enum EAIStateType
    {
        RELAY = 1,
        ATTACK = 2,
        MOVE_RANDOM = 3,
        MOVE_TOWARD_TARGET = 4,
        PLAY_CARD = 5
    }
}
