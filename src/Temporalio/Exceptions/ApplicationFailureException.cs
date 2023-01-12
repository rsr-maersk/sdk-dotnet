using System;
using System.Collections.Generic;
using Temporalio.Api.Failure.V1;

namespace Temporalio.Exceptions
{
    /// <summary>
    /// Exception representing an error in user code.
    /// </summary>
    /// <remarks>
    /// For workflows, users should throw this exception to signal a workflow failure. Other
    /// non-Temporal exceptions will not fail the workflow.
    /// <para>
    /// In activities, all non-Temporal exceptions are translated to this exception as retryable
    /// with the <see cref="Type" /> as the unqualified exception class name.
    /// </para>
    /// </remarks>
    public class ApplicationFailureException : FailureException
    {
        /// <summary>
        /// <see cref="ApplicationFailureException(string, Exception?, string?, bool, IReadOnlyCollection&lt;object&gt;?)" />
        /// </summary>
        public ApplicationFailureException(
            string message,
            string? type = null,
            bool nonRetryable = false,
            IReadOnlyCollection<object>? details = null
        ) : base(message)
        {
            Type = type;
            NonRetryable = nonRetryable;
            Details = new OutboundFailureDetails(details ?? new object[0]);
        }

        /// <summary>
        /// Create a new application exception.
        /// </summary>
        /// <param name="message">Required message for the exception.</param>
        /// <param name="inner">
        /// Cause of the exception (can use other constructor if no cause).
        /// </param>
        /// <param name="type">Optional string type name of the exception</param>
        /// <param name="nonRetryable">If true, marks the exception as non-retryable.</param>
        /// <param name="details">Collection of details to serialize into the exception.</param>
        public ApplicationFailureException(
            string message,
            Exception? inner,
            string? type = null,
            bool nonRetryable = false,
            IReadOnlyCollection<object>? details = null
        ) : base(message, inner)
        {
            Type = type;
            NonRetryable = nonRetryable;
            Details = new OutboundFailureDetails(details);
        }

        /// <inheritdoc />
        protected internal ApplicationFailureException(
            Failure failure,
            Exception? inner,
            Converters.IPayloadConverter converter
        ) : base(failure, inner)
        {
            var info =
                failure.ApplicationFailureInfo
                ?? throw new ArgumentException("No application failure info");
            Type = info.Type.Length == 0 ? null : info.Type;
            NonRetryable = info.NonRetryable;
            Details = new InboundFailureDetails(converter, info.Details?.Payloads_);
        }

        /// <summary>
        /// String "type" of the exception if any.
        /// </summary>
        public string? Type { get; protected init; }

        /// <summary>
        /// Whether this exception is non-retryable.
        /// </summary>
        public bool NonRetryable { get; protected init; }

        /// <summary>
        /// Access to the details of the exception. This is never null.
        /// </summary>
        /// <remarks>
        /// This will be <see cref="OutboundFailureDetails" /> for user-created exceptions and
        /// <see cref="InboundFailureDetails" /> for server-serialized exceptions.
        /// </remarks>
        public IFailureDetails Details { get; protected init; }
    }
}
