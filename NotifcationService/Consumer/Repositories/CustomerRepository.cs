using Consumer.Contexts;
using Consumer.Models;
using Microsoft.EntityFrameworkCore;

namespace Consumer.Repositories;

public class PersonRepository: IPersonRepository
{
    private readonly DataContext _dataContext;

    public PersonRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
    
    public async Task<List<Person>> GetAllPersonsAsync()
    {
        return await _dataContext.Persons.ToListAsync();
    }
}