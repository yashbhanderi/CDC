using Consumer.Models;
using Consumer.Repositories;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace Consumer.Hubs;

public class NotificationHub: Hub
{
    private readonly IHubContext<NotificationHub> _context;
    private readonly IPersonRepository _PersonRepository;
    
    public NotificationHub(IHubContext<NotificationHub> hubContext, IPersonRepository PersonRepository)
    {
        _context = hubContext;
        _PersonRepository =  PersonRepository;
    }
    
    public async Task SendNotifications(string message)
    {
        var data = JsonConvert.DeserializeObject<DebeziumMessage>(message);

        var payload = JsonConvert.SerializeObject(new Notification()
        {
            Operation = data.Payload.Op,
            Table = data.Payload.Source.Table,
            Data = data.Payload.Op == "d" ? new Person() { Id = data.Payload.Before.Id*(-1), Name = data.Payload.Before.Name} :  new Person() { Id = data.Payload.After.Id, Name = data.Payload.After.Name}
        });
        
        await _context.Clients.All.SendAsync("ReceivedNotifications", payload);
    }
    
    public async Task GetTableData()
    {
        var Persons = await _PersonRepository.GetAllPersonsAsync();
        await _context.Clients.All.SendAsync("ReceivedTableData", Persons);
    }
}