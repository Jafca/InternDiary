using InternDiary.Controllers;
using InternDiary.Data;
using InternDiary.Models.Database;
using InternDiary.ViewModels.SkillVM;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;

namespace InternDiary.Tests.Controllers
{
    [TestClass]
    public class SkillControllerTest
    {
        private Mock<DataAccess> _mockData;
        private List<Entry> _entryList;
        private List<Skill> _skillList;
        private List<SelectListItem> _selectListSkills;
        private List<EntrySkill> _entrySkills;
        private string _userId = "InvalidUser";
        private SkillController _skillController;

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
            _skillController = new SkillController(_mockData.Object);
        }

        [TestMethod]
        public void Index_ReturnsSkills()
        {
            // Arrange
            var date = DateTime.Now.Date;
            _mockData.Setup(m => m.SkillService.GetSkillsByUserAlphabetically(It.IsAny<string>())).Returns(_skillList);
            _mockData.Setup(m => m.EntrySkillService.CountEntrySkillsBySkillId(_skillList[0].Id)).Returns(1);
            _mockData.Setup(m => m.EntrySkillService.CountEntrySkillsBySkillId(_skillList[1].Id)).Returns(1);
            _mockData.Setup(m => m.EntrySkillService.CountEntrySkillsBySkillId(_skillList[2].Id)).Returns(0);

            // Act
            var result = _skillController.Index() as ViewResult;
            var viewModel = result.Model as SkillIndexViewModel;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, viewModel.SkillsFrequency.Count);
        }

        [TestMethod]
        public void Create_ReturnsResult()
        {
            // Arrange

            // Act
            var result = _skillController.Create() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Edit_NullId_BadRequest()
        {
            // Arrange
            Guid? id = null;

            // Act
            var result = _skillController.Edit(id) as HttpStatusCodeResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [TestMethod]
        public void Edit_NullSkill_HttpNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            Skill skill = null;
            _mockData.Setup(m => m.SkillService.GetSkillById(id)).Returns(skill);

            // Act
            var result = _skillController.Edit(id) as HttpNotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [TestMethod]
        public void Edit_InvalidUser_Forbidden()
        {
            // Arrange
            var id = Guid.NewGuid();
            var skill = new Skill { Id = id, AuthorId = _userId };
            _mockData.Setup(m => m.SkillService.GetSkillById(id)).Returns(skill);

            // Act
            var result = _skillController.Edit(id) as HttpStatusCodeResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual((int)HttpStatusCode.Forbidden, result.StatusCode);
        }

        [TestMethod]
        public void Edit_ValidId_ReturnsSkill()
        {
            // Arrange
            var skill = _skillList[0];
            _mockData.Setup(m => m.SkillService.GetSkillById(skill.Id)).Returns(skill);

            // Act
            var result = _skillController.Edit(skill.Id) as ViewResult;
            var model = result.Model as Skill;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(model);
        }

        [TestMethod]
        public void Delete_NullId_BadRequest()
        {
            // Arrange
            Guid? id = null;

            // Act
            var result = _skillController.Delete(id) as HttpStatusCodeResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [TestMethod]
        public void Delete_NullEntry_HttpNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            Skill skill = null;
            _mockData.Setup(m => m.SkillService.GetSkillById(id)).Returns(skill);

            // Act
            var result = _skillController.Delete(id) as HttpNotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [TestMethod]
        public void Delete_InvalidUser_Forbidden()
        {
            // Arrange
            var id = Guid.NewGuid();
            var skill = new Skill { Id = id, AuthorId = _userId };
            _mockData.Setup(m => m.SkillService.GetSkillById(id)).Returns(skill);

            // Act
            var result = _skillController.Delete(id) as HttpStatusCodeResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual((int)HttpStatusCode.Forbidden, result.StatusCode);
        }

        [TestMethod]
        public void Delete_ValidId_ReturnsSkill()
        {
            // Arrange
            var skill = _skillList[0];
            _mockData.Setup(m => m.SkillService.GetSkillById(skill.Id)).Returns(skill);

            // Act
            var result = _skillController.Delete(skill.Id) as ViewResult;
            var model = result.Model as Skill;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(model);
        }
    }
}

