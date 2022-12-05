using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UserSample.Test
{

    public class UserRepositoryTest
    {
        public readonly IUserRepository MockUserRepository;

        public UserRepositoryTest()
        {
            // Test metotları genelinde kullanacağımız User listesi
            var userList = new List<User>
            {
            new User {Id=1,FirstName="User1",LastName="User1LastName" },
            new User {Id=2,FirstName="User2",LastName="User2LastName" },
            new User {Id=3,FirstName="User3",LastName="User3LastName" }
            };

            // Mock the Products Repository using Moq
            var mockUserRepository = new Mock<IUserRepository>();

            // GetAll metodu için setup işlemi
            mockUserRepository.Setup(mr => mr.GetAll()).Returns(userList);

            // GetById metodu için setup işlemi
            mockUserRepository.Setup(mr => mr.GetById(It.IsAny<int>())).Returns((int i) => userList.Single(x => x.Id == i));

            // Insert için setup işlemi
            mockUserRepository.Setup(mr => mr.Insert(It.IsAny<User>())).Callback(
            (User target) =>
            {
                userList.Add(target);
            });

            // Update için setup işlemi
            mockUserRepository.Setup(mr => mr.Update(It.IsAny<User>())).Callback(
            (User target) =>
            {
                var original = userList.Where(q => q.Id == target.Id).Single();

                if (original == null)
                {
                    throw new InvalidOperationException();
                }

                original.FirstName = target.FirstName;
                original.LastName = target.LastName;

            });

            // Test metotlarından erişilebilmesi için global olarak tanımladığımız MockUserRepository'e yukarıdaki setup işlemlerini atıyoruz
            this.MockUserRepository = mockUserRepository.Object;

        }
        [Fact]
            public void GetAll_Than_Check_Count_Test()
            {
                var expected = this.MockUserRepository.GetAll().Count;

                Assert.NotNull(expected);// Test not null
                Assert.True(expected > 0);// Test GetAll returns user objects
            }

        [Fact]
        public void GetById_Than_Check_Correct_Object_Test()
        {
            var actual = new User { Id = 2, FirstName = "User2", LastName = "User2LastName" };

            var expected = this.MockUserRepository.GetById(2);

            Assert.NotNull(expected); // Test is not null
            //Assert.IsType(expected, typeof(User)); // Test type
            Assert.IsType<User>(expected); // Test type
            Assert.Equal(actual.Id, expected.Id); // test correct object found
        }

        [Fact]
        public void Insert_User_Than_Check_GetAll_Count_Test()
        {
            var actual = this.MockUserRepository.GetAll().Count + 1;

            var user = new User { Id = 4, FirstName = "User4", LastName = "User4LastName" };

            this.MockUserRepository.Insert(user);

            var expected = this.MockUserRepository.GetAll().Count;

            Assert.Equal(actual, expected);
        }

        //[ExpectedException(typeof(InvalidOperationException))]//Eğer beklediğimiz bir exception var ise bu şekilde tanımlayabiliriz
        [Fact]
        public void GetyId_With_Undefined_Id_Than_Exception_Occurred_Test()
        {
            //var expected = this.MockUserRepository.GetById(It.IsAny<int>());

            var ex = Assert.Throws<InvalidOperationException>(() => this.MockUserRepository.GetById(It.IsAny<int>()));
            //var ex = Assert.Throws<InvalidOperationException>(() => expected.Id);
            Assert.Equal("Sequence contains no matching element", ex.Message);
        }

        [Fact]
        public void Ipdate_User_Than_Check_It_Is_Updated_Test()
        {
            var actual = new User { Id = 2, FirstName = "User2_Updated", LastName = "User2LastName_Updated" };

            this.MockUserRepository.Update(actual);

            var expected = this.MockUserRepository.GetById(actual.Id);

            Assert.NotNull(expected);
            Assert.Equal(actual.FirstName, expected.FirstName);
            Assert.Equal(actual.LastName, expected.LastName);
        }

    }
}
