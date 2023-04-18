using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.DTO
{
    public class ReturnResult<T>
    {
        public ReturnResult()
        {
            ErrorList = new List<string>();
        }

        public bool IsSuccess { get; set; }
        public HttpCode HttpCode { get; set; }
        public T Data { get; set; }
        public List<string> ErrorList { get; set; }

        /// <summary>
        /// Set success result with data
        /// </summary>
        /// <param name="Data"></param>
        public void Success(T Data)
        {
            this.IsSuccess = true;
            this.HttpCode = HttpCode.Success;
            this.Data = Data;
        }

        /// <summary>
        /// Set Server Error result with error message
        /// </summary>
        /// <param name="Error"></param>
        public void ServerError(string Error)
        {
            this.IsSuccess = false;
            this.HttpCode = HttpCode.ServerError;
            this.ErrorList.Add(Error);
        }

        /// <summary>
        /// Set Server Error result with error message
        /// </summary>
        /// <param name="Errors"></param>
        public void ServerError(List<string> Errors)
        {
            this.IsSuccess = false;
            this.HttpCode = HttpCode.ServerError;
            this.ErrorList = Errors;
        }

        /// <summary>
        /// Set Default Not Found result
        /// </summary>
        public void DefaultNotFound()
        {
            this.IsSuccess = false;
            this.HttpCode = HttpCode.NotFound;
            this.ErrorList.Add("لا يوجد بيانات");
        }

        /// <summary>
        /// Set Not Found result with error message
        /// </summary>
        /// <param name="Error"></param>
        public void NotFound(string Error)
        {
            this.IsSuccess = false;
            this.HttpCode = HttpCode.NotFound;
            this.ErrorList.Add(Error);
        }

        /// <summary>
        /// Set Not Found result with error message
        /// </summary>
        /// <param name="Error"></param>
        public void NotFound(List<string> Errors)
        {
            this.IsSuccess = false;
            this.HttpCode = HttpCode.NotFound;
            this.ErrorList = Errors;
        }

        /// <summary>
        /// Set BadRequest result with error message
        /// </summary>
        /// <param name="Errors"></param>
        public void BadRequest(List<string> Errors)
        {
            this.IsSuccess = false;
            this.HttpCode = HttpCode.BadRequest;
            this.ErrorList = Errors;
        }

        /// <summary>
        /// Set BadRequest result with error message
        /// </summary>
        /// <param name="Error"></param>
        public void BadRequest(string Error)
        {
            this.IsSuccess = false;
            this.HttpCode = HttpCode.BadRequest;
            this.ErrorList.Add(Error);
        }
    }
}
