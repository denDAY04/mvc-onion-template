using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModel;
using Core.DomainServices;

namespace Infrastructure.DataAccess {
    public class StudentRepository : GenericRepository<Student>, IStudentRepository {

        public StudentRepository(ApplicationContext context) : base(context) {}
        
        public IEnumerable<Student> GetThreeHighestGpa() {
            // Handle query syntax within the repository; don't expose it 
            // to the controllers that use the repository.
            //return AsQueryable().OrderByDescending(s => s.AverageGrade).Take(3);
            return Get(null, students => students.OrderByDescending(student => student.AverageGrade), "", 1, 3);
        }
    }
}
