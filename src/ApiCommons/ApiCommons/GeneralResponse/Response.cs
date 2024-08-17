using ApiCommons.Result;
using Microsoft.AspNetCore.Mvc;
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
        /// Converts the current <see cref="Response{T}"/> instance to an <see cref="IActionResult"/>.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> representing the response.</returns>
        public IActionResult ToActionResult()
        {
            return (ObjectResult)this;
        }

        /// <summary>
        /// Creates a successful response with the specified status code, data, and message.
        /// </summary>
        /// <typeparam name="T">The type of the data in the response.</typeparam>
        /// <param name="statusCode">The HTTP status code of the response.</param>
        /// <param name="data">The data to include in the response.</param>
        /// <param name="message">The message to include in the response.</param>
        /// <returns>A new instance of <see cref="Response{T}"/> representing a successful response.</returns>
        public static Response<T> Success(HttpStatusCode statusCode, T? data, string message)
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
        public static Response<T> Success(HttpStatusCode statusCode, T? data)
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
        public static Response<T> Fail(HttpStatusCode statusCode, string message, T? data)
        {
            return new Response<T> { StatusCode = statusCode, Message = new List<string> { message }, Error = true, Data = data };
        }

        /// <summary>
        /// Implicitly converts a <see cref="Response{T}"/> instance to an <see cref="ObjectResult"/>.
        /// </summary>
        /// <param name="response">The response to convert.</param>
        /// <returns>An <see cref="ObjectResult"/> representing the response.</returns>
        public static implicit operator ObjectResult(Response<T> response) => new(response)
        {
            StatusCode = (int)response.StatusCode,
        };

        /// <summary>
        /// Implicitly converts a <see cref="Result{T, Exception}"/> to a <see cref="Response{T}"/>.
        /// </summary>
        /// <param name="result">The result to convert.</param>
        /// <returns>A <see cref="Response{T}"/> representing the result, where success is mapped to the response's data and error is handled appropriately.</returns>
        public static implicit operator Response<T>(Result<T, Exception> result) => result.Match<Response<T>>(
            (value) => value,
            (error) => error
        );

        /// <summary>
        /// Implicitly converts a data object of type <typeparamref name="T"/> to a <see cref="Response{T}"/>.
        /// </summary>
        /// <param name="data">The data to include in the response.</param>
        /// <returns>A <see cref="Response{T}"/> representing a successful response with the provided data, 
        /// a status code of <see cref="HttpStatusCode.OK"/>, and a default message of "OK".</returns>
        public static implicit operator Response<T>(T? data) => Response<T>.Success(HttpStatusCode.OK, data, "OK");

        /// <summary>
        /// Implicitly converts an <see cref="Exception"/> to a <see cref="Response{T}"/> representing a failed response.
        /// </summary>
        /// <param name="ex">The exception to convert.</param>
        /// <returns>A <see cref="Response{T}"/> representing a failed response with a status code of <see cref="HttpStatusCode.BadRequest"/> 
        /// and the exception's message as the error message.</returns>
        public static implicit operator Response<T>(Exception ex) => Response<T>.Fail(HttpStatusCode.BadRequest, ex.Message);
    }
}
