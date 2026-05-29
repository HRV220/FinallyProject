using Core.Domain.Common;
using Core.Domain.Entities;

namespace Transactions.Domain.Entities;

public class Currency : BaseEntity
{
  public string Name { get; private set; } = string.Empty;
  public int Nominal { get; private set; }
  public decimal Rate { get; private set; }
  public string NumericCode { get; private set; } = string.Empty;
  public string Code { get; private set; } = string.Empty;
  public decimal UnitRate { get; private set; }

  private Currency() : base() { }

  private Currency(Guid id, string name, int nominal, decimal rate, string numericCode, string code, decimal unitRate) : base(id)
  {
    Name = name;
    Nominal = nominal;
    Rate = rate;
    NumericCode = numericCode;
    Code = code;
    UnitRate = unitRate;
  }

  public static Result<Currency> Create(string name, int nominal, decimal rate, string numericCode, string code, decimal unitRate)
  {
    if (string.IsNullOrWhiteSpace(name))
      return Result<Currency>.Failure(new DomainError("Currency.InvalidName", "Name is required."));

    if (string.IsNullOrWhiteSpace(code))
      return Result<Currency>.Failure(new DomainError("Currency.InvalidCode", "ISO symbolic code is required."));

    if (string.IsNullOrWhiteSpace(numericCode))
      return Result<Currency>.Failure(new DomainError("Currency.InvalidNumericCode", "ISO numeric code is required."));

    if (nominal <= 0)
      return Result<Currency>.Failure(new DomainError("Currency.InvalidNominal", "Nominal must be greater than 0."));

    if (rate <= 0)
      return Result<Currency>.Failure(new DomainError("Currency.InvalidRate", "Rate must be greater than 0."));

    return Result<Currency>.Success(new Currency(Guid.NewGuid(), name, nominal, rate, numericCode, code.ToUpper(), unitRate));
  }

  public void UpdateRate(decimal rate, decimal unitRate)
  {
    Rate = rate;
    UnitRate = unitRate;
    SetUpdated();
  }
}
