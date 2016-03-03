﻿using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Faker;
using Microsoft.AspNet.Identity;
using NSubstitute;
using Presentation.Web.Controllers;
using Presentation.Web.Mail;
using Presentation.Web.Models.Student;
using Xunit;
using Core.DomainServices;
using Core.DomainModel;
using AutoMapper;
using IndexViewModel = Presentation.Web.Models.Student.IndexViewModel;

namespace UnitTests.Examples
{
    public class FakeDbContext
    {
        // http://romiller.com/2012/02/14/testing-with-a-fake-dbcontext/
        // For replacing a database with a fake DbContext.

        private readonly StudentController _studentController;
        private readonly IStudentRepository _studentRepository;
        private readonly IUnitOfWork _unitOfWork;

        // Usage of Faker.Net to generate random variables.
        private readonly string _studentName1 = Name.First();
        private readonly string _studentName2 = Name.First();
        private readonly string _studentName3 = Name.First();
        private readonly string _studentName4 = Name.First();


        /// <summary>
        /// Constructor for the test cases.
        /// For each test case, the class is instantiated and disposed.
        /// The basic naming of a test comprises of three main parts:
        /// [UnitOfWork_StateUnderTest_ExpectedBehavior]
        /// Followed by //Arrange, //Act and //Assert
        /// </summary>
        public FakeDbContext()
        {
            // Example: https://msdn.microsoft.com/en-us/data/dn314429

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

            // Substitute the repository.
            _studentRepository = Substitute.For<IStudentRepository>();

            // Set the AsQueryable to return the DbSet
            // You may need to set up other functions depending on the test case.
            _studentRepository.AsQueryable().Returns(substituteDbSet.AsQueryable());

            // A substitute for the UnitOfWork, used when saving context.
            _unitOfWork = Substitute.For<IUnitOfWork>();

            // Link the UnitOfWork's model-specific repository to the mock version.
            _unitOfWork.StudentRepository.Returns(_studentRepository);

            // Substitute for mailservice.
            var mailService = Substitute.For<IIdentityMessageService>();

            // Substitute for mailHandler.
            var mailHandler = Substitute.For<IMailHandler>();

            // Pass the context to the controller, and use this for testing.
            _studentController = new StudentController(_unitOfWork, mailService, mailHandler);

            // The controller uses automapper.
            Mapper.CreateMap<NewStudentViewModel, Student>().ReverseMap();
        }

        [Fact]
        public void IndexStudentsByName_CallingTheMethod_ReturnsStudents()
        {
            // Arrange
            // Act
            var res = _studentController.IndexStudentsByName();
            // Assert
            Assert.True(res.FindIndex(d => d.Name == _studentName1) > -1);
            Assert.True(res.FindIndex(d => d.Name == _studentName2) > -1);
            Assert.True(res.FindIndex(d => d.Name == _studentName3) > -1);
            Assert.True(res.FindIndex(d => d.Name == _studentName4) > -1);
        }

        [Fact]
        public void IndexStudentsById_CallingTheMethod_ReturnsStudents()
        {
            // Arrange
            // Act
            var res = _studentController.IndexStudentsById();
            // Assert
            Assert.True(res[0].Id == 0);
            Assert.True(res[1].Id == 1);
            Assert.True(res[2].Id == 2);
            Assert.True(res[3].Id == 3);
        }

        [Fact]
        public void NewStudent_CanCreateNewStudent_RepositoryReceivedInsertCallAndUnitOfWorkSaved()
        {
            // Arrange
            var model = new NewStudentViewModel() { Name = Name.First() };
            // Act
            _studentController.NewStudent(model);
            // Assert
            _studentRepository.Received().Insert(Arg.Any<Student>());
            _unitOfWork.Received().Save();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void FindStudent_CanFindStudentWithId_ReturnsANotNullObject(int value)
        {
            // Arrange
            // Act
            var res = _studentController.FindStudent(value);
            // Assert
            Assert.NotNull(res);
            _studentRepository.Received().AsQueryable();
        }

        [Fact]
        public void FindStudent_CannotFindStudentWithNull_ReturnsJsonWithNullString()
        {
            // Arrange
            // Act
            var res = _studentController.FindStudent(null) as JsonResult;
            var data = res.Data as NewStudentViewModel;
            // Assert
            Assert.Equal("null", data.Name);
        }

        [Fact]
        public void FindStudent_CannotFindStudentThatDoesNotExist_ReturnHttpNotFound()
        {
            // Arrange
            // Act
            var res = _studentController.FindStudent(5);
            // Assert
            Assert.IsType<HttpNotFoundResult>(res);
        }

        [Fact]
        public void Index_ReturnsSelectListAndViewModel_TypesAndModelMatchesAndSelectedIdIsZeroAndNameIsNull()
        {
            // Arrange
            // Act
            var res = _studentController.Index() as ViewResult;
            var viewModel = res.Model as IndexViewModel;
            var selectList = res.ViewBag.StudentIds;
            // Assert
            Assert.NotNull(viewModel.PagedStudents);
            Assert.Null(viewModel.Name);
            Assert.IsType<int>(viewModel.SelectedId);
            Assert.Equal(viewModel.SelectedId, 0);
            Assert.IsType<EnumerableQuery<SelectListItem>>(selectList);
        }

        [Fact]
        public void _Students_TablePaging_PagedDataContainsAListOfStudentsAndNumberOfPagesIsZero()
        {
            // Arrange

            // Act
            var res = _studentController._Students(0) as PartialViewResult;
            var viewModel = res.Model as IndexViewModel;
            var pagedData = viewModel.PagedStudents;
            // Assert
            Assert.IsType<List<NewStudentViewModel>>(pagedData.Data);
            Assert.True(pagedData.NumberOfPages == 0);
        }

        [Fact]
        public void NewStudent_IsWorking_ReturnsAView()
        {
            // Arrange
            // Act
            var res = _studentController.NewStudent();
            // Assert
            Assert.IsType<ViewResult>(res);
        }

        [Fact]
        public void TestMail()
        {
            // Arrange
            // Act
            var res = _studentController.Mail();
            // Assert
            Assert.IsType<ViewResult>(res);
        }

        [Fact]
        public void SendMail()
        {
            // Arrange
            var model = new MailViewModel()
            {
                Subject = "",
                Email = "",
                Foo = "",
                Bar = "",
                Baz = "",
                Qux = ""
            };
            // Act
            _studentController.SendMail(model);
            // Assert

            //_mailHandler.Received().GetMailMessage(model, Arg.Any<string>);

        }
    }
}
