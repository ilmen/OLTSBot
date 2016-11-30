using System;
using System.Diagnostics;

namespace StreamForRead.Classes
{
    public class Result
    {
        #region Статичные методы

        public static Result Fail(string errorText)
        {
            return new Result(errorText);
        }

        public static Result Fail(string errorText, Exception error)
        {
            return new Result(errorText, error);
        }

        public static Result Ok()
        {
            return new Result();
        }


        public static Result<T> Fail<T>(string errorText)
        {
            return new Result<T>(errorText);
        }

        public static Result<T> Fail<T>(string errorText, Exception error)
        {
            return new Result<T>(errorText, error);
        }

        public static Result<T> Ok<T>(T value)
        {
            return new Result<T>(value);
        }

        #endregion

        protected Result()
        {
            Error = null;
            ErrorText = null;
            IsFailured = false;
        }

        public Result(String errorText, Exception error = null)
        {
            Error = error;
            ErrorText = errorText;
            IsFailured = true;
        }

        public Exception Error { get; private set; }

        public String ErrorText { get; private set; }

        public bool IsFailured { get; private set; }

        public void ThrowIfFailure()
        {
            if (!IsFailured) return;

            System.Diagnostics.Debug.Assert(!System.Diagnostics.Debugger.IsAttached, ErrorText);

            // ReSharper disable once HeuristicUnreachableCode
            throw Error == null ? new Exception(ErrorText) : new Exception(ErrorText, Error);
        }

        public Result<TResult> FailCastTo<TResult>()
        {
            if (!IsFailured) throw new NotSupportedException("Supported only failed results!");

            return new Result<TResult>(ErrorText, Error);
        }

        public override string ToString()
        {
            var description = IsFailured ? string.Format(". Error: {0}", ErrorText) : ". Correct";
            return base.ToString() + description;
        }
    }

    public sealed class Result<T> : Result
    {
        // ReSharper disable once RedundantDefaultMemberInitializer
        private readonly T _value = default(T);

        public T Value
        {
            get { return _value; }
        }

        public Result(T value)
        {
            _value = value;
        }

        public Result(String errorText, Exception error = null)
            : base(errorText, error)
        {
        }
    }
}