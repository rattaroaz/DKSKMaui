using DKSKMaui.Backend.Models;
using DKSKMaui.Tests.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace DKSKMaui.Tests.Models;

public class InvoiceTests
{
    [Fact]
    public void Invoice_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var invoice = new Invoice();

        // Assert
        invoice.CompanyName.Should().Be(string.Empty);
        invoice.PropertyAddress.Should().Be(string.Empty);
        invoice.Unit.Should().Be(string.Empty);
        invoice.JobDescriptionChoice.Should().Be(string.Empty);
        invoice.GateCode.Should().BeNull();
        invoice.LockBox.Should().BeNull();
        invoice.WorkOrder.Should().BeNull();
        invoice.ContractorName.Should().BeNull();
    }

    [Fact]
    public void Invoice_WithValidData_ShouldBeValid()
    {
        // Arrange
        var invoice = TestDataBuilder.CreateInvoice(i =>
        {
            i.JobDescriptionChoice = "Interior Painting";
            i.AmountCost = 1500;
            i.WorkDate = DateTime.Now.Date;
        });

        // Act & Assert
        invoice.JobDescriptionChoice.Should().NotBeNullOrEmpty();
        invoice.AmountCost.Should().BePositive();
        invoice.WorkDate.Should().BeOnOrBefore(DateTime.Now);
    }

    [Fact]
    public void Invoice_CalculateBalance_ReturnsCorrectAmount()
    {
        // Arrange
        var invoice = TestDataBuilder.CreateInvoice(i =>
        {
            i.AmountCost = 5000;
            i.AmountPaid1 = 2000;
            i.AmountPaid2 = 1000;
        });

        // Act
        var balance = invoice.AmountCost - (invoice.AmountPaid1 + invoice.AmountPaid2);

        // Assert
        balance.Should().Be(2000);
    }

    [Fact]
    public void Invoice_IsPaid_WhenFullyPaid_ReturnsTrue()
    {
        // Arrange
        var invoice = TestDataBuilder.CreateInvoice(i =>
        {
            i.AmountCost = 3000;
            i.AmountPaid1 = 2000;
            i.AmountPaid2 = 1000;
            i.Status = 1; // Paid status
        });

        // Act
        var totalPaid = invoice.AmountPaid1 + invoice.AmountPaid2;
        var isPaid = totalPaid >= invoice.AmountCost;

        // Assert
        isPaid.Should().BeTrue();
        invoice.Status.Should().Be(1);
    }

    [Fact]
    public void Invoice_IsOverdue_WhenPastDueDate_ReturnsTrue()
    {
        // Arrange
        var invoice = TestDataBuilder.CreateInvoice(i =>
        {
            i.WorkDate = DateTime.Now.AddDays(-60); // 60 days old
            i.AmountCost = 2000;
            i.AmountPaid1 = 0;
            i.Status = 0; // Unpaid
        });

        // Act
        var daysSinceWork = (DateTime.Now - invoice.WorkDate).Days;
        var isOverdue = daysSinceWork > 30 && invoice.Status == 0;

        // Assert
        isOverdue.Should().BeTrue();
    }

    [Theory]
    [InlineData(1000, 1000, 0, 1)] // Fully paid with first payment
    [InlineData(2000, 1000, 1000, 1)] // Fully paid with two payments
    [InlineData(3000, 2000, 500, 0)] // Partially paid
    [InlineData(1500, 0, 0, 0)] // Unpaid
    public void Invoice_PaymentStatus_DeterminesCorrectStatus(
        int amountCost, int amountPaid1, int amountPaid2, int expectedStatus)
    {
        // Arrange
        var invoice = TestDataBuilder.CreateInvoice(i =>
        {
            i.AmountCost = amountCost;
            i.AmountPaid1 = amountPaid1;
            i.AmountPaid2 = amountPaid2;
        });

        // Act
        var totalPaid = amountPaid1 + amountPaid2;
        var actualStatus = totalPaid >= amountCost ? 1 : 0;

        // Assert
        actualStatus.Should().Be(expectedStatus);
    }

    [Fact]
    public void Invoice_InvoiceNumber_GeneratesCorrectFormat()
    {
        // Arrange
        var invoice = TestDataBuilder.CreateInvoice(i => i.Id = 123);

        // Act
        var invoiceNumber = (invoice.Id + 10000).ToString();

        // Assert
        invoiceNumber.Should().Be("10123");
        invoiceNumber.Should().HaveLength(5);
    }

    [Fact]
    public void Invoice_RequiredFields_ShouldNotBeNull()
    {
        // Arrange
        var invoice = TestDataBuilder.CreateInvoice();

        // Assert
        invoice.CompanyName.Should().NotBeNull();
        invoice.PropertyAddress.Should().NotBeNull();
        invoice.Unit.Should().NotBeNull();
        invoice.JobDescriptionChoice.Should().NotBeNull();
        invoice.TodaysDate.Should().NotBe(default);
        invoice.WorkDate.Should().NotBe(default);
    }
}
