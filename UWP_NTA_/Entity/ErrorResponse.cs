﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_NTA_.Entity
{
    class ErrorResponse
    {
        public int status { get; set; }
        public string message { get; set; }
        public Dictionary<String, String> error { get; set; }
    }
}
