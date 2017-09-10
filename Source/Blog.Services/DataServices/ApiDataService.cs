namespace Blog.Services.DataServices
{
    using System.Collections.Generic;
    using System.Linq;
    using Models;

    public class ApiDataService : BaseDataService
    {
        private readonly Dictionary<string, ApiConnection> apisByName;

        public ApiDataService()
        {
            this.apisByName = new Dictionary<string, ApiConnection>();
        }

        public (string, string, string) GetApiParametersByName(string name)
        {
            var apiConnection = this.GetDbContext.ApiConnections.FirstOrDefault(api => api.ApiName == name);
            if (apiConnection == null)
            {
                return default((string, string, string));
            }

            if (!this.apisByName.ContainsKey(name))
            {
                this.apisByName.Add(name, apiConnection);
            }

            var apiValue = this.apisByName[name];
            return (apiValue.Key, apiValue.Secrete, apiValue.Token);
        }
    }
}