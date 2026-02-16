namespace RevisionNotes.Workflows.SagaOrchestration.Contracts;

public sealed record LoginRequest(string Username, string Password);
public sealed record StartOrderSagaRequest(string ProductCode, int Quantity, decimal Amount, bool SimulatePaymentFailure);
