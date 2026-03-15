using Microsoft.AspNetCore.Components.WebView;

namespace TCSA.KnowTheCity;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private void OnBlazorWebViewInitialized(object? sender, BlazorWebViewInitializedEventArgs e)
    {
#if DEBUG && WINDOWS
        if (e.WebView.CoreWebView2 is not null)
        {
            e.WebView.CoreWebView2.OpenDevToolsWindow();
        }
#endif
    }
}
