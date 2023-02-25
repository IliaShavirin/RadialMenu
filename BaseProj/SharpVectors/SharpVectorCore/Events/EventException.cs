using System;

namespace BaseProj.SharpVectors.SharpVectorCore.Events
{
    /// <summary>
    ///     Event operations may throw an
    ///     <see cref="EventException">EventException</see> as specified in their
    ///     method descriptions.
    /// </summary>
    public class EventException
        : Exception
    {
        #region Constructors

        public EventException(
            EventExceptionCode code)
        {
            Code = code;
        }

        #endregion

        #region Properties

        #region DOM Level 2

        /// <summary>
        ///     An integer indicating the type of error generated.
        /// </summary>
        public EventExceptionCode Code { get; }

        #endregion

        #endregion

        #region Private Fields

        #endregion
    }
}