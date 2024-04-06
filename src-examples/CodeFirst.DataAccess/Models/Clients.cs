using System;
using System.Collections.Generic;

namespace CodeFirst.DataAccess.Models;

public class Clients
{
    public int ClientId { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public DateTime? Birthday { get; set; }
        
    // navigational properties
    public virtual ICollection<Rentals> Rentals { get; set; }

    public Clients()
    {
    }

    public Clients(int clientId, string firstname, string lastname, DateTime? birthday)
    {
        ClientId = clientId;
        Firstname = firstname;
        Lastname = lastname;
        Birthday = birthday;
    }
}