using Microsoft.JSInterop;

namespace D69soft.Client.Extension
{
    public static class IJSRuntimeExtensions
    {
        public static ValueTask Swal_Message(this IJSRuntime js, string title, string message, SweetAlertMessageType sweetAlertMessageType)
        {
            return js.InvokeVoidAsync("Swal.fire", title, message, sweetAlertMessageType.ToString());
        }

        public static ValueTask<bool> Swal_Confirm(this IJSRuntime js, string title, string message, SweetAlertMessageType sweetAlertMessageType)
        {
            return js.InvokeAsync<bool>("Swal_Confirm", title, message, sweetAlertMessageType.ToString());
        }

        public static ValueTask Swal_Alert(this IJSRuntime js, string title, SweetAlertMessageType sweetAlertMessageType)
        {
            return js.InvokeVoidAsync("Swal_Alert", title, sweetAlertMessageType.ToString());
        }

        public static ValueTask Toast_Alert(this IJSRuntime js, string title, SweetAlertMessageType sweetAlertMessageType)
        {
            return js.InvokeVoidAsync("Toast_Alert", title, sweetAlertMessageType.ToString());
        }
    }

    public enum SweetAlertMessageType
    {
        question, warning, error, success, info
    }
}