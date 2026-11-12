using Anonymazer.Models;
using Anonymazer.Services;

namespace Anonymazer.Tests;

public class AnonymizationServiceTests
{
    private readonly AnonymizationService _service = new(
        new IDataDetector[]
        {
        new EmailDetector(),
        new PassportDetector(),
        new PhoneDetector(),
        new SnilsDetector(),
        new InnDetector(),
        new FullNameDetector(),
        new BankCardDetector()
        });

    [Fact]
    public void Anonymize_Email_ReplacesEmail()
    {
        var result = _service.Anonymize(
            "Моя почта: ivan.petrov@example.com");

        Assert.Equal("Моя почта: [EMAIL]", result);
    }

    [Fact]
    public void Anonymize_Phone_ReplacesPhone()
    {
        var result = _service.Anonymize(
            "Мой телефон: +7 (999) 123-45-67");

        Assert.Equal("Мой телефон: [PHONE]", result);
    }

    [Fact]
    public void Anonymize_Passport_ReplacesPassport()
    {
        var result = _service.Anonymize(
            "Мой паспорт: 45 12 123456");

        Assert.Equal("Мой паспорт: [PASSPORT]", result);
    }


    [Fact]
    public void Anonymize_Snils_ReplacesSnils()
    {
        var result = _service.Anonymize(
            "СНИЛС: 123-456-789 64");

        Assert.Equal("СНИЛС: [SNILS]", result);
    }

    [Fact]
    public void Anonymize_Inn_ReplacesInn()
    {
        var result = _service.Anonymize(
            "ИНН: 500100732259");

        Assert.Equal("ИНН: [INN]", result);
    }

    [Fact]
    public void Anonymize_FullName_ReplacesFullName()
    {
        var result = _service.Anonymize(
            "Клиент: Иванов Иван Иванович");

        Assert.Equal("Клиент: Иванов И.И.", result);
    }

    [Fact]
    public void Anonymize_BankCard_MasksCardNumber()
    {
        var result = _service.Anonymize(
            "Карта: 4111 1111 1111 1111");

        Assert.Equal("Карта: 4111************", result);
    }

    [Fact]
    public void Anonymize_InvalidSnils_RemainsUnchanged()
    {
        var result = _service.Anonymize(
            "СНИЛС: 123-456-789 65");

        Assert.Equal("СНИЛС: 123-456-789 65", result);
    }

    [Fact]
    public void Anonymize_InvalidInn_RemainsUnchanged()
    {
        var result = _service.Anonymize(
            "ИНН: 500100732250");

        Assert.Equal("ИНН: 500100732250", result);
    }

    [Fact]
    public void Anonymize_InvalidBankCard_RemainsUnchanged()
    {
        var result = _service.Anonymize(
            "Карта: 4111 1111 1111 1112");

        Assert.Equal("Карта: 4111 1111 1111 1112", result);
    }


    [Fact]
    public void Anonymize_TextWithoutSensitiveData_RemainsUnchanged()
    {
        var text = "Привет! Сегодня хорошая погода.";

        var result = _service.Anonymize(text);

        Assert.Equal(text, result);
    }
    [Fact]
    public void AnonymizeWithStatistics_CountsDetectedData()
    {
        var result = _service.AnonymizeWithStatistics(
            "Почта: ivan.petrov@example.com, " +
            "телефон: +7 (999) 123-45-67, " +
            "паспорт: 45 12 123456, " +
            "СНИЛС: 123-456-789 64, " +
            "ИНН: 500100732259, " +
            "ФИО: Иванов Иван Иванович, " +
            "карта: 4111 1111 1111 1111");

        Assert.Equal(1, result.DetectedCounts["EMAIL"]);
        Assert.Equal(1, result.DetectedCounts["PHONE"]);
        Assert.Equal(1, result.DetectedCounts["PASSPORT"]);
        Assert.Equal(1, result.DetectedCounts["SNILS"]);
        Assert.Equal(1, result.DetectedCounts["INN"]);
        Assert.Equal(1, result.DetectedCounts["FULL_NAME"]);
        Assert.Equal(1, result.DetectedCounts["BANK_CARD"]);
    }


    [Fact]
    public void AnonymizeWithStatistics_EmptyText_ReturnsEmptyResult()
    {
        var result = _service.AnonymizeWithStatistics("");

        Assert.Equal("", result.OriginalText);
        Assert.Equal("", result.AnonymizedText);
        Assert.Empty(result.DetectedCounts);
    }
    [Fact]
    public void Anonymize_UnlabeledSensitiveData_DetectsAllTypes()
    {
        var result = _service.Anonymize(
            "Иванов Иван Иванович, " +
            "ivan.petrov@example.com, " +
            "+7 (999) 123-45-67, " +
            "45 12 123456, " +
            "123-456-789 64, " +
            "500100732259, " +
            "4111 1111 1111 1111");

        Assert.Contains("[EMAIL]", result);
        Assert.Contains("[PHONE]", result);
        Assert.Contains("[PASSPORT]", result);
        Assert.Contains("[SNILS]", result);
        Assert.Contains("[INN]", result);
        Assert.Contains("Иванов И.И.", result);
        Assert.Contains("4111************", result);
    }

    [Fact]
    public void Anonymize_OverlappingDetectors_HigherPriorityWins()
    {
        var service = new AnonymizationService(new IDataDetector[]
        {
        new TestDetector(new DetectedData
        {
            Start = 0,
            Length = 4,
            Type = "HIGH",
            Replacement = "[HIGH]",
            Priority = 1
        }),
        new TestDetector(new DetectedData
        {
            Start = 0,
            Length = 4,
            Type = "LOW",
            Replacement = "[LOW]",
            Priority = 100
        })
        });

        var result = service.Anonymize("test");

        Assert.Equal("[HIGH]", result);
    }

    [Fact]
    public void Anonymize_PartiallyOverlappingDetectors_HigherPriorityWins()
    {
        var service = new AnonymizationService(new IDataDetector[]
        {
        new TestDetector(new DetectedData
        {
            Start = 1,
            Length = 4,
            Type = "FIRST",
            Replacement = "[FIRST]",
            Priority = 1
        }),
        new TestDetector(new DetectedData
        {
            Start = 3,
            Length = 4,
            Type = "SECOND",
            Replacement = "[SECOND]",
            Priority = 2
        })
        });

        var result = service.Anonymize("abcdefgh");

        Assert.Equal("a[FIRST]fgh", result);
    }

    [Fact]
    public void Anonymize_NonOverlappingDetectors_BothAreApplied()
    {
        var service = new AnonymizationService(new IDataDetector[]
        {
        new TestDetector(new DetectedData
        {
            Start = 0,
            Length = 2,
            Type = "FIRST",
            Replacement = "[FIRST]",
            Priority = 1
        }),
        new TestDetector(new DetectedData
        {
            Start = 2,
            Length = 2,
            Type = "SECOND",
            Replacement = "[SECOND]",
            Priority = 2
        })
        });

        var result = service.Anonymize("abcdef");

        Assert.Equal("[FIRST][SECOND]ef", result);
    }

    [Fact]
    public void Anonymize_SamePriorityAndStart_LongestMatchWins()
    {
        var service = new AnonymizationService(new IDataDetector[]
        {
        new TestDetector(new DetectedData
        {
            Start = 1,
            Length = 2,
            Type = "SHORT",
            Replacement = "[SHORT]",
            Priority = 1
        }),
        new TestDetector(new DetectedData
        {
            Start = 1,
            Length = 3,
            Type = "LONG",
            Replacement = "[LONG]",
            Priority = 1
        })
        });

        var result = service.Anonymize("abcdef");

        Assert.Equal("a[LONG]ef", result);
    }

    private sealed class TestDetector : IDataDetector
    {
        private readonly DetectedData[] _data;

        public TestDetector(params DetectedData[] data)
        {
            _data = data;
        }

        public IEnumerable<DetectedData> Detect(string text)
        {
            return _data;
        }
    }

    [Fact]
    public void AnonymizeWithStatistics_OverlappingDetectors_CountsOnlySelectedData()
    {
        var service = new AnonymizationService(new IDataDetector[]
        {
        new TestDetector(new DetectedData
        {
            Start = 0,
            Length = 4,
            Type = "HIGH",
            Replacement = "[HIGH]",
            Priority = 1
        }),
        new TestDetector(new DetectedData
        {
            Start = 0,
            Length = 4,
            Type = "LOW",
            Replacement = "[LOW]",
            Priority = 100
        })
        });

        var result = service.AnonymizeWithStatistics("test");

        Assert.Equal("[HIGH]", result.AnonymizedText);
        Assert.Equal(1, result.DetectedCounts["HIGH"]);
        Assert.False(result.DetectedCounts.ContainsKey("LOW"));
    }

    [Fact]
    public void Anonymize_InvalidDetectorPositions_AreIgnored()
    {
        var service = new AnonymizationService(new IDataDetector[]
        {
        new TestDetector(
            new DetectedData
            {
                Start = -1,
                Length = 2,
                Type = "NEGATIVE",
                Replacement = "[NEGATIVE]"
            },
            new DetectedData
            {
                Start = 0,
                Length = 0,
                Type = "EMPTY",
                Replacement = "[EMPTY]"
            },
            new DetectedData
            {
                Start = 2,
                Length = 10,
                Type = "OUT_OF_RANGE",
                Replacement = "[OUT_OF_RANGE]"
            },
            new DetectedData
            {
                Start = 0,
                Length = 10,
                Type = "OVERFLOW",
                Replacement = "[OVERFLOW]"
            })
        });

        var result = service.AnonymizeWithStatistics("test");

        Assert.Equal("test", result.AnonymizedText);
        Assert.Empty(result.DetectedCounts);
    }

}