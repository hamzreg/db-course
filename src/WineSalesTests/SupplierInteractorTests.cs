using Xunit;
using Moq;

using WineSales.Domain.Models;
using WineSales.Domain.RepositoryInterfaces;
using WineSales.Domain.Interactors;
using WineSales.Domain.Exceptions;

namespace DomainTests
{
    public class SupplierInteractorTests
    {
        private readonly ISupplierInteractor _interactor;
        private readonly ISupplierRepository _mockRepository;

        private readonly List<Supplier> mockSuppliers;
        private readonly List<SupplierWine> mockSupplierWine;

        public SupplierInteractorTests()
        {
            mockSuppliers = new List<Supplier>
            {
                new Supplier
                {
                    ID = 1,
                    Name = "Fanagoria",
                    Country = "Russia",
                    Experience = 65,
                    License = true,
                    Rating = 10
                },
                new Supplier
                {
                    ID = 2,
                    Name = "Agora",
                    Country = "Russia",
                    Experience = 25,
                    License = true,
                    Rating = 10
                },
                new Supplier
                {
                    ID = 3,
                    Name = "Alianta",
                    Country = "Russia",
                    Experience = 29,
                    License = true,
                    Rating = 10
                }
            };

            mockSupplierWine = new List<SupplierWine>
            {
                new SupplierWine
                {
                    ID = 1,
                    SupplierID = 1,
                    WineID = 1,
                    Percent = 50
                },
                new SupplierWine
                {
                    ID = 2,
                    SupplierID = 2,
                    WineID = 1,
                    Percent = 50
                },
                new SupplierWine
                {
                    ID = 3,
                    SupplierID = 1,
                    WineID = 2,
                    Percent = 50
                }
            };

            var mockRepository = new Mock<ISupplierRepository>();
            mockRepository.Setup(obj => obj.GetByID(It.IsAny<int>())).Returns(
                (int id) => mockSuppliers.Find(x => x.ID == id));
            mockRepository.Setup(obj => obj.GetByName(It.IsAny<string>())).Returns(
                (string name) => mockSuppliers.Find(x => x.Name == name));
            mockRepository.Setup(obj => obj.Create(It.IsAny<Supplier>())).Callback(
                (Supplier supplier) =>
                {
                    supplier.ID = mockSuppliers.Count + 1;
                    mockSuppliers.Add(supplier);
                }
                );
            mockRepository.Setup(obj => obj.GetBySupplierWineID(It.IsAny<int>())).Returns(
                (int id) =>
                {
                    var supplierWine = mockSupplierWine.Find(x => x.ID == id);
                    return mockSuppliers.Find(x => x.ID == supplierWine.SupplierID);
                }
                );
            mockRepository.Setup(obj => obj.Update(It.IsAny<Supplier>())).Callback(
                (Supplier supplier) =>
                {
                    mockSuppliers.FindAll(x => x.ID == supplier.ID)
                                 .ForEach(x =>
                                 {
                                     x.Name = supplier.Name;
                                     x.Country = supplier.Country;
                                     x.Experience = supplier.Experience;
                                     x.License = supplier.License;
                                     x.Rating = supplier.Rating;
                                 });
                }
                );
            mockRepository.Setup(obj => obj.Delete(It.IsAny<Supplier>())).Callback(
                (Supplier supplier) =>
                {
                    mockSuppliers.RemoveAll(x => x.ID == supplier.ID);
                }
                );

            _mockRepository = mockRepository.Object;
            _interactor = new SupplierInteractor(_mockRepository);
        }

        [Fact]
        public void CreateSupplierTest()
        {
            var expectedCount = mockSuppliers.Count + 1;

            var supplier = new Supplier
            {
                Name = "Castel",
                Country = "France",
                Experience = 73,
                License = true,
                Rating = 10
            };

            _interactor.CreateSupplier(supplier);
            Assert.Equal(expectedCount, mockSuppliers.Count);

            var suppliersList = mockSuppliers;
            Assert.All(suppliersList, obj => Assert.InRange(obj.ID, low: 1, high: expectedCount));
        }

        [Fact]
        public void AlreadyExistsCreateSupplierTest()
        {
            void action() => _interactor.CreateSupplier(mockSuppliers[0]);
            Assert.Throws<SupplierException>(action);

            var exception = Assert.Throws<SupplierException>(action);
            Assert.Equal("Supplier: This supplier already exists.",
                         exception.Message);
        }

        [Fact]
        public void GetBySupplierWineIDTest()
        {
            int supplierWineID = 1;
            var expectedSupplier = _mockRepository.GetByID(1);

            var supplier = _interactor.GetBySupplierWineID(supplierWineID);
            Assert.Equal(expectedSupplier, supplier);
        }

        [Fact]
        public void UpdateSupplierTest()
        {
            var expectedCount = mockSuppliers.Count;

            var supplier = new Supplier
            {
                ID = 1,
                Name = "Castel",
                Country = "France",
                Experience = 20,
                License = true,
                Rating  = 10
            };

            _interactor.UpdateSupplier(supplier);
            Assert.Equal(expectedCount, mockSuppliers.Count);

            var suppliersList = mockSuppliers;
            Assert.All(suppliersList, obj => Assert.InRange(obj.ID, low: 1, high: expectedCount));

            var updatedSupplier = mockSuppliers.Find(x => x.ID == supplier.ID);
            Assert.NotNull(updatedSupplier);
            Assert.Equal(supplier.ID, updatedSupplier?.ID);
            Assert.Equal(supplier.Name, updatedSupplier?.Name);
            Assert.Equal(supplier.Country, updatedSupplier?.Country);
            Assert.Equal(supplier.Experience, updatedSupplier?.Experience);
            Assert.Equal(supplier.License, updatedSupplier?.License);
            Assert.Equal(supplier.Rating, updatedSupplier?.Rating);
        }

        [Fact]
        public void NotExistsUpdateSupplierTest()
        {
            var supplier = new Supplier
            {
                ID = 5,
                Name = "Castel",
                Country = "France",
                Experience = 20,
                License = true,
                Rating = 10
            };

            void action() => _interactor.UpdateSupplier(supplier);
            Assert.Throws<SupplierException>(action);

            var exception = Assert.Throws<SupplierException>(action);
            Assert.Equal("Supplier: This supplier doesn't exist.",
                         exception.Message);
        }

        [Fact]
        public void DeleteSupplierTest()
        {
            var expectedCount = mockSuppliers.Count - 1;

            var supplier = new Supplier
            {
                ID = 2,
                Name = "Agora",
                Country = "Russia",
                Experience = 25,
                License = true,
                Rating = 10
            };

            _interactor.DeleteSupplier(supplier);
            Assert.Equal(expectedCount, mockSuppliers.Count);

            Assert.Null(mockSuppliers.Find(x => x.ID == supplier.ID));
        }

        [Fact]
        public void NotExistsDeleteSupplierTest()
        {
            var supplier = new Supplier
            {
                ID = 6,
                Name = "Castel",
                Country = "France",
                Experience = 20,
                License = true,
                Rating = 10
            };

            void action() => _interactor.DeleteSupplier(supplier);
            Assert.Throws<SupplierException>(action);

            var exception = Assert.Throws<SupplierException>(action);
            Assert.Equal("Supplier: This supplier doesn't exist.",
                         exception.Message);
        }
    }
}
