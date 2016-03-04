﻿using System.Collections.Generic;
using Core.DomainModel;

namespace Core.DomainServices {
    public interface IStudentRepository : IGenericRepository<Student> {
        // Define any members specific for student access functionality

        // E.g.
        IEnumerable<Student> GetThreeHighestGpa();
    }
}
