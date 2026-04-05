using Microsoft.AspNetCore.SignalR;

namespace SportSkin.Web.Hubs
{
    public class SubastaHub : Hub
    {
        //Añade la conexión del navegador en ejecucción a determinado grupo de SignalR.
        public async Task UnirseASubasta(int idSubasta)
        {
            //Distribuye las conexiones en grupos a partir del Id de la Subasta.
            await Groups.AddToGroupAsync(Context.ConnectionId, $"subasta-{idSubasta}");
        }            
    }
}
