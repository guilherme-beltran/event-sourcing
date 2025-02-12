using Flunt.Notifications;

namespace EventSourcing.Results
{
    public class Result
    {
        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Result"/> com os parâmetros especificados.
        /// </summary>
        /// <param name="isSuccess">A bandeira indicando se o resultado foi bem sucedido.</param>
        /// <param name="error">O erro.</param>
        protected Result(bool isSuccess, Error error)
        {
            //if (isSuccess && error != Error.None)
            //{
            //    throw new InvalidOperationException();
            //}

            if (!isSuccess && error == Error.None)
            {
                throw new InvalidOperationException();
            }

            IsSuccess = isSuccess;
            Error = error;
        }

        protected Result(bool isSuccess, IReadOnlyCollection<Notification> notifications)
        {

            IsSuccess = isSuccess;
            Notifications = notifications;
        }

        /// <summary>
        /// Obtém um valor que indica se o resultado é um resultado de sucesso.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Obtém um valor que indica se o resultado é um resultado de falha.
        /// </summary>
        public bool IsFailure => !IsSuccess;

        /// <summary>
        /// Obtém o erro.
        /// </summary>
        public Error Error { get; }

        /// <summary>
        /// Obtém as notificações de domínio.
        /// </summary>
        public IReadOnlyCollection<Notification> Notifications { get; }

        /// <summary>
        /// Retorna um sucesso <see cref="Result"/>.
        /// </summary>
        /// <returns>Uma nova instância de <see cref="Result"/> com o sinalizador de sucesso definido.</returns>
        public static Result Success() => new Result(true, Error.None);

        /// <summary>
        /// Retorna um sucesso <see cref="Result{TValue}"/> com o valor especificado.
        /// </summary>
        /// <typeparam name="TValue">O tipo de resultado.</typeparam>
        /// <param name="value">O valor do resultado.</param>
        /// <returns>Uma nova instância de <see cref="Result{TValue}"/> com o sinalizador de sucesso definido.</returns>
        public static Result<TValue> Success<TValue>(TValue value) => new Result<TValue>(value, true, Error.None);

        /// <summary>
        /// Cria um novo <see cref="Result{TValue}"/> com o valor nullable especificado e o erro especificado.
        /// </summary>
        /// <typeparam name="TValue">O tipo de resultado.</typeparam>
        /// <param name="value">O valor do resultado.</param>
        /// <param name="error">O erro no caso de o valor ser nulo.</param>
        /// <returns>Uma nova instância de <see cref="Result{TValue}"/> com o valor especificado ou um erro.</returns>
        public static Result<TValue> Create<TValue>(TValue value, Error error)
            where TValue : class
            => value is null ? Failure<TValue>(error) : Success(value);

        /// <summary>
        /// Retorna uma falha <see cref="Result"/> com o erro especificado.
        /// </summary>
        /// <param name="error">O erro.</param>
        /// <returns>Uma nova instância de <see cref="Result"/> com o erro especificado e o sinalizador de falha definido.</returns>
        public static Result Failure(Error error) => new Result(false, error);


        /// <summary>
        /// Devolve uma falha <see cref="Result{TValue}"/> com o erro especificado.
        /// </summary>
        /// <typeparam name="TValue">O tipo de resultado.</typeparam>
        /// <param name="error">O erro.</param>
        /// <returns>Uma nova instância de <see cref="Result{TValue}"/> com o erro especificado e o sinalizador de falha definido.</returns>
        /// <remarks>
        /// Nós estamos propositalmente ignorando a atribuição nullable aqui porque a API nunca permitirá que ela seja acessada.
        /// O valor é acessado através de um método que lançará uma exceção se o resultado for um resultado de falha.
        /// </ remarks>
        public static Result<TValue> Failure<TValue>(Error error) => new Result<TValue>(default!, false, error);

        /// <summary>
        /// Retorna a primeira falha do <paramref name="results"/> especificado.
        /// Se não houver nenhuma falha, um sucesso é retornado.
        /// </summary>
        /// <param name="results">O array de resultados.</param>
        /// <returns>
        /// A primeira falha do array especificado <paramref name="results"/>,ou um sucesso se não existir.
        /// </returns>
        public static Result FirstFailureOrSuccess(params Result[] results)
        {
            foreach (Result result in results)
            {
                if (result.IsFailure)
                {
                    return result;
                }
            }

            return Success();
        }

        public static IEnumerable<Error> HasFailure(params Result[] results)
        {
            IEnumerable<Error> errors = new List<Error>();

            if (!results.Any(r => r.IsFailure))
            {
                return Enumerable.Empty<Error>();
            }

            errors = results.Select(r =>
            {
                return r.Error;
            });

            return errors;
        }

        public static Result<TValue> Failure<TValue>(IReadOnlyCollection<Notification> notifications) => new Result<TValue>(default, false, notifications);

    }

}