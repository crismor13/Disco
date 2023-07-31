using System.Net;
using Discoteque.Data.Dto;
namespace Discoteque.Business;

public static class Utils
{
    public static string GetLenghtInMinuteNotation(int seconds)
    {        
        var value = new DateTime();
        value = value.AddSeconds(seconds);
        return value.ToString("mm:ss");
    }

}

public static class Response<T>
{
    public static BaseMessage<T> BuildResponse(HttpStatusCode statusCode, string message)
    {
        return new BaseMessage<T>{
            Message = message,
            TotalElements = 0,
            StatusCode = statusCode                
        }; 
    }
    
    public static BaseMessage<T> BuildResponse(HttpStatusCode statusCode, string message, List<T> entitiesList)
    {
        return new BaseMessage<T>{
            Message = message,
            TotalElements = entitiesList.Count,
            StatusCode = statusCode,
            EntitiesList = entitiesList                
        }; 
    }
}