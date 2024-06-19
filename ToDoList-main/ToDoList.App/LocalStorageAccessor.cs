
using System.Text;
using Microsoft.JSInterop;

namespace ToDoList.App
{
    public class LocalStorageAccessor : IAsyncDisposable
    {

        public async Task setValueAsync<T>(String key, T Value)
        {
            await WaitForReference();
            await _accessorJsRef.Value.InvokeAsync<T>("set", key, Value);

        }
        public async Task<T> GetValueAsync<T>(string key)
        {
            await WaitForReference();
            var result = await _accessorJsRef.Value.InvokeAsync<T>("get", key);
            return result;
        }

        public async Task RemoveValueAsync<T>(string key)
        {

            await WaitForReference();

            await _accessorJsRef.Value.InvokeVoidAsync("remove", key);

        }

        private Lazy<IJSObjectReference> _accessorJsRef = new();
        private readonly IJSRuntime _jsRuntime;

        public LocalStorageAccessor(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        private async Task WaitForReference()
        {
            if (!_accessorJsRef.IsValueCreated)
            {
                _accessorJsRef = new(await _jsRuntime.InvokeAsync<IJSObjectReference>
                ("import", "/js/LocalStorageAccessor.js"));
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_accessorJsRef.IsValueCreated)
            {
                await _accessorJsRef.Value.DisposeAsync();
            }
        }
    }
}