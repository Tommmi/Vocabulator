using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vocabulator
{
    internal class Config
    {
        public string ApiKey { get; set; }

        public Config(string apiKey)
        {
            ApiKey = apiKey;
        }

        public Config()
        {

        }
    }
}
