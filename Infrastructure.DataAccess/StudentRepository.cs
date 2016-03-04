using System.Collections.Generic;
using System.Linq;
using Core.DomainModel;
using Core.DomainServices;

namespace Infrastructure.DataAccess {
    public class StudentRepository : GenericRepository<Student>, IStudentRepository {

        public StudentRepository(ApplicationContext context) : base(context) {}
        
        public IEnumerable<Student> GetThreeHighestGpa() {
            // Handle query syntax within the repository; don't expose it 
            // to the controllers that use the repository.
            return Get(null, students => students.OrderByDescending(student => student.AverageGrade), "", 1, 3);
        }
    }
}
