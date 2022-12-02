namespace TraderBot.Abstractions;

public abstract record RecordWithValidation
{
    protected RecordWithValidation()
    {
        Validate();
    }

    protected virtual void Validate()
    {
    }
}