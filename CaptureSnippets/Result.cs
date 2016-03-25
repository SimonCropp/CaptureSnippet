using System;

namespace CaptureSnippets
{
    public struct Result<T>
    {
        string errorMessage;
        T value;

        public string ErrorMessage
        {
            get
            {
                if (Success)
                {
                    throw new Exception("Cane access ErrorMessage when Success.");
                }
                return errorMessage;
            }
        }

        public bool Success { get; }

        public T Value
        {
            get
            {
                if (!Success)
                {
                    throw new Exception("Cane access Value when not Success.");
                }
                return value;
            }
        }


        public static Result<T> Failed(string errorMessage)
        {
            return new Result<T>(errorMessage);
        }

        public Result(T value)
        {
            this.value = value;
            errorMessage = null;
            Success = true;
        }

        Result(string errorMessage)
        {
            this.errorMessage = errorMessage;
            Success = false;
            value = default(T);
        }
        public static implicit operator T(Result<T> result)
        {
            return result.Value;
        }

        public static implicit operator Result<T>(T value)
        {
            return new Result<T>(value);
        }

    }
}