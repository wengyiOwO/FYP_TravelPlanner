//using Microsoft.AspNet.SignalR;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace FYP_TravelPlanner.Hubs
//{
//    public class ChatHub : Hub
//    {
//        public void SendMessage(string senderId, string receiverId, string message, string timestamp)
//        {
//            Clients.Group(receiverId).ReceiveMessage(senderId, message, timestamp);
//        }

//        public override System.Threading.Tasks.Task OnConnected()
//        {
//            string userId = Context.QueryString["userId"];
//            if (!string.IsNullOrEmpty(userId))
//            {
//                Groups.Add(Context.ConnectionId, userId);
//            }
//            return base.OnConnected();
//        }

//        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
//        {
//            string userId = Context.QueryString["userId"];
//            if (!string.IsNullOrEmpty(userId))
//            {
//                Groups.Remove(Context.ConnectionId, userId);
//            }
//            return base.OnDisconnected(stopCalled);
//        }
//    }
//}