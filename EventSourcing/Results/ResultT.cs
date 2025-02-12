using Flunt.Notifications;

namespace EventSourcing.Results
{
    /// <summary>
    /// Representa o resultado de alguma operação, com informações de status e possivelmente um valor e um erro.
    /// </summary>
    /// <typeparam name="TValue">O tipo de valor do resultado.</typeparam>
    public class Result<TValue> : Result
    {
        private readonly TValue _value;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Result{TValueType}"/> com os parâmetros especificados.
        /// </summary>
        /// <param name="value">O valor do resultado.</param>
        /// <param name="isSuccess">A bandeira indicando se o resultado foi bem sucedido.</param>
        /// <param name="error">O erro.</param>
        protected internal Result(TValue value, bool isSuccess, Error error)
            : base(isSuccess, error)
            => _value = value;

        protected internal Result(TValue value, bool isSuccess, IReadOnlyCollection<Notification> notifications)
            : base(isSuccess, notifications)
            => _value = value;


        public static implicit operator Result<TValue>(TValue value) => Success(value);
        public static implicit operator Result<TValue>(Error error) => Failure<TValue>(error);

        /// <summary>
        /// Obtém o valor do resultado se o resultado for bem sucedido; caso contrário, lança uma exceção.
        /// </summary>
        /// <returns>O valor do resultado se o resultado for bem sucedido.</returns>
        /// <exception cref="InvalidOperationException"> quando <see cref="Result.IsFailure"/> é verdadeiro.</exception>
        public TValue Value => IsSuccess
            ? _value
            : throw new InvalidOperationException("The value of a failure result can not be accessed.");

    }
}
