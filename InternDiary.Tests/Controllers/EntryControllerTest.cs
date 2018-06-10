using InternDiary.Controllers;
using InternDiary.Data;
using InternDiary.Models.Database;
using InternDiary.ViewModels.EntryVM;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;

namespace InternDiary.Tests.Controllers
{
    [TestClass]
    public class EntryControllerTest
    {
        private Mock<DataAccess> _mockData;
        private List<Entry> _entryList;
        private List<Skill> _skillList;
        private List<SelectListItem> _selectListSkills;
        private List<EntrySkill> _entrySkills;
        private string _userId = "InvalidUser";
        private EntryController _entryController;

        [TestInitialize]
        public void Initialize()
        {
            _entryList = new List<Entry>()
            {
                new Entry() { Id = Guid.NewGuid(), Date = DateTime.Now.Date.AddDays(-1), Rating = 1 },
                new Entry() { Id = Guid.NewGuid(), Date = DateTime.Now.Date.AddDays(-2), Rating = 2 },
                new Entry() { Id = Guid.NewGuid(), Date = DateTime.Now.Date.AddDays(-3), Rating = 3 }
            };

            _skillList = new List<Skill>()
            {
                new Skill{ Id=Guid.NewGuid(), Text= "Skill1" },
                new Skill{ Id=Guid.NewGuid(), Text= "Skill2" },
                new Skill{ Id=Guid.NewGuid(), Text= "Skill3" }
            };

            _selectListSkills = new List<SelectListItem>
            {
                new SelectListItem { Text = _skillList[0].Text, Value = _skillList[0].Text },
                new SelectListItem { Text = _skillList[1].Text, Value = _skillList[1].Text },
                new SelectListItem { Text = _skillList[2].Text, Value = _skillList[2].Text }
            };

            _entrySkills = new List<EntrySkill>
            {
                new EntrySkill { Id = Guid.NewGuid(), EntryId = _entryList[0].Id, SkillId = _skillList[0].Id },
                new EntrySkill { Id = Guid.NewGuid(), EntryId = _entryList[0].Id, SkillId = _skillList[1].Id },
            };

            var _mockEntryService = new Mock<IEntryService>();
            var _mockEntrySkillService = new Mock<IEntrySkillService>();
            var _mockSkillService = new Mock<ISkillService>();
            _mockData = new Mock<DataAccess>(_mockEntryService.Object, _mockEntrySkillService.Object, _mockSkillService.Object);
            _entryController = new EntryController(_mockData.Object);
        }

        [TestMethod]
        public void Create_ValidDate_UsesDate()
        {
            // Arrange
            var date = DateTime.Now.Date;
            _mockData.Setup(m => m.EntryService.GetEntriesByUser(It.IsAny<string>())).Returns(_entryList);
            _mockData.Setup(m => m.SkillService.GetSelectListOfSkillsByUserAlphabetically(It.IsAny<string>())).Returns(_selectListSkills);

            // Act
            var result = _entryController.Create(date.ToShortDateString()) as ViewResult;
            var viewModel = result.Model as EntryCreateViewModel;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(date, viewModel.Entry.Date);
        }

        [TestMethod]
        public void Create_InvalidDate_UsesToday()
        {
            // Arrange
            var invalidDate = "invalid date";
            _mockData.Setup(m => m.EntryService.GetEntriesByUser(It.IsAny<string>())).Returns(_entryList);
            _mockData.Setup(m => m.SkillService.GetSelectListOfSkillsByUserAlphabetically(It.IsAny<string>())).Returns(_selectListSkills);

            // Act
            var result = _entryController.Create(invalidDate) as ViewResult;
            var viewModel = result.Model as EntryCreateViewModel;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(DateTime.Now.Date, viewModel.Entry.Date);
        }

        [TestMethod]
        public void Create_ExistingEntry_RedirectToEdit()
        {
            // Arrange
            var entry = _entryList[0];
            _mockData.Setup(m => m.EntryService.GetEntriesByUser(It.IsAny<string>())).Returns(_entryList);

            // Act
            var result = _entryController.Create(entry.Date.ToShortDateString()) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);

            Assert.AreEqual(result.RouteValues["id"], entry.Id);
            Assert.AreEqual(result.RouteValues["action"], "Edit");
        }

        [TestMethod]
        public void Edit_NullId_BadRequest()
        {
            // Arrange
            Guid? id = null;

            // Act
            var result = _entryController.Edit(id) as HttpStatusCodeResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [TestMethod]
        public void Edit_NullEntry_HttpNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            Entry entry = null;
            _mockData.Setup(m => m.EntryService.GetEntryById(id)).Returns(entry);

            // Act
            var result = _entryController.Edit(id) as HttpNotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [TestMethod]
        public void Edit_InvalidUser_Forbidden()
        {
            // Arrange
            var id = Guid.NewGuid();
            var entry = new Entry { Id = id, AuthorId = _userId };
            _mockData.Setup(m => m.EntryService.GetEntryById(id)).Returns(entry);

            // Act
            var result = _entryController.Edit(id) as HttpStatusCodeResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual((int)HttpStatusCode.Forbidden, result.StatusCode);
        }

        [TestMethod]
        public void Edit_ValidId_CurrentSkillsSelected()
        {
            // Arrange
            var entry = _entryList[0];
            _mockData.Setup(m => m.EntryService.GetEntryById(entry.Id)).Returns(entry);
            _mockData.Setup(m => m.SkillService.GetSelectListOfSkillsByUserAlphabetically(It.IsAny<string>())).Returns(_selectListSkills);
            _mockData.Setup(m => m.EntrySkillService.GetEntrySkillsByEntryId(entry.Id)).Returns(_entrySkills);
            _mockData.Setup(m => m.SkillService.GetSkillById(_skillList[0].Id)).Returns(_skillList[0]);
            _mockData.Setup(m => m.SkillService.GetSkillById(_skillList[1].Id)).Returns(_skillList[1]);
            _mockData.Setup(m => m.SkillService.GetSkillById(_skillList[2].Id)).Returns(_skillList[2]);

            // Act
            var result = _entryController.Edit(entry.Id) as ViewResult;
            var viewModel = result.Model as EntryCreateViewModel;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(viewModel.SavedSkills[0].Selected);
            Assert.IsTrue(viewModel.SavedSkills[1].Selected);
            Assert.IsFalse(viewModel.SavedSkills[2].Selected);
        }

        [TestMethod]
        public void Delete_NullId_BadRequest()
        {
            // Arrange
            Guid? id = null;

            // Act
            var result = _entryController.Delete(id) as HttpStatusCodeResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [TestMethod]
        public void Delete_NullEntry_HttpNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            Entry entry = null;
            _mockData.Setup(m => m.EntryService.GetEntryById(id)).Returns(entry);

            // Act
            var result = _entryController.Delete(id) as HttpNotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [TestMethod]
        public void Delete_InvalidUser_Forbidden()
        {
            // Arrange
            var id = Guid.NewGuid();
            var entry = new Entry { Id = id, AuthorId = _userId };
            _mockData.Setup(m => m.EntryService.GetEntryById(id)).Returns(entry);

            // Act
            var result = _entryController.Delete(id) as HttpStatusCodeResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual((int)HttpStatusCode.Forbidden, result.StatusCode);
        }

        [TestMethod]
        public void Delete_ValidId_CurrentSkillsSelected()
        {
            // Arrange
            var entry = _entryList[0];
            var skillsLearnt = "Skill1, Skill2, Skill3";
            _mockData.Setup(m => m.EntryService.GetEntryById(entry.Id)).Returns(entry);
            _mockData.Setup(m => m.SkillService.GetSkillsLearntByEntryId(entry.Id)).Returns(skillsLearnt);

            // Act
            var result = _entryController.Delete(entry.Id) as ViewResult;
            var viewModel = result.Model as EntryDeleteViewModel;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(skillsLearnt, viewModel.SkillsLearnt);
        }

        [TestMethod]
        public void GetCalendarEntries_ReturnsJson()
        {
            // Arrange
            _mockData.Setup(m => m.EntryService.GetEntriesByUser(It.IsAny<string>())).Returns(_entryList);
            _mockData.Setup(m => m.EntrySkillService.CountEntrySkillsByEntryId(_entryList[0].Id)).Returns(2);
            _mockData.Setup(m => m.EntrySkillService.CountEntrySkillsByEntryId(_entryList[1].Id)).Returns(1);
            _mockData.Setup(m => m.EntrySkillService.CountEntrySkillsByEntryId(_entryList[2].Id)).Returns(0);
            var expectedJson = 
                "[" +
                "{\"title\":\"\\n[Skills Count: 2]\",\"allDay\":true,\"start\":\"" + _entryList[0].Date.AddDays(1).ToString("yyyy-MM-ddTHH:mm:ssK") + "\",\"url\":\"/Entry/Edit/" + _entryList[0].Id + "\",\"color\":\"CRIMSON\",\"textColor\":\"WHITE\"}," +
                "{\"title\":\"\\n[Skills Count: 1]\",\"allDay\":true,\"start\":\"" + _entryList[1].Date.AddDays(1).ToString("yyyy-MM-ddTHH:mm:ssK") + "\",\"url\":\"/Entry/Edit/" + _entryList[1].Id + "\",\"color\":\"DARKORANGE\",\"textColor\":\"WHITE\"}," +
                "{\"title\":\"\\n[Skills Count: 0]\",\"allDay\":true,\"start\":\"" + _entryList[2].Date.AddDays(1).ToString("yyyy-MM-ddTHH:mm:ssK") + "\",\"url\":\"/Entry/Edit/" + _entryList[2].Id + "\",\"color\":\"GOLD\",\"textColor\":\"WHITE\"}" +
                "]";

            // Act
            var result = _entryController.GetCalendarEntries() as JsonResult;
            var data = JsonConvert.SerializeObject(result.Data);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedJson, data);
        }
    }
}
