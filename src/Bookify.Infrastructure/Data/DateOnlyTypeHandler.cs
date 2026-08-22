using Dapper;
using System.Data;

namespace Bookify.Infrastructure.Data;

internal sealed class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
{
    // Dapper does not support DateTime type
    // Will be invoked when sending DateTime value to the DB or reading into memory
    public override DateOnly Parse(object value) => DateOnly.FromDateTime((DateTime)value);

    public override void SetValue(IDbDataParameter parameter, DateOnly value)
    {
        parameter.DbType = DbType.Date;
        parameter.Value = value;
    }
}
