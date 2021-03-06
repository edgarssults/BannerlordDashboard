﻿using Ed.Bannerlord.Dashboard.Models.Widgets;
using SuperSocket.WebSocket;
using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace Ed.Bannerlord.Dashboard.Logic.Widgets
{
    /// <summary>
    /// A widget for displaying kingdom noble charts on the dashboard.
    /// </summary>
    public class KingdomNoblesWidget : WidgetBase
    {
        /// <summary>
        /// A widget for displaying kingdom noble charts on the dashboard.
        /// </summary>
        /// <param name="server">WebSocket server to send data to.</param>
        public KingdomNoblesWidget(WebSocketServer server) : base(server)
        {
        }

        /// <summary>
        /// Registers widget events.
        /// </summary>
        public override void RegisterEvents()
        {
            // Update all sessions about kingdom noble count every "hour"
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(() =>
            {
                foreach (var session in Server.GetAllSessions())
                {
                    SendUpdate(session);
                }
            }));
        }

        /// <summary>
        /// Initializes a widget.
        /// </summary>
        /// <param name="session">The session to initialize the widget for.</param>
        public override void Init(WebSocketSession session)
        {
            // Send the first update
            SendUpdate(session);
        }

        /// <summary>
        /// Sends a dashboard update to a WebSocket session.
        /// </summary>
        /// <param name="session">The session to send the update to.</param>
        private void SendUpdate(WebSocketSession session)
        {
            var model = new KingdomNoblesModel
            {
                Kingdoms = Campaign.Current.Kingdoms
                    .Select(k => new KingdomNoblesItem
                    {
                        Name = k.Name.ToString(),
                        Nobles = k.Nobles.Count(),
                        PrimaryColor = Color.FromUint(k.Color).ToString(),
                        SecondaryColor = Color.FromUint(k.Color2).ToString(),
                    })
                    .ToList(),
            };
            session.Send(new ArraySegment<byte>(model.ToByteArray()));
        }
    }
}
