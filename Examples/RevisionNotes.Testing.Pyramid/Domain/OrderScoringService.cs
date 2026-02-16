namespace RevisionNotes.Testing.Pyramid.Domain;

public sealed class OrderScoringService
{
    public int CalculateRiskScore(decimal totalAmount, bool isPriorityCustomer, bool hasFraudSignals)
    {
        var baseScore = totalAmount >= 1000 ? 60 : 25;
        if (isPriorityCustomer)
        {
            baseScore -= 10;
        }

        if (hasFraudSignals)
        {
            baseScore += 30;
        }

        return Math.Clamp(baseScore, 0, 100);
    }
}
