using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModel;

namespace Core.DomainServices {
    public interface IStudentRepository : IGenericRepository<Student> {
        // Define any members specific for student access functionality

        // E.g.
        IEnumerable<Student> GetThreeHighestGpa();
    }
}
