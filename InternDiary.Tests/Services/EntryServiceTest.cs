using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using InternDiary.Data;
using InternDiary.Models.Database;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace InternDiary.Tests.Services
{
    [TestClass]
    public class EntryServiceTest
    {
        private Mock<IRepository<Entry>> _mockEntryRepository;
        private IEntryService _entryService;
        private List<Entry> _entryList;
        private string _userId = "Jafca";

        [TestInitialize]
        public void Initialize()
        {
            _entryList = new List<Entry>() {
                new Entry() { Id = Guid.NewGuid(), AuthorId = _userId, Date = DateTime.Now.AddDays(-1), Rating = 1 },
                new Entry() { Id = Guid.NewGuid(), AuthorId = _userId, Date = DateTime.Now.AddDays(-2), Rating = 2 },
                new Entry() { Id = Guid.NewGuid(), AuthorId = _userId, Date = DateTime.Now.AddDays(-3), Rating = 3 }
            };

            _mockEntryRepository = new Mock<IRepository<Entry>>();
            _entryService = new EntryService(_mockEntryRepository.Object);
        }

        [TestMethod]
        public void GetEntryById_ReturnsEntry()
        {
            // Arrange
            var id = Guid.NewGuid();
            var entry = new Entry() { Id = id };
            _mockEntryRepository.Setup(m => m.Get(It.IsAny<Expression<Func<Entry, bool>>>(), null)).Returns(new List<Entry> { entry });

            // Act
            var results = _entryService.GetEntryById(id);

            // Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(id, results.Id);
        }

        [TestMethod]
        public void GetEntries_ReturnsEntries()
        {
            // Arrange
            _mockEntryRepository.Setup(m => m.Get(It.IsAny<Expression<Func<Entry, bool>>>(), null)).Returns(_entryList);

            // Act
            var results = _entryService.GetEntriesByUser(_userId);

            // Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(_entryList.Count, results.Count);
        }

        [TestMethod]
        public void AddEntry_CallsSave()
        {
            // Arrange
            var id = Guid.NewGuid();
            var entry = new Entry() { Id = id, AuthorId = _userId, Date = DateTime.Now.AddDays(-4), Rating = 1 };
            _mockEntryRepository.Setup(m => m.Add(entry));

            // Act
            _entryService.AddEntry(entry);

            // Assert
            Assert.AreEqual(id, entry.Id);
            _mockEntryRepository.Verify(m => m.Save(), Times.Once);
        }
    }
}
