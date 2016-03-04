using System.Collections.Generic;
using Core.DomainModel;

namespace Core.DomainServices 
{
    public interface IStudentRepository : IGenericRepository<Student> 
    {
        // Define any members specific for student access functionality
        // E.g.

        Student GetStudent(int id);
        IEnumerable<Student> GetStudents();
        IEnumerable<Student> GetStudents(string name);
        IEnumerable<Student> GetThreeHighestGpa();

        void AddStudent(Student student);
        void AddStudents(IEnumerable<Student> students);

        void DeleteStudent(int id);
        void DeleteStudents(IEnumerable<int> ids);
    }
}
