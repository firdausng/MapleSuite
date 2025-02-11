namespace leave.worker.Models;


public class Biographical
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
}

public class Contact
{
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string WorkEmail { get; set; }
    public string Address { get; set; }
}

public class Demographic
{
    public string Country { get; set; }
    public string Religion { get; set; }
    public string Enthic { get; set; }
    public string MaritalStatus { get; set; }
    public string Gender { get; set; }
}

public class Member
{
    public DateTime StartDate { get; set; }
    public Demographic Demographic { get; set; }
    public Biographical Biographical { get; set; }
    public Contact Contact { get; set; }
    public Guid Id { get; set; }
}

public class MemberRegisteredEvent
{
    public Member Member { get; set; }
}

public class EventData<T>
{
    public T Data { get; set; }
}