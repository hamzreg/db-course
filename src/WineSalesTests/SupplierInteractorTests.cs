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
        private readonly List<Tuple<int, int>> mockSupplierWine;
        public SupplierInteractorTests()
        {
            mockSuppliers = new List<Supplier>
            {
                new Supplier
                {
                    ID = 1,
                    Name = "Fanagoria"
                },
                new Supplier
                {
                    ID = 2,
                    Name = "Agora"
                },
                new Supplier
                {
                    ID = 3,
                    Name = "Alianta"
                }
            };

            mockSupplierWine = new List<Tuple<int, int>>
            {
                new Tuple<int, int>(1, 2),
                new Tuple<int, int>(1, 3),
                new Tuple<int, int>(2, 1)
            };

            var mockRepository = new Mock<ISupplierRepository>();
            mockRepository.Setup(obj => obj.GetByID(It.IsAny<int>())).Returns(
                (int id) => mockSuppliers.Find(x => x.ID == id));
            mockRepository.Setup(obj => obj.Create(It.IsAny<Supplier>())).Callback(
                (Supplier supplier) =>
                {
                    supplier.ID = mockSuppliers.Count + 1;
                    mockSuppliers.Add(supplier);
                }
                );
            mockRepository.Setup(obj => obj.GetByWineID(It.IsAny<int>())).Returns(
                (int id) =>
                {
                    var supplierWine = mockSupplierWine.Find(x => x.Item2 == id);
                    return mockSuppliers.Find(x => x.ID == supplierWine.Item1);
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
                Country = "France"
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
        public void GetByWineIDTest()
        {
            int wineID = 3;

            var supplierWine = mockSupplierWine.Find(x => x.Item2 == wineID);
            var expectedSupplier = _mockRepository.GetByID(supplierWine.Item1);

            var supplier = _interactor.GetByWineID(wineID);
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
            Assert.Equal(updatedSupplier?.ID, supplier.ID);
            Assert.Equal(updatedSupplier?.Name, supplier.Name);
            Assert.Equal(updatedSupplier?.Country, supplier.Country);
            Assert.Equal(updatedSupplier?.Experience, supplier.Experience);
            Assert.Equal(updatedSupplier?.License, supplier.License);
            Assert.Equal(updatedSupplier?.Rating, supplier.Rating);
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
                Name = "Castel",
                Country = "France",
                Experience = 20,
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
