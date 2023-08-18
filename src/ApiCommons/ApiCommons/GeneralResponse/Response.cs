using System.Net;

namespace ApiCommons.GeneralResponse
{
    /// <summary>
    /// Represents a generic response from an API endpoint.
    /// </summary>
    /// <typeparam name="T">The type of data contained in the response.</typeparam>
    public class Response<T>
    {
        /// <summary>
        /// Gets or sets the data of the response.
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Gets the list of messages associated with the response.
        /// </summary>
        public List<string> Message { get; private set; } = new List<string>();

        /// <summary>
        /// Gets or sets a value indicating whether an error occurred.
        /// </summary>
        public bool Error { get; set; } = false;

        /// <summary>
        /// Gets or sets the HTTP status code of the response.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Creates a successful response with the specified status code, data, and message.
        /// </summary>
        /// <typeparam name="T">The type of the data in the response.</typeparam>
        /// <param name="statusCode">The HTTP status code of the response.</param>
        /// <param name="data">The data to include in the response.</param>
        /// <param name="message">The message to include in the response.</param>
        /// <returns>A new instance of <see cref="Response{T}"/> representing a successful response.</returns>
        public static Response<T> Success(HttpStatusCode statusCode, T data, string message)
        {
            return new Response<T> { Data = data, StatusCode = statusCode, Message = new List<string> { message } };
        }

        /// <summary>
        /// Creates a successful response with the specified status code and message.
        /// </summary>
        /// <typeparam name="T">The type of the data in the response.</typeparam>
        /// <param name="statusCode">The HTTP status code of the response.</param>
        /// <param name="message">The message to include in the response.</param>
        /// <returns>A new instance of <see cref="Response{T}"/> representing a successful response.</returns>
        public static Response<T> Success(HttpStatusCode statusCode, string message)
        {
            return new Response<T> { StatusCode = statusCode, Message = new List<string> { message } };
        }

        /// <summary>
        /// Creates a successful response with the specified status code and data.
        /// </summary>
        /// <typeparam name="T">The type of the data in the response.</typeparam>
        /// <param name="statusCode">The HTTP status code of the response.</param>
        /// <param name="data">The data to include in the response.</param>
        /// <returns>A new instance of <see cref="Response{T}"/> representing a successful response.</returns>
        public static Response<T> Success(HttpStatusCode statusCode, T data)
        {
            return new Response<T> { StatusCode = statusCode, Data = data };
        }

        /// <summary>
        /// Creates a successful response with the specified status code.
        /// </summary>
        /// <typeparam name="T">The type of the data in the response.</typeparam>
        /// <param name="statusCode">The HTTP status code of the response.</param>
        /// <returns>A new instance of <see cref="Response{T}"/> representing a successful response.</returns>
        public static Response<T> Success(HttpStatusCode statusCode)
        {
            return new Response<T> { StatusCode = statusCode };
        }

        /// <summary>
        /// Creates a failed response with the specified status code and error message.
        /// </summary>
        /// <typeparam name="T">The type of the data in the response.</typeparam>
        /// <param name="statusCode">The HTTP status code of the response.</param>
        /// <param name="message">The error message to include in the response.</param>
        /// <returns>A new instance of <see cref="Response{T}"/> representing a failed response.</returns>
        public static Response<T> Fail(HttpStatusCode statusCode, string message)
        {
            return new Response<T> { StatusCode = statusCode, Message = new List<string> { message }, Error = true };
        }

        /// <summary>
        /// Creates a failed response with the specified status code, error message, and data.
        /// </summary>
        /// <typeparam name="T">The type of the data in the response.</typeparam>
        /// <param name="statusCode">The HTTP status code of the response.</param>
        /// <param name="message">The error message to include in the response.</param>
        /// <param name="data">The data to include in the response.</param>
        /// <returns>A new instance of <see cref="Response{T}"/> representing a failed response.</returns>
        public static Response<T> Fail(HttpStatusCode statusCode, string message, T data)
        {
            return new Response<T> { StatusCode = statusCode, Message = new List<string> { message }, Error = true, Data = data };
        }
    }
}
