using MediatR;

namespace Consumer.Models
{
    public class Notification : IRequest
    {
        public string Table { get; set; }
        public string Operation { get; set; }
        public Person Data { get; set; }
    }
    public class Person
    {
        public int? Id { get; set; }
        public string Name { get; set; }
    }
}