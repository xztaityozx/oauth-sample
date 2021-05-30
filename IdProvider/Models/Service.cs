using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace IdProvider.Models
{
    public record Service(int Id, string Uri, string ServiceName){}
}
