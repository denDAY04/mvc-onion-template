using System.Collections.Generic;
using System.Linq;
using Core.DomainModel;
using Core.DomainServices;

namespace Infrastructure.DataAccess 
{
    public class StudentRepository : GenericRepository<Student>, IStudentRepository 
    {

        // Handle query syntax within the repository; don't expose it 
        // to the controllers that use the repository.

        public StudentRepository(ApplicationContext context) : base(context) { }

        /// <summary>
        /// Find a student by ID. 
        /// </summary>
        /// <param name="id">The student's unique id.</param>
        /// <returns>The found student, or null.</returns>
        public Student GetStudent(int id) {
            return GetByKey(id);
        }

        /// <summary>
        /// Get all students.
        /// </summary>
        /// <returns>A collection of all students.</returns>
        public IEnumerable<Student> GetStudents() {
            return Get();
        }

        /// <summary>
        /// Get all students with a given name.
        /// Note that if the parameter is null, null is returned. 
        /// </summary>
        /// <param name="name">Name to match against.</param>
        /// <returns>All found matches, an empty collection if none were found, or null if argument was null.</returns>
        public IEnumerable<Student> GetStudents(string name) {
            return name != null ? Get(filter: (s => s.Name == name)) : null;
        }

        /// <summary>
        /// Get the three students with the highest GPA (Grade Point Average).
        /// </summary>
        /// <returns>A collection of (up to three) students with the highest GPA.</returns>
        public IEnumerable<Student> GetThreeHighestGpa() 
        {
            return Get(null, students => students.OrderByDescending(student => student.AverageGrade), "", 1, 3);
        }

        /// <summary>
        /// Add a new student.
        /// </summary>
        /// <param name="student">The new student.</param>
        public void AddStudent(Student student) {
            Insert(student);
        }

        /// <summary>
        /// Add a number of students.
        /// </summary>
        /// <param name="students">Collection of new students.</param>
        public void AddStudents(IEnumerable<Student> students) {
            foreach (var student in students) {
                Insert(student);
            }
        }

        /// <summary>
        /// Remove a student. 
        /// </summary>
        /// <param name="id">Unique ID of the student to be removed.</param>
        public void DeleteStudent(int id) {
            DeleteByKey(id);
        }

        /// <summary>
        /// Remove a number of students.
        /// </summary>
        /// <param name="ids">Collection of unique IDs of the students to be removed.</param>
        public void DeleteStudents(IEnumerable<int> ids) {
            foreach (var id in ids) {
                DeleteByKey(id);
            }
        }
    }
}
