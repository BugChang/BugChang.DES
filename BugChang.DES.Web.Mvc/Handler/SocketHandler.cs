using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Tools;
using BugChang.DES.Web.Mvc.Models.Index;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BugChang.DES.Web.Mvc.Handler
{
    public class SocketHandler
    {

        private readonly IOptions<CommonSettings> _options;

        public const int BufferSize = 4096;
        public WebSocket Socket;

        public SocketHandler(WebSocket socket, IOptions<CommonSettings> options)
        {
            Socket = socket;
            _options = options;
        }

        public async Task EchoLoop()
        {
            var buffer = new byte[BufferSize];
            var seg = new ArraySegment<byte>(buffer);

            while (Socket.State == WebSocketState.Open)
            {
                var incoming = await Socket.ReceiveAsync(seg, CancellationToken.None);
                string receivemsg = Encoding.UTF8.GetString(buffer, 0, incoming.Count);
                if (receivemsg == "getComputerInfo")
                {
                    var hardDiskName = _options.Value.HardDiskPartition;
                    var computerInfo = new ComputerInfoModel
                    {
                        HardDiskSpace = ComputerInfoHelper.GetHardDiskSpace(hardDiskName),
                        HardDiskUseageSpace = ComputerInfoHelper.GetHardDiskUseSpace(hardDiskName),
                        HardDiskUsageRate = ComputerInfoHelper.GetHardDiskUsageRate(hardDiskName),
                        CpuUsageRate = ComputerInfoHelper.GetCpuUsageRate(),
                        MemoryUsageRate = ComputerInfoHelper.GetMemoryUsageRate(),
                    };
                    var stringJson = JsonConvert.SerializeObject(computerInfo);
                    string userMsg = stringJson;
                    byte[] x = Encoding.UTF8.GetBytes(userMsg);
                    var outgoing = new ArraySegment<byte>(x);
                    await this.Socket.SendAsync(outgoing, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }

        public static async Task Acceptor(HttpContext hc, Func<Task> n)
        {
       
            try
            {
                if (!hc.WebSockets.IsWebSocketRequest)
                    return;

                var socket = await hc.WebSockets.AcceptWebSocketAsync();
                var options = (IOptions<CommonSettings>)hc.RequestServices.GetService(typeof(IOptions<CommonSettings>));
                var h = new SocketHandler(socket, options);
                await h.EchoLoop();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
         
        }

        /// <summary>
        /// branches the request pipeline for this SocketHandler usage
        /// </summary>
        /// <param name="app"></param>
        public static void Map(IApplicationBuilder app)
        {
            app.UseWebSockets();
            app.Use(Acceptor);
        }
    }
}
