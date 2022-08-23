﻿using VOD.Common.DTOModels;

namespace VOD.Common.Services
{
    public interface IHttpClientFactoryService
    {
        Task<List<TResponse>> GetListAsync<TResponse>(string uri, string serviceName, string token = "") where TResponse : class;
        Task<TResponse> GetAsync<TResponse>(string uri, string serviceName, string token = "") where TResponse : class;
        Task<TResponse> PostAsync<TRequest, TResponse>(TRequest content, string uri, string serviceName, string token = "") where TResponse : class where TRequest : class;
        Task<TResponse> PutAsync<TRequest, TResponse>(TRequest content, string uri, string serviceName, string token = "") where TResponse : class where TRequest : class;
        Task<string> DeleteAsync(string uri, string serviceName, string token = "");

        Task<TokenDTO> CreateTokenAsync(LoginUserDTO user, string uri, string serviceName, string token = "");
        Task<TokenDTO> GetTokenAsync(LoginUserDTO user, string uri, string serviceName, string token = "");
    }
}
