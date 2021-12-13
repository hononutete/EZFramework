
namespace EZFramework.Game.AI
{
    /// <summary>
    /// 遷移条件を指定
    /// </summary>
    public class GameEntityAITransitionConditionTrigger : GameEntityAITransitionCondition
    {
        string trigger;

        public GameEntityAITransitionConditionTrigger(string trigger)
        {
            this.trigger = trigger;
        }

        /// <summary>
        ///  条件を達成しているかどうかの判定
        /// </summary>
        public override bool CheckCondition(string trigger) => this.trigger == trigger;


    }
}

