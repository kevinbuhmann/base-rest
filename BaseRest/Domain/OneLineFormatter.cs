using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Text.RegularExpressions;

public class OneLineFormatter : DatabaseLogFormatter
{
    private readonly string contextName;

    public OneLineFormatter(DbContext context, Action<string> writeAction)
        : base(context, writeAction)
    {
        this.contextName = context.GetType().Name;
    }

    public override void LogCommand<TResult>(
        DbCommand command, DbCommandInterceptionContext<TResult> interceptionContext)
    {
        string commandStr = Regex.Replace(command.CommandText, "\\s+", " ").Trim();
        this.Write($"Context '{this.contextName}' is executing command '{commandStr}'{Environment.NewLine}");
    }

    public override void LogResult<TResult>(
        DbCommand command, DbCommandInterceptionContext<TResult> interceptionContext)
    {
    }
}