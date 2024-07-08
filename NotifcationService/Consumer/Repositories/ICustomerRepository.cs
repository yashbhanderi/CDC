using Consumer.Models;

namespace Consumer.Repositories;

public interface IPersonRepository
{
    Task<List<Person>> GetAllPersonsAsync();
}