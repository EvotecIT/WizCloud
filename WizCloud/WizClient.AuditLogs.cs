using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace WizCloud;

public partial class WizClient {
    /// <summary>
    /// Retrieves audit logs from Wiz.
    /// </summary>
    /// <param name="pageSize">The number of audit logs to retrieve per page.</param>
    /// <param name="startDate">Optional start date filter.</param>
    /// <param name="endDate">Optional end date filter.</param>
    /// <param name="user">Optional user filter.</param>
    /// <param name="action">Optional action filter.</param>
    /// <param name="status">Optional status filter.</param>
    /// <returns>A list of audit log entries.</returns>
    public async Task<List<WizAuditLogEntry>> GetAuditLogsAsync(
        int pageSize = 20,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? user = null,
        string? action = null,
        string? status = null) {
        var logs = new List<WizAuditLogEntry>();
        string? endCursor = null;
        bool hasNextPage = true;

        while (hasNextPage) {
            var result = await GetAuditLogsPageAsync(pageSize, endCursor, startDate, endDate, user, action, status).ConfigureAwait(false);
            logs.AddRange(result.Logs);
            hasNextPage = result.HasNextPage;
            endCursor = result.EndCursor;
        }

        return logs;
    }

    /// <summary>
    /// Streams audit logs from Wiz as an asynchronous sequence.
    /// </summary>
    /// <param name="pageSize">The number of audit logs to retrieve per page.</param>
    /// <param name="startDate">Optional start date filter.</param>
    /// <param name="endDate">Optional end date filter.</param>
    /// <param name="user">Optional user filter.</param>
    /// <param name="action">Optional action filter.</param>
    /// <param name="status">Optional status filter.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>An async enumerable of audit log entries.</returns>
    public async IAsyncEnumerable<WizAuditLogEntry> GetAuditLogsAsyncEnumerable(
        int pageSize = 20,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? user = null,
        string? action = null,
        string? status = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default) {
        string? endCursor = null;
        bool hasNextPage = true;

        while (!cancellationToken.IsCancellationRequested && hasNextPage) {
            (List<WizAuditLogEntry> Logs, bool HasNextPage, string? EndCursor) result;
            try {
                result = await GetAuditLogsPageAsync(pageSize, endCursor, startDate, endDate, user, action, status).ConfigureAwait(false);
            } catch (HttpRequestException) {
                yield break;
            }

            foreach (var log in result.Logs) {
                if (cancellationToken.IsCancellationRequested)
                    yield break;

                yield return log;
            }

            hasNextPage = result.HasNextPage;
            endCursor = result.EndCursor;
        }
    }

    /// <summary>
    /// Retrieves a single page of audit logs from the Wiz API.
    /// </summary>
    /// <param name="first">The number of audit logs to retrieve.</param>
    /// <param name="after">Cursor for pagination.</param>
    /// <param name="startDate">Optional start date filter.</param>
    /// <param name="endDate">Optional end date filter.</param>
    /// <param name="user">Optional user filter.</param>
    /// <param name="action">Optional action filter.</param>
    /// <param name="status">Optional status filter.</param>
    /// <returns>A tuple containing the logs, pagination flag, and end cursor.</returns>
    private async Task<(List<WizAuditLogEntry> Logs, bool HasNextPage, string? EndCursor)> GetAuditLogsPageAsync(
        int first,
        string? after = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? user = null,
        string? action = null,
        string? status = null) {
        const string query = GraphQlQueries.AuditLogsQuery;

        var variables = new {
            first,
            after,
            filterBy = new {
                startTime = startDate,
                endTime = endDate,
                user = string.IsNullOrEmpty(user) ? null : new { equals = new[] { user } },
                action = string.IsNullOrEmpty(action) ? null : new { equals = new[] { action } },
                status = string.IsNullOrEmpty(status) ? null : new { equals = new[] { status } }
            }
        };

        var requestBody = new { query, variables };
        var jsonResponse = await SendGraphQlRequestAsync(requestBody).ConfigureAwait(false);

        var logs = new List<WizAuditLogEntry>();
        var nodes = jsonResponse["data"]?["auditLogs"]?["nodes"]?.AsArray();
        if (nodes != null) {
            foreach (JsonNode? node in nodes) {
                if (node != null) {
                    var log = node.Deserialize<WizAuditLogEntry>();
                    if (log != null)
                        logs.Add(log);
                }
            }
        }

        var pageInfo = jsonResponse["data"]?["auditLogs"]?["pageInfo"];
        bool hasNextPage = pageInfo?["hasNextPage"]?.GetValue<bool>() ?? false;
        string? endCursor = pageInfo?["endCursor"]?.GetValue<string>();

        return (logs, hasNextPage, endCursor);
    }
}