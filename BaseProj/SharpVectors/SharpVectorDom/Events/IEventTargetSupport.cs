using BaseProj.SharpVectors.SharpVectorCore.Events;

namespace BaseProj.SharpVectors.SharpVectorDom.Events
{
    /// <summary>
    ///     Summary description for IEventTargetSupports.
    /// </summary>
    public interface IEventTargetSupport : IEventTarget
    {
        #region NON-DOM

        void FireEvent(
            IEvent evt);

        #endregion
    }
}