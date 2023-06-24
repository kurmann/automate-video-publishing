namespace AutomateVideoPublishing.Entities.Elementary;

public class PositiveInteger
{
    public int Value { get; }

    private PositiveInteger(int value) => Value = value;

    public static Result<PositiveInteger> Create(int value)
    {
        if (value <= 0)
        {
            return Result.Failure<PositiveInteger>("Value should be positive.");
        }

        return Result.Success(new PositiveInteger(value));
    }

    public static PositiveInteger Default => new PositiveInteger(1);

    public static implicit operator int(PositiveInteger positiveInteger) => positiveInteger.Value;
}
