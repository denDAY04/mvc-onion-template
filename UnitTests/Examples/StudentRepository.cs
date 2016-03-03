using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModel;
using Core.DomainServices;
using Faker;
using Infrastructure.DataAccess;
using NSubstitute;
using Xunit;

namespace UnitTests.Examples {
    public class StudentRepository {
        private readonly IStudentRepository _studentRepository;
        private readonly ApplicationContext _applicationContext;

        // Usage of Faker.Net to generate random variables.
        private readonly string _studentName1 = Name.First();
        private readonly string _studentName2 = Name.First();
        private readonly string _studentName3 = Name.First();
        private readonly string _studentName4 = Name.First();

        public StudentRepository() {
            // Create fake context and populate it with some fake data. (Add more data as needed.)
            var substituteDbSet = Substitute.For<IDbSet<Student>>();
            var studentList = new List<Student>()
            {
                new Student() {Id = 0, Name = _studentName1, AverageGrade = 5.0f},
                new Student() {Id = 1, Name = _studentName2, AverageGrade = 2.9f},
                new Student() {Id = 2, Name = _studentName3, AverageGrade = 3.1f},
                new Student() {Id = 3, Name = _studentName4, AverageGrade = 4.5f}
            }.AsQueryable();

            // Setup for the substitute to behave like a DbSet.
            substituteDbSet.Provider.Returns(studentList.Provider);
            substituteDbSet.Expression.Returns(studentList.Expression);
            substituteDbSet.ElementType.Returns(studentList.ElementType);
            substituteDbSet.GetEnumerator().Returns(studentList.GetEnumerator());

            // Fake application context to work on local data and not databse.
            _applicationContext = ApplicationContext.Create();

            // Concrete implementations for the unit test
            _studentRepository = new Infrastructure.DataAccess.StudentRepository(_applicationContext);
        }

        [Fact]
        public void GetThreeHighestGpa_Works_ReturnsThreeStudentsWithTopGpa() {
            // Arrange
            // Act
            var res = _studentRepository.GetThreeHighestGpa();
            var data = res.ToArray();
            //Assert 
            Assert.Equal(3, data.Length);
            Assert.Equal(1, data[0].Id);
            Assert.Equal(5.0f, data[0].AverageGrade);
            Assert.Equal(4, data[1].Id);
            Assert.Equal(4.5f, data[1].AverageGrade);
            Assert.Equal(11, data[2].Id);
            Assert.Equal(3.1f, data[2].AverageGrade);
        }
    }
}
