using EZFramework.Json;

namespace EZFramework.API
{
    /// <summary>
    /// APIの結果を処理する基底クラス
    /// </summary>
    public abstract class APIReponseHandler
    {
        public abstract void OnSuccess(string text);
    }

    public class APIReponseHandler<T> : APIReponseHandler where T : APIResponse
    {
        public override void OnSuccess(string text)
        {
            T newModel = Json.Json.ToObject<T>(text);
            OnSuccess(newModel);
        }

        public virtual void OnSuccess(T response)
        {

        }
    }
}