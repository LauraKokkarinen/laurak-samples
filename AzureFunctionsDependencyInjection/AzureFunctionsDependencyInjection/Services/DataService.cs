﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AzureFunctionsDependencyInjection.Services
{
    public class DataService : IDataService
    {
        private readonly IBlobCache _blobCache;

        private const string CacheKeyData = "CACHE_KEY_DATA";

        public DataService(IBlobCache blobCache)
        {
            _blobCache = blobCache;
        }

        public async Task<IEnumerable<JToken>> GetDataAsync()
        {
            return await GetCachedDataAsync() ?? await GetAndCacheDataAsync();
        }

        private async Task<IEnumerable<JToken>> GetCachedDataAsync()
        {
            string content = await _blobCache.GetBlobContentAsync(CacheKeyData);

            return content != null ? (IEnumerable<JToken>)JsonConvert.DeserializeObject(content) : null;
        }

        public async Task<IEnumerable<JToken>> GetAndCacheDataAsync()
        {
            var data = new List<JToken>(); // Get your data from the actual data source here

            await CacheData(data);

            return data;
        }

        private async Task CacheData(IEnumerable<JToken> data)
        {
            await _blobCache.SetBlobContentAsync(CacheKeyData, JsonConvert.SerializeObject(data));
        }
    }
}
