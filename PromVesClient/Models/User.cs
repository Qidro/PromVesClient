using System;
using System.Collections.Generic;
using System.Text;

namespace PromVesClient.Models
{
    public class User
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Role { get; set; }
        public string PasswordHash { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
