namespace EmployeeApi.Models
{
    public class Result
    {
        public bool Success {get; set;}
        public bool NotFound {get; set;}
        public string Error {get; set;}
        public int Id {get; set;}

        public static Result Ok(int id = 0)
        {
            return new Result { Success = true, Id = id };
        }

        public static Result Fail(string error)
        {
            return new Result { Error = error };
        }

        public static Result NotFoundResult()
        {
            return new Result { NotFound = true };
        }
    }

    public class Result<T>
    {
        public bool Success { get; set; }
        public bool NotFound { get; set; }
        public string Error { get; set; }
        public T Value { get; set; }

        public static Result<T> Ok(T value)
        {
            return new Result<T> { Success = true, Value = value };
        }

        public static Result<T> Fail(string error)
        {
            return new Result<T> { Error = error };
        }

        public static Result<T> NotFoundResult()
        {
            return new Result<T> { NotFound = true };
        }
    }
}