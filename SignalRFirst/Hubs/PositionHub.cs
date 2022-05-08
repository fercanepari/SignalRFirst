using Microsoft.AspNetCore.SignalR;

namespace SignalRFirst.Hubs
{
    public class PositionHub : Hub
    {
        public static List<WebClientInfo> ConnectedClients = new List<WebClientInfo>();

        public PositionHub()
        {
        }

        public override Task OnConnectedAsync()
        {
            Console.WriteLine("Se conecto el cliente: " + Context.ConnectionId);
            ConnectedClients.Add(new WebClientInfo(Context.ConnectionId));
            updateTotalClients(Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                var clientToRemove = ConnectedClients.Where(c => c.ConnectionId == Context.ConnectionId).FirstOrDefault();
                if (clientToRemove != null)
                {
                    ConnectedClients.Remove(clientToRemove);
                }
                updateTotalClients(Context.ConnectionId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException?.Message);
            }
            return base.OnDisconnectedAsync(exception);

        }

        public async Task SendPosition(int left, int top)
        {
            //Console.WriteLine(left + " - " + top);
            if(left < 0)
            {
                left = 0;
            }
            if (top < 0)
            {
                top = 0;
            }

            await Clients.Others.SendAsync("ReceivePosition", left, top);
        }

        public async Task ValidateClient(string connectionId)
        {
            Console.WriteLine("Valido la conexion desde el cliente");
            var clientToValidate = ConnectedClients.Where(c => c.ConnectionId == Context.ConnectionId).FirstOrDefault();
            if (clientToValidate != null)
            {
                clientToValidate.IsValidated = true;
            }

            //Elimino clientes no validados
            //ConnectedClients.RemoveAll(cn => cn.IsValidated == false);
        }

        private async Task updateTotalClients(string connectionId)
        {
            Console.WriteLine("Actualizo totales clientes");
            List<WebClientInfo> clientsToRemove = new List<WebClientInfo>();
            var count = Context.Items.Count;

            //ConnectedClients.RemoveAll(cn => cn.IsValidated == false);
            ConnectedClients.ForEach(cnn => {
                //Console.WriteLine(cnn.ConnectionId);
                //Console.WriteLine(cnn.IsValidated);
            });


            var validatedClients = ConnectedClients.Where(c => c.IsValidated).ToList();

            Console.WriteLine(validatedClients.Count + 1);

            var result = Math.Truncate(Decimal.Floor(ConnectedClients.Count / 2));
            Console.WriteLine(result);
            await Clients.Others.SendAsync("ReceiveConnectedClients", result);

            await Clients.Caller.SendAsync("ReceiveConnectionId", connectionId);
        }

    }

    public class WebClientInfo
    {
        public string? ConnectionId { get; set; }
        //public string Name { get; set; }
        public bool IsValidated { get; set; }

        public WebClientInfo()
        {
            IsValidated = false;
        }

        public WebClientInfo(string connectionId)
        {
            this.ConnectionId = connectionId;
            IsValidated = false;
        }
    }
}
