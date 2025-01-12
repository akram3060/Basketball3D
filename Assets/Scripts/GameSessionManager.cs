// using UnityEngine;
// using System;
// using System.Net.WebSockets;
// using System.Text;
// using System.Threading;
// using System.Threading.Tasks;

// public class GameSessionManager : MonoBehaviour
// {
//     private ClientWebSocket webSocket;
//     private string serverUrl = "ws://localhost:8005/socket.io/?EIO=4&transport=websocket"; // Socket.IO connection
//     private CancellationTokenSource cts;

//     public string userId;
//     public string sessionId;

//     private void Start()
//     {
//         userId = SystemInfo.deviceUniqueIdentifier;
//         sessionId = Guid.NewGuid().ToString(); // Generate a session ID
//         cts = new CancellationTokenSource();
//         _ = ConnectToServer();
//     }

//     private async Task ConnectToServer(int maxRetries = 3)
//     {
//         for (int i = 0; i < maxRetries; i++)
//         {
//             try
//             {
//                 webSocket = new ClientWebSocket();
//                 webSocket.Options.SetRequestHeader("User-Agent", "Unity WebSocket Client");

//                 Debug.Log($"Attempt {i + 1} to connect to {serverUrl}");

//                 using (var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
//                 {
//                     await webSocket.ConnectAsync(new Uri(serverUrl), timeoutCts.Token);
//                 }

//                 if (webSocket.State == WebSocketState.Open)
//                 {
//                     Debug.Log("Successfully connected to WebSocket server");
//                     Debug.Log($"Connected with User ID: {userId}");
//                     _ = ReceiveMessages();
//                     await SendSocketIOHandshake();
//                     return;
//                 }
//                 else
//                 {
//                     Debug.LogWarning($"Connection attempt {i + 1} failed. WebSocket state: {webSocket.State}");
//                 }
//             }
//             catch (WebSocketException wse)
//             {
//                 Debug.LogError($"WebSocket error (Attempt {i + 1}): {wse.Message}");
//                 if (wse.InnerException != null)
//                 {
//                     Debug.LogError($"Inner Exception: {wse.InnerException.Message}");
//                 }
//             }
//             catch (Exception e)
//             {
//                 Debug.LogError($"Error connecting to WebSocket server (Attempt {i + 1}): {e.Message}");
//             }

//             if (i < maxRetries - 1)
//             {
//                 Debug.Log($"Waiting 5 seconds before retry {i + 2}...");
//                 await Task.Delay(5000, cts.Token);
//             }
//         }

//         Debug.LogError($"Failed to connect after {maxRetries} attempts");
//     }

//     private async Task ReceiveMessages()
//     {
//         byte[] buffer = new byte[1024];
//         while (webSocket.State == WebSocketState.Open && !cts.IsCancellationRequested)
//         {
//             try
//             {
//                 WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cts.Token);
//                 if (result.MessageType == WebSocketMessageType.Text)
//                 {
//                     string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
//                     ProcessMessage(message);
//                 }
//                 else if (result.MessageType == WebSocketMessageType.Close)
//                 {
//                     await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", cts.Token);
//                     Debug.Log("WebSocket closed by server");
//                     break;
//                 }
//             }
//             catch (Exception e)
//             {
//                 Debug.LogError($"Error receiving message: {e.Message}");
//                 break;
//             }
//         }
//     }

//     private void ProcessMessage(string message)
//     {
//         Debug.Log($"Received message: {message}");
//         if (message.StartsWith("40"))
//         {
//             Debug.Log("Received Socket.IO handshake response");
//         }
//         else if (message.StartsWith("42"))
//         {
//             string payload = message.Substring(2);
//             Debug.Log($"Received Socket.IO event: {payload}");
//         }
//     }

//     private async Task SendSocketIOHandshake()
//     {
//         string handshakeMessage = "40" + JsonUtility.ToJson(new { sid = sessionId });
//         await SendMessage(handshakeMessage);
//     }

//     public async Task UpdateScore(int localScore, int opponentScore)
//     {
//         string message = $"42[\"updateScore\",{{\"userId\":\"{userId}\",\"sessionId\":\"{sessionId}\",\"localScore\":{localScore},\"opponentScore\":{opponentScore}}}]";
//         await SendMessage(message);
//     }

//     public async Task EndGame(int localScore, int opponentScore)
//     {
//         string message = $"42[\"endGame\",{{\"userId\":\"{userId}\",\"sessionId\":\"{sessionId}\",\"localScore\":{localScore},\"opponentScore\":{opponentScore}}}]";
//         await SendMessage(message);
//     }

//     private async Task SendMessage(string message)
//     {
//         if (webSocket.State == WebSocketState.Open)
//         {
//             try
//             {
//                 byte[] buffer = Encoding.UTF8.GetBytes(message);
//                 await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, cts.Token);
//             }
//             catch (Exception e)
//             {
//                 Debug.LogError($"Error sending message: {e.Message}");
//             }
//         }
//         else
//         {
//             Debug.LogError("WebSocket is not open");
//         }
//     }

//     private async void OnDestroy()
//     {
//         cts.Cancel();
//         if (webSocket != null && webSocket.State == WebSocketState.Open)
//         {
//             try
//             {
//                 await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Game session ended", CancellationToken.None);
//             }
//             catch (Exception e)
//             {
//                 Debug.LogError($"Error closing WebSocket: {e.Message}");
//             }
//         }
//         cts.Dispose();
//     }
// }

using UnityEngine;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class GameSessionManager : MonoBehaviour
{
    private ClientWebSocket webSocket;
    private string serverUrl = "ws://localhost:8005/socket.io/?EIO=4&transport=websocket"; // Socket.IO connection
    private CancellationTokenSource cts;

    public string userId;
    public string sessionId;

    private void Start()
    {
        userId = SystemInfo.deviceUniqueIdentifier;
        sessionId = Guid.NewGuid().ToString(); // Generate a session ID
        cts = new CancellationTokenSource();
        _ = ConnectToServer();
    }

    private async Task ConnectToServer(int maxRetries = 3)
    {
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                webSocket = new ClientWebSocket();
                webSocket.Options.SetRequestHeader("User-Agent", "Unity WebSocket Client");

                Debug.Log($"Attempt {i + 1} to connect to {serverUrl}");

                using (var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
                {
                    await webSocket.ConnectAsync(new Uri(serverUrl), timeoutCts.Token);
                }

                if (webSocket.State == WebSocketState.Open)
                {
                    Debug.Log("Successfully connected to WebSocket server");
                    Debug.Log($"Connected with User ID: {userId}");
                    _ = ReceiveMessages();
                    await SendSocketIOHandshake();
                    return;
                }
                else
                {
                    Debug.LogWarning($"Connection attempt {i + 1} failed. WebSocket state: {webSocket.State}");
                }
            }
            catch (WebSocketException wse)
            {
                Debug.LogError($"WebSocket error (Attempt {i + 1}): {wse.Message}");
                Debug.LogError($"WebSocket error details: {wse.WebSocketErrorCode}");
                if (wse.InnerException != null)
                {
                    Debug.LogError($"Inner exception: {wse.InnerException.Message}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error connecting to WebSocket server (Attempt {i + 1}): {e.Message}");
            }

            if (i < maxRetries - 1)
            {
                Debug.Log($"Waiting 5 seconds before retry {i + 2}...");
                await Task.Delay(5000, cts.Token);
            }
        }

        Debug.LogError($"Failed to connect after {maxRetries} attempts");
    }

    private async Task ReceiveMessages()
    {
        byte[] buffer = new byte[1024];
        while (webSocket.State == WebSocketState.Open && !cts.IsCancellationRequested)
        {
            try
            {
                Array.Clear(buffer, 0, buffer.Length); // Clear the buffer before each receive
                WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cts.Token);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    ProcessMessage(message);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", cts.Token);
                    Debug.Log("WebSocket closed by server");
                    _ = ReconnectToServer(); // Attempt to reconnect
                    break;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error receiving message: {e.Message}");
                _ = ReconnectToServer(); // Attempt to reconnect
                break;
            }
        }
    }

    private async Task ReconnectToServer()
    {
        Debug.Log("Attempting to reconnect...");
        await ConnectToServer();
    }

    private void ProcessMessage(string message)
    {
        Debug.Log($"Received message: {message}");
        if (message.StartsWith("40"))
        {
            Debug.Log("Received Socket.IO handshake response");
        }
        else if (message.StartsWith("42"))
        {
            string payload = message.Substring(2);
            Debug.Log($"Received Socket.IO event: {payload}");
        }
    }

    private async Task SendSocketIOHandshake()
{
    string handshakeMessage = "40" + JsonUtility.ToJson(new { sid = sessionId });
    await SendMessage(handshakeMessage);
}

public async Task UpdateScore(int localScore, int opponentScore)
{
    string message = $"42[\"updateScore\",{{\"userId\":\"{userId}\",\"sessionId\":\"{sessionId}\",\"localScore\":{localScore},\"opponentScore\":{opponentScore}}}]";
    await SendMessage(message);
}

public async Task EndGame(int localScore, int opponentScore)
{
    string message = $"42[\"endGame\",{{\"userId\":\"{userId}\",\"sessionId\":\"{sessionId}\",\"localScore\":{localScore},\"opponentScore\":{opponentScore}}}]";
    await SendMessage(message);
}


    private async Task SendMessage(string message)
{
    if (webSocket.State == WebSocketState.Open)
    {
        try
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, cts.Token);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error sending message: {e.Message}");
            await ReconnectToServer(); // Attempt to reconnect if sending fails
        }
    }
    else
    {
        Debug.LogError("WebSocket is not open");
        await ReconnectToServer(); // Attempt to reconnect if the socket is not open
    }
}


    private async void OnDestroy()
    {
        cts.Cancel();
        if (webSocket != null && webSocket.State == WebSocketState.Open)
        {
            try
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Game session ended", CancellationToken.None);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error closing WebSocket: {e.Message}");
            }
        }
        cts.Dispose();
    }
}

