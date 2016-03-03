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
            return Get(s => s.Id > 0, students => students.OrderByDescending(s => s.AverageGrade), "", 1, 3);
        }
    }
}
