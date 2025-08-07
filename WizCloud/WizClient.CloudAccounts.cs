using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WizCloud;

/// <summary>
/// Provides cloud account operations for <see cref="WizClient"/>.
/// </summary>
public partial class WizClient {
    /// <summary>
    /// Retrieves all cloud accounts from Wiz asynchronously.
    /// </summary>
    /// <param name="pageSize">The number of cloud accounts to retrieve per page. Defaults to 20.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of all cloud accounts.</returns>
    public async Task<List<WizCloudAccount>> GetCloudAccountsAsync(int pageSize = 20) {
        var accounts = new List<WizCloudAccount>();
        string? endCursor = null;
        bool hasNextPage = true;

        while (hasNextPage) {
            var result = await GetCloudAccountsPageAsync(pageSize, endCursor).ConfigureAwait(false);
            accounts.AddRange(result.Accounts);
            hasNextPage = result.HasNextPage;
            endCursor = result.EndCursor;
        }

        return accounts;
    }

    /// <summary>
    /// Streams cloud accounts from Wiz asynchronously as an <see cref="IAsyncEnumerable{WizCloudAccount}"/>.
    /// </summary>
    /// <param name="pageSize">The number of cloud accounts to retrieve per page. Defaults to 20.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>An async enumerable sequence of cloud accounts.</returns>
    public async IAsyncEnumerable<WizCloudAccount> GetCloudAccountsAsyncEnumerable(int pageSize = 20, [EnumeratorCancellation] CancellationToken cancellationToken = default) {
        string? endCursor = null;
        bool hasNextPage = true;

        while (!cancellationToken.IsCancellationRequested && hasNextPage) {
            var result = await GetCloudAccountsPageAsync(pageSize, endCursor).ConfigureAwait(false);

            foreach (var account in result.Accounts) {
                if (cancellationToken.IsCancellationRequested)
                    yield break;

                yield return account;
            }

            hasNextPage = result.HasNextPage;
            endCursor = result.EndCursor;
        }
    }

    /// <summary>
    /// Retrieves a single page of cloud accounts from the Wiz API.
    /// </summary>
    /// <param name="first">The number of cloud accounts to retrieve.</param>
    /// <param name="after">The cursor for pagination, if retrieving subsequent pages.</param>
    /// <returns>A tuple containing the cloud accounts, whether there's a next page, and the cursor for the next page.</returns>
    private async Task<(List<WizCloudAccount> Accounts, bool HasNextPage, string? EndCursor)> GetCloudAccountsPageAsync(int first, string? after = null) {
        const string query = GraphQlQueries.CloudAccountsQuery;

        var variables = new {
            first,
            after
        };

        var requestBody = new {
            query,
            variables
        };

        var jsonResponse = await SendGraphQlRequestAsync(requestBody).ConfigureAwait(false);

        var accounts = new List<WizCloudAccount>();
        var nodes = jsonResponse["data"]?["cloudAccounts"]?["nodes"]?.AsArray();

        if (nodes != null) {
            foreach (var node in nodes) {
                if (node != null) {
                    accounts.Add(WizCloudAccount.FromJson(node));
                }
            }
        }

        var pageInfo = jsonResponse["data"]?["cloudAccounts"]?["pageInfo"];
        bool hasNextPage = pageInfo?["hasNextPage"]?.GetValue<bool>() ?? false;
        string? endCursor = pageInfo?["endCursor"]?.GetValue<string>();

        return (accounts, hasNextPage, endCursor);
    }
}
