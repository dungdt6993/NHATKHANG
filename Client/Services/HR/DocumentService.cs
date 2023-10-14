using D69soft.Shared.Models.ViewModels.HR;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using System.Net.Http.Json;

namespace D69soft.Client.Services.HR
{
    public class DocumentService
    {
        private readonly HttpClient _httpClient;

        public DocumentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<DocumentTypeVM>> GetDocTypes(FilterVM _filterVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Document/GetDocTypes", _filterVM);

            return await response.Content.ReadFromJsonAsync<IEnumerable<DocumentTypeVM>>();
        }

        public async Task<List<DocumentVM>> GetDocs(FilterVM _filterVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Document/GetDocs", _filterVM);

            return await response.Content.ReadFromJsonAsync<List<DocumentVM>>();
        }

        public async Task<bool> UpdateDocument(DocumentVM _documentVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Document/UpdateDocument", _documentVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<int> UpdateDocType(DocumentTypeVM _documentTypeVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Document/UpdateDocType", _documentTypeVM);

            return await response.Content.ReadFromJsonAsync<int>();
        }

    }
}
