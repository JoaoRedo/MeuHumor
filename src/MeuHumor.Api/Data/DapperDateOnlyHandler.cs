using System.Data;
using Dapper;

namespace MeuHumor.Api.Data;

public sealed class DapperDateOnlyHandler : SqlMapper.TypeHandler<DateOnly>
{
    public override void SetValue(IDbDataParameter parameter, DateOnly value) =>
        parameter.Value = value;

    public override DateOnly Parse(object value) => value switch
    {
        DateOnly date => date,
        DateTime dateTime => DateOnly.FromDateTime(dateTime),
        _ => DateOnly.FromDateTime(Convert.ToDateTime(value))
    };
}
