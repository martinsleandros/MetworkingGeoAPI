using System.Text.Json;
using MetworkingGeoAPI.Application.Interfaces;

namespace MetworkingGeoAPI.Application.Response
{
    public class BaseResponse<T> : IBaseResponse
    {
        public Errors<T> Errors { get; private set; }
        public bool IsOk { get; private set; }
        public T Data { get; private set; }
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }

        public void SetIsOk(T result)
        {
            IsOk = true;
            Data = result;
        }

        public void SetIsForbidden()
        {
            Errors = new Errors<T>();
            IsOk = false;
            Errors.IsForbbiden = true;
        }

        public void SetIsInternalError(T errors)
        {
            Errors = new Errors<T>();
            IsOk = false;
            Errors.IsForbbiden = false;
            Errors.data = errors;
        }
    }
    
    public class Errors<K>
    {
        public bool IsForbbiden { get; set; }
        public K data { get; set; }
    }
}