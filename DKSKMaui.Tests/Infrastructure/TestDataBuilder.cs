using Bogus;
using DKSKMaui.Backend.Models;
using System;
using System.Threading;

namespace DKSKMaui.Tests.Infrastructure;
/// <summary>
/// Provides test data generation using Bogus faker library
/// </summary>
public static class TestDataBuilder
{
    private static readonly Faker _faker = new();
    private static int _idCounter = 1;
    
    /// <summary>
    /// Reset the ID counter for test isolation
    /// </summary>
    public static void ResetIdCounter()
    {
        _idCounter = 1;
    }

    /// <summary>
    /// Creates a valid Company entity with optional overrides
    /// </summary>
    public static Companny CreateCompany(Action<Companny>? customize = null)
    {
        var company = new Companny
        {
            Id = Interlocked.Increment(ref _idCounter),
            Name = _faker.Company.CompanyName(),
            Address = _faker.Address.StreetAddress(),
            Phone = _faker.Phone.PhoneNumber("###-###-####"),
            Email = _faker.Internet.Email(),
            City = _faker.Address.City(),
            Zip = _faker.Address.ZipCode(),
            Supervisors = new List<Supervisor>()
        };

        customize?.Invoke(company);
        return company;
    }

    /// <summary>
    /// Creates a valid Contractor entity with optional overrides
    /// </summary>
    public static Contractor CreateContractor(Action<Contractor>? customize = null)
    {
        var contractor = new Contractor
        {
            Id = Interlocked.Increment(ref _idCounter),
            Name = _faker.Name.FullName(),
            Address = _faker.Address.StreetAddress(),
            CellPhone = _faker.Phone.PhoneNumber("###-###-####"),
            Email = _faker.Internet.Email(),
            SocailSecurityNumber = _faker.Random.Replace("###-##-####"),
            LicenseNumber = _faker.Random.AlphaNumeric(10).ToUpper(),
            ContractorID = _faker.Random.AlphaNumeric(8).ToUpper(),
            PayrollPercent = _faker.Random.Int(10, 30).ToString(),
            City = _faker.Address.City(),
            Zip = _faker.Address.ZipCode(),
            SpecialNote = _faker.Lorem.Sentence(),
            IsActive = _faker.Random.Bool()
        };

        customize?.Invoke(contractor);
        return contractor;
    }

    /// <summary>
    /// Creates a valid Invoice entity with optional overrides
    public static Invoice CreateInvoice(Action<Invoice>? customize = null)
    {
        var invoice = new Invoice
        {
            WorkOrder = "WO-1001",
            JobDescriptionChoice = "Interior Painting",
            ContractorName = "Test Contractor",
            CompanyName = "Test Company",
            PropertyAddress = "123 Test St",
            Unit = "Unit 1",
            AmountCost = 1000,
            AmountPaid1 = 0,
            CheckNumber1 = null,
            AmountPaid2 = 0,
            DatePaid1 = new DateTime(2000, 1, 1),
            DatePaid2 = new DateTime(2000, 1, 1),
            CheckNumber2 = null,
            TodaysDate = DateTime.Now,
            WorkDate = DateTime.Now.AddDays(-30),
            InvoiceCreatedDate = DateTime.Now,
            SpecialNote = "Test note",
            GarageRemoteCode = "1234",
            GateCode = null,
            LockBox = null,
            SizeBedroom = 2,
            SizeBathroom = 1,
            Status = 0
        };

        customize?.Invoke(invoice);
        return invoice;
    }

    /// <summary>
    /// Creates a valid Properties entity with optional overrides
    /// </summary>
    public static Properties CreateProperty(Action<Properties>? customize = null)
    {
        var property = new Properties
        {
            Id = Interlocked.Increment(ref _idCounter),
            Name = _faker.Company.CompanyName(),
            Address = _faker.Address.StreetAddress(),
            City = _faker.Address.City(),
            Zip = _faker.Address.ZipCode(),
            GateCode = _faker.Random.AlphaNumeric(6),
            LockBox = _faker.Random.AlphaNumeric(4),
            GarageRemoteCode = _faker.Random.AlphaNumeric(8),
            SpecialNote = _faker.Lorem.Sentence(),
            SupervisorId = _faker.Random.Int(1, 100)
        };

        customize?.Invoke(property);
        return property;
    }

    /// <summary>
    /// Creates a valid Supervisor entity with optional overrides
    /// </summary>
    public static Supervisor CreateSupervisor(Action<Supervisor>? customize = null)
    {
        var supervisor = new Supervisor
        {
            Id = Interlocked.Increment(ref _idCounter),
            Name = _faker.Name.FullName(),
            Phone = _faker.Phone.PhoneNumber("###-###-####"),
            Email = _faker.Internet.Email(),
            CompanyId = _faker.Random.Int(1, 100)
        };

        customize?.Invoke(supervisor);
        return supervisor;
    }

    /// <summary>
    /// Creates a valid JobDiscription entity with optional overrides
    /// </summary>
    public static JobDiscription CreateJobDescription(Action<JobDiscription>? customize = null)
    {
        var jobDescription = new JobDiscription
        {
            Id = Interlocked.Increment(ref _idCounter),
            description = _faker.PickRandom(new[] {
                "Interior Painting",
                "Exterior Painting",
                "Drywall Repair",
                "Cabinet Painting",
                "Deck Staining",
                "Pressure Washing"
            }),
            sizeBathroom = _faker.Random.Int(1, 3),
            sizeBedroom = _faker.Random.Int(1, 5),
            price = _faker.Random.Int(100, 2000)
        };

        customize?.Invoke(jobDescription);
        return jobDescription;
    }

    /// <summary>
    /// Creates a valid MyCompanyInfo entity with optional overrides
    /// </summary>
    public static MyCompanyInfo CreateMyCompanyInfo(Action<MyCompanyInfo>? customize = null)
    {
        var myCompanyInfo = new MyCompanyInfo
        {
            Id = Interlocked.Increment(ref _idCounter),
            Name = "DKSK Official Painting",
            Phone = _faker.Phone.PhoneNumber("###-###-####"),
            Email = _faker.Internet.Email(),
            Address = _faker.Address.StreetAddress(),
            LicenseNumber = $"LIC-{_faker.Random.AlphaNumeric(10).ToUpper()}",
            Zip = _faker.Address.ZipCode()
        };

        customize?.Invoke(myCompanyInfo);
        return myCompanyInfo;
    }

    /// <summary>
    /// Creates a list of entities
    /// </summary>
    public static List<T> CreateMany<T>(Func<T> creator, int count = 5)
    {
        var items = new List<T>();
        for (int i = 0; i < count; i++)
        {
            items.Add(creator());
        }
        return items;
    }
}
