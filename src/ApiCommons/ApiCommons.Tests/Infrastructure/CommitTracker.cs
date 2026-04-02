using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ApiCommons.Tests.Infrastructure;

/// <summary>
/// EF Core interceptor that records whether the most recent transaction was committed or rolled back.
/// </summary>
internal sealed class CommitTracker : DbTransactionInterceptor
{
    public bool Committed { get; private set; }
    public bool RolledBack { get; private set; }

    public override Task TransactionCommittedAsync(
        DbTransaction transaction,
        TransactionEndEventData eventData,
        CancellationToken cancellationToken = default)
    {
        Committed = true;
        return base.TransactionCommittedAsync(transaction, eventData, cancellationToken);
    }

    public override Task TransactionRolledBackAsync(
        DbTransaction transaction,
        TransactionEndEventData eventData,
        CancellationToken cancellationToken = default)
    {
        RolledBack = true;
        return base.TransactionRolledBackAsync(transaction, eventData, cancellationToken);
    }
}
