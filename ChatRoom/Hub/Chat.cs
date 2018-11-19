// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//https://docs.microsoft.com/en-us/aspnet/core/signalr/hubs?view=aspnetcore-2.1
using Microsoft.AspNetCore.SignalR;

namespace Microsoft.Azure.SignalR.Samples.ChatRoom
{
    //The Chat class name becomes 
    public class Chat : Hub
    {
        public void BroadcastMessage(string name, string message)
        {            
            Clients.All.SendAsync("broadcastMessage", name, message);
        }

        public void Echo(string name, string message)
        {
            Clients.Client(Context.ConnectionId).SendAsync("echo", name, message + " (echo from server)");
        }
    }
}
