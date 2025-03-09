using Microsoft.AspNetCore.SignalR.Client;

namespace BlazorAuto.Shared.Services;

public class SignalRService
    {
    private readonly HubConnection _hubConnection;

    public SignalRService(string hubUrl)
        {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .Build();
        }

    public async Task StartAsync()
        {
        if (_hubConnection == null || _hubConnection.State == HubConnectionState.Disconnected)
            await _hubConnection.StartAsync();
        }

    public async Task SendMessageAsync(string user, string message)
        {
        await _hubConnection.InvokeAsync("SendMessage", user, message);
        }

    public void OnMessageReceived(Action<string, string> handler)
        {
        _hubConnection.On("ReceiveMessage", handler);
        }
    }

