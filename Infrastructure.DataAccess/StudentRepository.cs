using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModel;
using Core.DomainServices;

namespace Infrastructure.DataAccess {
    public class StudentRepository : GenericRepository<Student>, IStudentRepository {

        public StudentRepository(ApplicationContext context) : base(context) {
        }

        public IEnumerable<Student> GetTenHighestGpa() {
            Expression<Func<Student, bool>> filter = s => s.Id > 0;
            Func<IQueryable<Student>, IOrderedQueryable<Student>> orderBy =
                students => students.OrderByDescending(s => s.AverageGrade);

            return Get(filter, orderBy, "", 1, 10);
        }
    }
}
