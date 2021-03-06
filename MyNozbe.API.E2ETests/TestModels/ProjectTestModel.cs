﻿using System.Collections.Generic;

namespace MyNozbe.API.E2ETests.TestModels
{
    public class ProjectTestModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<TaskTestModel> Tasks { get; set; }
    }
}