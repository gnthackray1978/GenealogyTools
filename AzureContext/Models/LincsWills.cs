﻿using System;
using System.Collections.Generic;

namespace AzureContext.Models
{


    public partial class LincsWills
    {
        public int Id { get; set; }
        public string DateString { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public string Collection { get; set; }
        public string Reference { get; set; }
        public string Place { get; set; }
        public int? Year { get; set; }
        public int? Typ { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Occupation { get; set; }
        public string Aliases { get; set; }
    }
}
