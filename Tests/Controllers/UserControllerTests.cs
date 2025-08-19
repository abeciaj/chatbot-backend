using ChatSupport.Controllers;
using ChatSupport.Data;
using ChatSupport.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ChatSupport.Tests.Controllers
{
    public class UserControllerTests
    {
        private static Mock<DbSet<User>> CreateMockDbSet(List<User> data)
        {
            var queryable = data.AsQueryable();
            var mockSet = new Mock<DbSet<User>>();
            mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
            mockSet.Setup(m => m.Add(It.IsAny<User>())).Callback<User>(data.Add);
            mockSet.Setup(m => m.Remove(It.IsAny<User>())).Callback<User>(u => data.Remove(u));
            mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                .Returns<object[]>(ids =>
                {
                    var id = (long)ids[0];
                    var user = data.FirstOrDefault(u => u.user_id == id);
                    return new ValueTask<User>(user);
                });
            return mockSet;
        }

        private static UserController CreateController(List<User> users)
        {
            var mockSet = CreateMockDbSet(users);
            var mockContext = new Mock<UserDbContext>(new DbContextOptions<UserDbContext>());
            mockContext.Setup(c => c.users).Returns(mockSet.Object);
            mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            return new UserController(mockContext.Object);
        }

        [Fact]
        public async Task GetUsers_ReturnsAllUsers()
        {
            var users = new List<User> { new User { user_id = 1 }, new User { user_id = 2 } };
            var controller = CreateController(users);

            var result = await controller.GetUsers();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedUsers = Assert.IsAssignableFrom<List<User>>(okResult.Value);
            Assert.Equal(2, returnedUsers.Count);
        }

        [Fact]
        public async Task GetUserById_UserExists_ReturnsUser()
        {
            var users = new List<User> { new User { user_id = 1, name = "Test" } };
            var controller = CreateController(users);

            var result = await controller.GetUserById(1);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var user = Assert.IsType<User>(okResult.Value);
            Assert.Equal(1, user.user_id);
        }

        [Fact]
        public async Task GetUserById_UserNotFound_ReturnsNotFound()
        {
            var users = new List<User>();
            var controller = CreateController(users);

            var result = await controller.GetUserById(99);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task AddUser_ValidUser_ReturnsCreatedAtAction()
        {
            var users = new List<User>();
            var controller = CreateController(users);
            var newUser = new User { user_id = 10, name = "New" };

            var result = await controller.AddUser(newUser);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var user = Assert.IsType<User>(createdResult.Value);
            Assert.Equal(10, user.user_id);
            Assert.Single(users);
        }

        [Fact]
        public async Task AddUser_NullUser_ReturnsBadRequest()
        {
            var users = new List<User>();
            var controller = CreateController(users);

            var result = await controller.AddUser(null);

            Assert.IsType<BadRequestResult>(result.Result);
        }

        [Fact]
        public async Task UpdateUser_UserExists_UpdatesAndReturnsNoContent()
        {
            var users = new List<User> { new User { user_id = 1, name = "Old" } };
            var controller = CreateController(users);
            var updatedUser = new User { user_id = 1, name = "Updated", email = "a@b.com", password = "pw", created_on = System.DateTime.UtcNow };

            var result = await controller.UpdateUser(1, updatedUser);

            Assert.IsType<NoContentResult>(result);
            Assert.Equal("Updated", users[0].name);
        }

        [Fact]
        public async Task UpdateUser_UserNotFound_ReturnsNotFound()
        {
            var users = new List<User>();
            var controller = CreateController(users);
            var updatedUser = new User { user_id = 1 };

            var result = await controller.UpdateUser(1, updatedUser);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteUser_UserExists_RemovesAndReturnsNoContent()
        {
            var users = new List<User> { new User { user_id = 1 } };
            var controller = CreateController(users);

            var result = await controller.DeleteUser(1);

            Assert.IsType<NoContentResult>(result);
            Assert.Empty(users);
        }

        [Fact]
        public async Task DeleteUser_UserNotFound_ReturnsNotFound()
        {
            var users = new List<User>();
            var controller = CreateController(users);

            var result = await controller.DeleteUser(1);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}